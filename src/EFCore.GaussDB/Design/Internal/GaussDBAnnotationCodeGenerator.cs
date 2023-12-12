using System.Diagnostics.CodeAnalysis;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Design.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBAnnotationCodeGenerator : AnnotationCodeGenerator
{
    #region MethodInfos

    private static readonly MethodInfo ModelHasPostgresExtensionMethodInfo1
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresExtension), typeof(ModelBuilder), typeof(string));

    private static readonly MethodInfo ModelHasPostgresExtensionMethodInfo2
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresExtension), typeof(ModelBuilder), typeof(string), typeof(string),
            typeof(string));

    private static readonly MethodInfo ModelHasPostgresEnumMethodInfo1
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresEnum), typeof(ModelBuilder), typeof(string), typeof(string[]));

    private static readonly MethodInfo ModelHasPostgresEnumMethodInfo2
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresEnum), typeof(ModelBuilder), typeof(string), typeof(string), typeof(string[]));

    private static readonly MethodInfo ModelHasPostgresRangeMethodInfo1
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresRange), typeof(ModelBuilder), typeof(string), typeof(string));

    private static readonly MethodInfo ModelHasPostgresRangeMethodInfo2
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.HasPostgresRange), typeof(ModelBuilder), typeof(string), typeof(string), typeof(string),
            typeof(string), typeof(string), typeof(string), typeof(string));

    private static readonly MethodInfo ModelUseSerialColumnsMethodInfo
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.UseSerialColumns), typeof(ModelBuilder));

    private static readonly MethodInfo ModelUseIdentityAlwaysColumnsMethodInfo
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.UseIdentityAlwaysColumns), typeof(ModelBuilder));

    private static readonly MethodInfo ModelUseIdentityByDefaultColumnsMethodInfo
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.UseIdentityByDefaultColumns), typeof(ModelBuilder));

    private static readonly MethodInfo ModelUseHiLoMethodInfo
        = typeof(GaussDBModelBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.UseHiLo), typeof(ModelBuilder), typeof(string), typeof(string));

    private static readonly MethodInfo ModelHasAnnotationMethodInfo
        = typeof(ModelBuilder).GetRequiredRuntimeMethod(
            nameof(ModelBuilder.HasAnnotation), typeof(string), typeof(object));

    private static readonly MethodInfo ModelUseKeySequencesMethodInfo
        = typeof(GaussDBModelBuilderExtensions).GetRuntimeMethod(
            nameof(GaussDBModelBuilderExtensions.UseKeySequences), new[] { typeof(ModelBuilder), typeof(string), typeof(string) })!;

    private static readonly MethodInfo EntityTypeIsUnloggedMethodInfo
        = typeof(GaussDBEntityTypeBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBEntityTypeBuilderExtensions.IsUnlogged), typeof(EntityTypeBuilder), typeof(bool));

    private static readonly MethodInfo PropertyUseSerialColumnMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.UseSerialColumn), typeof(PropertyBuilder));

    private static readonly MethodInfo PropertyUseIdentityAlwaysColumnMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.UseIdentityAlwaysColumn), typeof(PropertyBuilder));

    private static readonly MethodInfo PropertyUseIdentityByDefaultColumnMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.UseIdentityByDefaultColumn), typeof(PropertyBuilder));

    private static readonly MethodInfo PropertyUseHiLoMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.UseHiLo), typeof(PropertyBuilder), typeof(string), typeof(string));

    private static readonly MethodInfo PropertyHasIdentityOptionsMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.HasIdentityOptions), typeof(PropertyBuilder), typeof(long?), typeof(long?),
            typeof(long?), typeof(long?), typeof(bool?), typeof(long?));

    private static readonly MethodInfo PropertyUseSequenceMethodInfo
        = typeof(GaussDBPropertyBuilderExtensions).GetRuntimeMethod(
            nameof(GaussDBPropertyBuilderExtensions.UseSequence), new[] { typeof(PropertyBuilder), typeof(string), typeof(string) })!;

    private static readonly MethodInfo IndexUseCollationMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.UseCollation), typeof(IndexBuilder), typeof(string[]));

    private static readonly MethodInfo IndexHasMethodMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.HasMethod), typeof(IndexBuilder), typeof(string));

    private static readonly MethodInfo IndexHasOperatorsMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.HasOperators), typeof(IndexBuilder), typeof(string[]));

    private static readonly MethodInfo IndexHasSortOrderMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.HasSortOrder), typeof(IndexBuilder), typeof(SortOrder[]));

    private static readonly MethodInfo IndexHasNullSortOrderMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.HasNullSortOrder), typeof(IndexBuilder), typeof(NullSortOrder[]));

    private static readonly MethodInfo IndexIncludePropertiesMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.IncludeProperties), typeof(IndexBuilder), typeof(string[]));

    private static readonly MethodInfo IndexAreNullsDistinctMethodInfo
        = typeof(GaussDBIndexBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBIndexBuilderExtensions.AreNullsDistinct), typeof(IndexBuilder), typeof(bool));

    #endregion MethodInfos

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBAnnotationCodeGenerator(AnnotationCodeGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsHandledByConvention(IModel model, IAnnotation annotation)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(annotation, nameof(annotation));

        if (annotation.Name == RelationalAnnotationNames.DefaultSchema
            && (string?)annotation.Value == "public")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsHandledByConvention(IIndex index, IAnnotation annotation)
    {
        Check.NotNull(index, nameof(index));
        Check.NotNull(annotation, nameof(annotation));

        if (annotation.Name == GaussDBAnnotationNames.IndexMethod
            && (string?)annotation.Value == "btree")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override bool IsHandledByConvention(IProperty property, IAnnotation annotation)
    {
        Check.NotNull(property, nameof(property));
        Check.NotNull(annotation, nameof(annotation));

        // The default by-convention value generation strategy is serial in pre-10 PostgreSQL,
        // and IdentityByDefault otherwise.
        if (annotation.Name == GaussDBAnnotationNames.ValueGenerationStrategy)
        {
            // Note: both serial and identity-by-default columns are considered by-convention - we don't want
            // to assume that the PostgreSQL version of the scaffolded database necessarily determines the
            // version of the database that the scaffolded model will target. This makes life difficult for
            // models with mixed strategies but that's an edge case.
            return (GaussDBValueGenerationStrategy?)annotation.Value switch
            {
                GaussDBValueGenerationStrategy.SerialColumn => true,
                GaussDBValueGenerationStrategy.IdentityByDefaultColumn => true,
                _ => false
            };
        }

        return false;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
        IModel model,
        IDictionary<string, IAnnotation> annotations)
    {
        var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(model, annotations));

        if (GenerateValueGenerationStrategy(annotations, onModel: true) is { } valueGenerationStrategy)
        {
            fragments.Add(valueGenerationStrategy);
        }

        return fragments;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override MethodCallCodeFragment? GenerateFluentApi(IModel model, IAnnotation annotation)
    {
        Check.NotNull(model, nameof(model));
        Check.NotNull(annotation, nameof(annotation));

        if (annotation.Name.StartsWith(GaussDBAnnotationNames.PostgresExtensionPrefix, StringComparison.Ordinal))
        {
            var extension = new PostgresExtension(model, annotation.Name);

            return extension.Schema is "public" or null
                ? new MethodCallCodeFragment(ModelHasPostgresExtensionMethodInfo1, extension.Name)
                : new MethodCallCodeFragment(ModelHasPostgresExtensionMethodInfo2, extension.Schema, extension.Name);
        }

        if (annotation.Name.StartsWith(GaussDBAnnotationNames.EnumPrefix, StringComparison.Ordinal))
        {
            var enumTypeDef = new PostgresEnum(model, annotation.Name);

            return enumTypeDef.Schema is null
                ? new MethodCallCodeFragment(ModelHasPostgresEnumMethodInfo1, enumTypeDef.Name, enumTypeDef.Labels)
                : new MethodCallCodeFragment(ModelHasPostgresEnumMethodInfo2, enumTypeDef.Schema, enumTypeDef.Name, enumTypeDef.Labels);
        }

        if (annotation.Name.StartsWith(GaussDBAnnotationNames.RangePrefix, StringComparison.Ordinal))
        {
            var rangeTypeDef = new PostgresRange(model, annotation.Name);

            if (rangeTypeDef.Schema is null
                && rangeTypeDef.CanonicalFunction is null
                && rangeTypeDef.SubtypeOpClass is null
                && rangeTypeDef.Collation is null
                && rangeTypeDef.SubtypeDiff is null)
            {
                return new MethodCallCodeFragment(ModelHasPostgresRangeMethodInfo1, rangeTypeDef.Name, rangeTypeDef.Subtype);
            }

            return new MethodCallCodeFragment(
                ModelHasPostgresRangeMethodInfo2,
                rangeTypeDef.Schema,
                rangeTypeDef.Name,
                rangeTypeDef.Subtype,
                rangeTypeDef.CanonicalFunction,
                rangeTypeDef.SubtypeOpClass,
                rangeTypeDef.Collation,
                rangeTypeDef.SubtypeDiff);
        }

        return null;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override MethodCallCodeFragment? GenerateFluentApi(IEntityType entityType, IAnnotation annotation)
    {
        Check.NotNull(entityType, nameof(entityType));
        Check.NotNull(annotation, nameof(annotation));

        if (annotation.Name == GaussDBAnnotationNames.UnloggedTable)
        {
            return new MethodCallCodeFragment(EntityTypeIsUnloggedMethodInfo, annotation.Value);
        }

        return null;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IReadOnlyList<MethodCallCodeFragment> GenerateFluentApiCalls(
        IProperty property,
        IDictionary<string, IAnnotation> annotations)
    {
        var fragments = new List<MethodCallCodeFragment>(base.GenerateFluentApiCalls(property, annotations));

        if (GenerateValueGenerationStrategy(annotations, onModel: false) is { } valueGenerationStrategy)
        {
            fragments.Add(valueGenerationStrategy);
        }

        if (GenerateIdentityOptions(annotations) is { } identityOptionsFragment)
        {
            fragments.Add(identityOptionsFragment);
        }

        return fragments;
    }

    private MethodCallCodeFragment? GenerateValueGenerationStrategy(IDictionary<string, IAnnotation> annotations, bool onModel)
    {
        if (!TryGetAndRemove(annotations, GaussDBAnnotationNames.ValueGenerationStrategy, out GaussDBValueGenerationStrategy strategy))
        {
            return null;
        }

        switch (strategy)
        {
            case GaussDBValueGenerationStrategy.SerialColumn:
                return new MethodCallCodeFragment(onModel ? ModelUseSerialColumnsMethodInfo : PropertyUseSerialColumnMethodInfo);

            case GaussDBValueGenerationStrategy.IdentityAlwaysColumn:
                return new MethodCallCodeFragment(
                    onModel ? ModelUseIdentityAlwaysColumnsMethodInfo : PropertyUseIdentityAlwaysColumnMethodInfo);

            case GaussDBValueGenerationStrategy.IdentityByDefaultColumn:
                return new MethodCallCodeFragment(
                    onModel ? ModelUseIdentityByDefaultColumnsMethodInfo : PropertyUseIdentityByDefaultColumnMethodInfo);

            case GaussDBValueGenerationStrategy.SequenceHiLo:
            {
                var name = GetAndRemove<string>(GaussDBAnnotationNames.HiLoSequenceName)!;
                var schema = GetAndRemove<string>(GaussDBAnnotationNames.HiLoSequenceSchema);
                return new MethodCallCodeFragment(
                    onModel ? ModelUseHiLoMethodInfo : PropertyUseHiLoMethodInfo,
                    (name, schema) switch
                    {
                        (null, null) => Array.Empty<object>(),
                        (_, null) => new object[] { name },
                        _ => new object?[] { name!, schema }
                    });
            }

            case GaussDBValueGenerationStrategy.Sequence:
            {
                var nameOrSuffix = GetAndRemove<string>(
                    onModel ? GaussDBAnnotationNames.SequenceNameSuffix : GaussDBAnnotationNames.SequenceName);

                var schema = GetAndRemove<string>(GaussDBAnnotationNames.SequenceSchema);
                return new MethodCallCodeFragment(
                    onModel ? ModelUseKeySequencesMethodInfo : PropertyUseSequenceMethodInfo,
                    (name: nameOrSuffix, schema) switch
                    {
                        (null, null) => Array.Empty<object>(),
                        (_, null) => new object[] { nameOrSuffix },
                        _ => new object[] { nameOrSuffix!, schema }
                    });
            }
            case GaussDBValueGenerationStrategy.None:
                return new MethodCallCodeFragment(
                    ModelHasAnnotationMethodInfo, GaussDBAnnotationNames.ValueGenerationStrategy, GaussDBValueGenerationStrategy.None);

            default:
                throw new ArgumentOutOfRangeException(strategy.ToString());
        }

        T? GetAndRemove<T>(string annotationName)
            => TryGetAndRemove(annotations, annotationName, out T? annotationValue)
                ? annotationValue
                : default;
    }

    private MethodCallCodeFragment? GenerateIdentityOptions(IDictionary<string, IAnnotation> annotations)
    {
        if (!TryGetAndRemove(
                annotations, GaussDBAnnotationNames.IdentityOptions,
                out string? annotationValue))
        {
            return null;
        }

        var identityOptions = IdentitySequenceOptionsData.Deserialize(annotationValue);
        return new MethodCallCodeFragment(
            PropertyHasIdentityOptionsMethodInfo,
            identityOptions.StartValue,
            identityOptions.IncrementBy == 1 ? null : (long?)identityOptions.IncrementBy,
            identityOptions.MinValue,
            identityOptions.MaxValue,
            identityOptions.IsCyclic ? true : null,
            identityOptions.NumbersToCache == 1 ? null : (long?)identityOptions.NumbersToCache);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override MethodCallCodeFragment? GenerateFluentApi(IIndex index, IAnnotation annotation)
        => annotation.Name switch
        {
            RelationalAnnotationNames.Collation
                => new MethodCallCodeFragment(IndexUseCollationMethodInfo, annotation.Value),

            GaussDBAnnotationNames.IndexMethod
                => new MethodCallCodeFragment(IndexHasMethodMethodInfo, annotation.Value),
            GaussDBAnnotationNames.IndexOperators
                => new MethodCallCodeFragment(IndexHasOperatorsMethodInfo, annotation.Value),
            GaussDBAnnotationNames.IndexSortOrder
                => new MethodCallCodeFragment(IndexHasSortOrderMethodInfo, annotation.Value),
            GaussDBAnnotationNames.IndexNullSortOrder
                => new MethodCallCodeFragment(IndexHasNullSortOrderMethodInfo, annotation.Value),
            GaussDBAnnotationNames.IndexInclude
                => new MethodCallCodeFragment(IndexIncludePropertiesMethodInfo, annotation.Value),
            GaussDBAnnotationNames.NullsDistinct
                => new MethodCallCodeFragment(IndexAreNullsDistinctMethodInfo, annotation.Value),
            _ => null
        };

    private static bool TryGetAndRemove<T>(
        IDictionary<string, IAnnotation> annotations,
        string annotationName,
        [NotNullWhen(true)] out T? annotationValue)
    {
        if (annotations.TryGetValue(annotationName, out var annotation)
            && annotation.Value is not null)
        {
            annotations.Remove(annotationName);
            annotationValue = (T)annotation.Value;
            return true;
        }

        annotationValue = default;
        return false;
    }
}
