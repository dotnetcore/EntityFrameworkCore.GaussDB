using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBAnnotationProvider : RelationalAnnotationProvider
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBAnnotationProvider(RelationalAnnotationProviderDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IEnumerable<IAnnotation> For(ITable table, bool designTime)
    {
        if (!designTime)
        {
            yield break;
        }

        // Model validation ensures that these facets are the same on all mapped entity types
        var entityType = (IEntityType)table.EntityTypeMappings.First().TypeBase;

        if (entityType.GetIsUnlogged())
        {
            yield return new Annotation(GaussDBAnnotationNames.UnloggedTable, entityType.GetIsUnlogged());
        }

        if (entityType[CockroachDbAnnotationNames.InterleaveInParent] is not null)
        {
            yield return new Annotation(
                CockroachDbAnnotationNames.InterleaveInParent, entityType[CockroachDbAnnotationNames.InterleaveInParent]);
        }

        foreach (var storageParamAnnotation in entityType.GetAnnotations()
                     .Where(a => a.Name.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
        {
            yield return storageParamAnnotation;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
    {
        if (!designTime)
        {
            yield break;
        }

        var table = StoreObjectIdentifier.Table(column.Table.Name, column.Table.Schema);
        var valueGeneratedProperty = column.PropertyMappings.Where(
                m => (m.TableMapping.IsSharedTablePrincipal ?? true)
                    && m.TableMapping.TypeBase == m.Property.DeclaringType)
            .Select(m => m.Property)
            .FirstOrDefault(
                p => p.GetValueGenerationStrategy(table) switch
                {
                    GaussDBValueGenerationStrategy.IdentityByDefaultColumn => true,
                    GaussDBValueGenerationStrategy.IdentityAlwaysColumn => true,
                    GaussDBValueGenerationStrategy.SerialColumn => true,
                    _ => false
                });

        if (valueGeneratedProperty is not null)
        {
            var valueGenerationStrategy = valueGeneratedProperty.GetValueGenerationStrategy();
            yield return new Annotation(GaussDBAnnotationNames.ValueGenerationStrategy, valueGenerationStrategy);

            if (valueGenerationStrategy is GaussDBValueGenerationStrategy.IdentityByDefaultColumn
                or GaussDBValueGenerationStrategy.IdentityAlwaysColumn)
            {
                if (valueGeneratedProperty[GaussDBAnnotationNames.IdentityOptions] is string identityOptions)
                {
                    yield return new Annotation(GaussDBAnnotationNames.IdentityOptions, identityOptions);
                }
            }
        }

        if (column.PropertyMappings.Select(m => m.Property.GetTsVectorConfig())
                .FirstOrDefault(c => c is not null) is { } tsVectorConfig)
        {
            yield return new Annotation(GaussDBAnnotationNames.TsVectorConfig, tsVectorConfig);
        }

        valueGeneratedProperty = column.PropertyMappings.Select(m => m.Property)
            .FirstOrDefault(p => p.GetTsVectorProperties() is not null);
        if (valueGeneratedProperty is not null)
        {
            var tableIdentifier = StoreObjectIdentifier.Table(column.Table.Name, column.Table.Schema);

            yield return new Annotation(
                GaussDBAnnotationNames.TsVectorProperties,
                valueGeneratedProperty.GetTsVectorProperties()!
                    .Select(p2 => valueGeneratedProperty.DeclaringType.FindProperty(p2)!.GetColumnName(tableIdentifier))
                    .ToArray());
        }

        // JSON columns have no property mappings so all annotations that rely on property mappings should be skipped for them
        if (column is not JsonColumn
            && column.PropertyMappings.FirstOrDefault()?.Property.GetCompressionMethod() is { } compressionMethod)
        {
            // Model validation ensures that these facets are the same on all mapped properties
            yield return new Annotation(GaussDBAnnotationNames.CompressionMethod, compressionMethod);
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IEnumerable<IAnnotation> For(ITableIndex index, bool designTime)
    {
        if (!designTime)
        {
            yield break;
        }

        // Model validation ensures that these facets are the same on all mapped indexes
        var modelIndex = index.MappedIndexes.First();

        if (modelIndex.GetCollation() is { } collation)
        {
            yield return new Annotation(RelationalAnnotationNames.Collation, collation);
        }

        if (modelIndex.GetMethod() is { } method)
        {
            yield return new Annotation(GaussDBAnnotationNames.IndexMethod, method);
        }

        if (modelIndex.GetOperators() is { } operators)
        {
            yield return new Annotation(GaussDBAnnotationNames.IndexOperators, operators);
        }

        if (modelIndex.GetNullSortOrder() is { } nullSortOrder)
        {
            yield return new Annotation(GaussDBAnnotationNames.IndexNullSortOrder, nullSortOrder);
        }

        if (modelIndex.GetTsVectorConfig() is { } configName)
        {
            yield return new Annotation(GaussDBAnnotationNames.TsVectorConfig, configName);
        }

        if (modelIndex.GetIncludeProperties() is { } includeProperties)
        {
            var tableIdentifier = StoreObjectIdentifier.Table(index.Table.Name, index.Table.Schema);

            yield return new Annotation(
                GaussDBAnnotationNames.IndexInclude,
                includeProperties
                    .Select(p => modelIndex.DeclaringEntityType.FindProperty(p)!.GetColumnName(tableIdentifier))
                    .ToArray());
        }

        if (modelIndex.IsCreatedConcurrently() is { } isCreatedConcurrently)
        {
            yield return new Annotation(GaussDBAnnotationNames.CreatedConcurrently, isCreatedConcurrently);
        }

        if (modelIndex.GetAreNullsDistinct() is { } nullsDistinct)
        {
            yield return new Annotation(GaussDBAnnotationNames.NullsDistinct, nullsDistinct);
        }

        foreach (var storageParamAnnotation in modelIndex.GetAnnotations()
                     .Where(a => a.Name.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
        {
            yield return storageParamAnnotation;
        }

        // Support legacy annotation for index ordering
        if (modelIndex[GaussDBAnnotationNames.IndexSortOrder] is IReadOnlyList<SortOrder> legacySortOrder)
        {
            yield return new Annotation(GaussDBAnnotationNames.IndexSortOrder, legacySortOrder);
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override IEnumerable<IAnnotation> For(IRelationalModel model, bool designTime)
    {
        if (!designTime)
        {
            return [];
        }

        return model.Model.GetAnnotations().Where(
            a =>
                a.Name.StartsWith(GaussDBAnnotationNames.PostgresExtensionPrefix, StringComparison.Ordinal)
                || a.Name.StartsWith(GaussDBAnnotationNames.EnumPrefix, StringComparison.Ordinal)
                || a.Name.StartsWith(GaussDBAnnotationNames.RangePrefix, StringComparison.Ordinal)
                || a.Name.StartsWith(GaussDBAnnotationNames.CollationDefinitionPrefix, StringComparison.Ordinal));
    }
}
