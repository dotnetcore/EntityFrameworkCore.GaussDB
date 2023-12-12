// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Design.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Design.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
#pragma warning disable EF1001 // Internal EF Core API usage.
public class GaussDBCSharpRuntimeAnnotationCodeGenerator : RelationalCSharpRuntimeAnnotationCodeGenerator
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBCSharpRuntimeAnnotationCodeGenerator(
        CSharpRuntimeAnnotationCodeGeneratorDependencies dependencies,
        RelationalCSharpRuntimeAnnotationCodeGeneratorDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override bool Create(
        CoreTypeMapping typeMapping,
        CSharpRuntimeAnnotationCodeGeneratorParameters parameters,
        ValueComparer? valueComparer = null,
        ValueComparer? keyValueComparer = null,
        ValueComparer? providerValueComparer = null)
    {
        var result = base.Create(typeMapping, parameters, valueComparer, keyValueComparer, providerValueComparer);

        var mainBuilder = parameters.MainBuilder;

        var GaussDBDbTypeBasedDefaultInstance = typeMapping switch
        {
            GaussDBStringTypeMapping => GaussDBStringTypeMapping.Default,
            GaussDBULongTypeMapping => GaussDBULongTypeMapping.Default,
            // GaussDBMultirangeTypeMapping => GaussDBMultirangeTypeMapping.Default,
            _ => (IGaussDBTypeMapping?)null
        };

        if (GaussDBDbTypeBasedDefaultInstance is not null)
        {
            var GaussDBDbType = ((IGaussDBTypeMapping)typeMapping).GaussDBDbType;

            if (GaussDBDbType != GaussDBDbTypeBasedDefaultInstance.GaussDBDbType)
            {
                mainBuilder.AppendLine(";");

                mainBuilder.Append(
                    $"{parameters.TargetName}.TypeMapping = (({typeMapping.GetType().Name}){parameters.TargetName}.TypeMapping).Clone(GaussDBDbType: ");

                mainBuilder
                    .Append(nameof(GaussDBTypes))
                    .Append(".")
                    .Append(nameof(GaussDBDbType))
                    .Append(".")
                    .Append(GaussDBDbType.ToString());

                mainBuilder
                    .Append(")")
                    .DecrementIndent();
            }

        }

        switch (typeMapping)
        {
#pragma warning disable CS0618 // GaussDBConnection.GlobalTypeMapper is obsolete
            case GaussDBEnumTypeMapping enumTypeMapping:
                if (enumTypeMapping.NameTranslator != GaussDBConnection.GlobalTypeMapper.DefaultNameTranslator)
                {
                    throw new NotSupportedException(
                        "Mapped enums are only supported in the compiled model if they use the default name translator");
                }
                break;
#pragma warning restore CS0618

            case GaussDBRangeTypeMapping rangeTypeMapping:
            {
                var defaultInstance = GaussDBRangeTypeMapping.Default;

                var GaussDBDbTypeDifferent = rangeTypeMapping.GaussDBDbType != defaultInstance.GaussDBDbType;
                var subtypeTypeMappingIsDifferent = rangeTypeMapping.SubtypeMapping != defaultInstance.SubtypeMapping;

                if (GaussDBDbTypeDifferent || subtypeTypeMappingIsDifferent)
                {
                    mainBuilder.AppendLine(";");

                    mainBuilder.AppendLine(
                        $"{parameters.TargetName}.TypeMapping = ((GaussDBRangeTypeMapping){parameters.TargetName}.TypeMapping).Clone(")
                        .IncrementIndent();

                    mainBuilder
                        .Append("GaussDBDbType: ")
                        .Append(nameof(GaussDBTypes))
                        .Append(".")
                        .Append(nameof(GaussDBDbType))
                        .Append(".")
                        .Append(rangeTypeMapping.GaussDBDbType.ToString())
                        .AppendLine(",");

                    mainBuilder.Append("subtypeTypeMapping: ");

                    Create(rangeTypeMapping.SubtypeMapping, parameters);

                    mainBuilder
                        .Append(")")
                        .DecrementIndent();
                }

                break;
            }

        }

        return result;
    }

    /// <inheritdoc />
    public override void Generate(IModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;
            annotations.Remove(GaussDBAnnotationNames.DatabaseTemplate);
            annotations.Remove(GaussDBAnnotationNames.Tablespace);
            annotations.Remove(GaussDBAnnotationNames.CollationDefinitionPrefix);

#pragma warning disable CS0618
            annotations.Remove(GaussDBAnnotationNames.DefaultColumnCollation);
#pragma warning restore CS0618

            foreach (var annotationName in annotations.Keys.Where(
                         k =>
                             k.StartsWith(GaussDBAnnotationNames.PostgresExtensionPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.EnumPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.RangePrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(model, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IRelationalModel model, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;
            annotations.Remove(GaussDBAnnotationNames.DatabaseTemplate);
            annotations.Remove(GaussDBAnnotationNames.Tablespace);
            annotations.Remove(GaussDBAnnotationNames.CollationDefinitionPrefix);

#pragma warning disable CS0618
            annotations.Remove(GaussDBAnnotationNames.DefaultColumnCollation);
#pragma warning restore CS0618

            foreach (var annotationName in annotations.Keys.Where(
                         k =>
                             k.StartsWith(GaussDBAnnotationNames.PostgresExtensionPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.EnumPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.RangePrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(model, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IProperty property, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;
            annotations.Remove(GaussDBAnnotationNames.IdentityOptions);
            annotations.Remove(GaussDBAnnotationNames.TsVectorConfig);
            annotations.Remove(GaussDBAnnotationNames.TsVectorProperties);
            annotations.Remove(GaussDBAnnotationNames.CompressionMethod);

            if (!annotations.ContainsKey(GaussDBAnnotationNames.ValueGenerationStrategy))
            {
                annotations[GaussDBAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }

        base.Generate(property, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IColumn column, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(GaussDBAnnotationNames.IdentityOptions);
            annotations.Remove(GaussDBAnnotationNames.TsVectorConfig);
            annotations.Remove(GaussDBAnnotationNames.TsVectorProperties);
            annotations.Remove(GaussDBAnnotationNames.CompressionMethod);
        }

        base.Generate(column, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(GaussDBAnnotationNames.IndexMethod);
            annotations.Remove(GaussDBAnnotationNames.IndexOperators);
            annotations.Remove(GaussDBAnnotationNames.IndexSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexNullSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexInclude);
            annotations.Remove(GaussDBAnnotationNames.CreatedConcurrently);
            annotations.Remove(GaussDBAnnotationNames.NullsDistinct);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(index, parameters);
    }

    /// <inheritdoc />
    public override void Generate(ITableIndex index, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(GaussDBAnnotationNames.IndexMethod);
            annotations.Remove(GaussDBAnnotationNames.IndexOperators);
            annotations.Remove(GaussDBAnnotationNames.IndexSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexNullSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexInclude);
            annotations.Remove(GaussDBAnnotationNames.CreatedConcurrently);
            annotations.Remove(GaussDBAnnotationNames.NullsDistinct);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(index, parameters);
    }

    /// <inheritdoc />
    public override void Generate(IEntityType entityType, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(GaussDBAnnotationNames.UnloggedTable);
            annotations.Remove(CockroachDbAnnotationNames.InterleaveInParent);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(entityType, parameters);
    }

    /// <inheritdoc />
    public override void Generate(ITable table, CSharpRuntimeAnnotationCodeGeneratorParameters parameters)
    {
        if (!parameters.IsRuntime)
        {
            var annotations = parameters.Annotations;

            annotations.Remove(GaussDBAnnotationNames.UnloggedTable);
            annotations.Remove(CockroachDbAnnotationNames.InterleaveInParent);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }

        base.Generate(table, parameters);
    }
}
