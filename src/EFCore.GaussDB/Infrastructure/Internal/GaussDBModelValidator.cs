using GaussDB.EntityFrameworkCore.PostgreSQL.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBModelValidator : RelationalModelValidator
{
    /// <summary>
    ///     The backend version to target.
    /// </summary>
    private readonly Version _postgresVersion;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBModelValidator(
        ModelValidatorDependencies dependencies,
        RelationalModelValidatorDependencies relationalDependencies,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
        : base(dependencies, relationalDependencies)
    {
        _postgresVersion = GaussDBSingletonOptions.PostgresVersion;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override void Validate(IModel model, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.Validate(model, logger);

        ValidateIdentityVersionCompatibility(model);
        ValidateIndexIncludeProperties(model);
    }

    /// <summary>
    ///     Validates that identity columns are used only with PostgreSQL 10.0 or later.
    /// </summary>
    /// <param name="model">The model to validate.</param>
    protected virtual void ValidateIdentityVersionCompatibility(IModel model)
    {
        if (_postgresVersion.AtLeast(10))
        {
            return;
        }

        var strategy = model.GetValueGenerationStrategy();

        if (strategy is GaussDBValueGenerationStrategy.IdentityAlwaysColumn or GaussDBValueGenerationStrategy.IdentityByDefaultColumn)
        {
            throw new InvalidOperationException(
                $"'{strategy}' requires PostgreSQL 10.0 or later. "
                + "If you're using an older version, set PostgreSQL compatibility mode by calling "
                + $"'optionsBuilder.{nameof(GaussDBDbContextOptionsBuilder.SetPostgresVersion)}()' in your model's OnConfiguring. "
                + "See the docs for more info.");
        }

        foreach (var property in model.GetEntityTypes().SelectMany(e => e.GetProperties()))
        {
            var propertyStrategy = property.GetValueGenerationStrategy();

            if (propertyStrategy is GaussDBValueGenerationStrategy.IdentityAlwaysColumn
                or GaussDBValueGenerationStrategy.IdentityByDefaultColumn)
            {
                throw new InvalidOperationException(
                    $"{property.DeclaringType}.{property.Name}: '{propertyStrategy}' requires PostgreSQL 10.0 or later.");
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override void ValidateValueGeneration(
        IEntityType entityType,
        IKey key,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        if (entityType.GetTableName() != null
            && (string?)entityType[RelationalAnnotationNames.MappingStrategy] == RelationalAnnotationNames.TpcMappingStrategy)
        {
            foreach (var storeGeneratedProperty in key.Properties.Where(
                         p => (p.ValueGenerated & ValueGenerated.OnAdd) != 0
                             && p.GetValueGenerationStrategy() != GaussDBValueGenerationStrategy.Sequence))
            {
                logger.TpcStoreGeneratedIdentityWarning(storeGeneratedProperty);
            }
        }
    }

    /// <inheritdoc/>
    protected override void ValidateTypeMappings(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.ValidateTypeMappings(model, logger);

        foreach (var entityType in model.GetEntityTypes())
        {
            foreach (var property in entityType.GetFlattenedDeclaredProperties())
            {
                var strategy = property.GetValueGenerationStrategy();
                var propertyType = property.ClrType;

                switch (strategy)
                {
                    case GaussDBValueGenerationStrategy.None:
                        break;

                    case GaussDBValueGenerationStrategy.IdentityByDefaultColumn:
                    case GaussDBValueGenerationStrategy.IdentityAlwaysColumn:
                        if (!GaussDBPropertyExtensions.IsCompatibleWithValueGeneration(property))
                        {
                            throw new InvalidOperationException(
                                GaussDBStrings.IdentityBadType(
                                    property.Name, property.DeclaringType.DisplayName(), propertyType.ShortDisplayName()));
                        }

                        break;

                    case GaussDBValueGenerationStrategy.SequenceHiLo:
                    case GaussDBValueGenerationStrategy.Sequence:
                    case GaussDBValueGenerationStrategy.SerialColumn:
                        if (!GaussDBPropertyExtensions.IsCompatibleWithValueGeneration(property))
                        {
                            throw new InvalidOperationException(
                                GaussDBStrings.SequenceBadType(
                                    property.Name, property.DeclaringType.DisplayName(), propertyType.ShortDisplayName()));
                        }

                        break;

                    default:
                        throw new UnreachableException();
                }
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual void ValidateIndexIncludeProperties(IModel model)
    {
        foreach (var index in model.GetEntityTypes().SelectMany(t => t.GetDeclaredIndexes()))
        {
            var includeProperties = index.GetIncludeProperties();
            if (includeProperties?.Count > 0)
            {
                var notFound = includeProperties
                    .FirstOrDefault(i => index.DeclaringEntityType.FindProperty(i) is null);

                if (notFound is not null)
                {
                    throw new InvalidOperationException(
                        GaussDBStrings.IncludePropertyNotFound(index.DeclaringEntityType.DisplayName(), notFound));
                }

                var duplicate = includeProperties
                    .GroupBy(i => i)
                    .Where(g => g.Count() > 1)
                    .Select(y => y.Key)
                    .FirstOrDefault();

                if (duplicate is not null)
                {
                    throw new InvalidOperationException(
                        GaussDBStrings.IncludePropertyDuplicated(index.DeclaringEntityType.DisplayName(), duplicate));
                }

                var inIndex = includeProperties
                    .FirstOrDefault(i => index.Properties.Any(p => i == p.Name));

                if (inIndex is not null)
                {
                    throw new InvalidOperationException(
                        GaussDBStrings.IncludePropertyInIndex(index.DeclaringEntityType.DisplayName(), inIndex));
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void ValidateStoredProcedures(
        IModel model,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.ValidateStoredProcedures(model, logger);

        foreach (var entityType in model.GetEntityTypes())
        {
            if (entityType.GetDeleteStoredProcedure() is { } deleteStoredProcedure)
            {
                ValidateSproc(deleteStoredProcedure, logger);
            }

            if (entityType.GetInsertStoredProcedure() is { } insertStoredProcedure)
            {
                ValidateSproc(insertStoredProcedure, logger);
            }

            if (entityType.GetUpdateStoredProcedure() is { } updateStoredProcedure)
            {
                ValidateSproc(updateStoredProcedure, logger);
            }
        }

        static void ValidateSproc(IStoredProcedure sproc, IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
        {
            var entityType = sproc.EntityType;
            var storeObjectIdentifier = sproc.GetStoreIdentifier();

            if (sproc.ResultColumns.Any())
            {
                throw new InvalidOperationException(
                    GaussDBStrings.StoredProcedureResultColumnsNotSupported(
                        entityType.DisplayName(),
                        storeObjectIdentifier.DisplayName()));
            }

            if (sproc.IsRowsAffectedReturned)
            {
                throw new InvalidOperationException(
                    GaussDBStrings.StoredProcedureReturnValueNotSupported(
                        entityType.DisplayName(),
                        storeObjectIdentifier.DisplayName()));
            }
        }
    }

    /// <inheritdoc />
    protected override void ValidateCompatible(
        IProperty property,
        IProperty duplicateProperty,
        string columnName,
        in StoreObjectIdentifier storeObject,
        IDiagnosticsLogger<DbLoggerCategory.Model.Validation> logger)
    {
        base.ValidateCompatible(property, duplicateProperty, columnName, storeObject, logger);

        if (property.GetCompressionMethod(storeObject) != duplicateProperty.GetCompressionMethod(storeObject))
        {
            throw new InvalidOperationException(
                GaussDBStrings.DuplicateColumnCompressionMethodMismatch(
                    duplicateProperty.DeclaringType.DisplayName(),
                    duplicateProperty.Name,
                    property.DeclaringType.DisplayName(),
                    property.Name,
                    columnName,
                    storeObject.DisplayName()));
        }
    }
}
