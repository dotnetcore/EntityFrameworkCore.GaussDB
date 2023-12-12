using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.Internal;

/// <summary>
///     The default factory for GaussDB-specific query SQL generators.
/// </summary>
public class GaussDBQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies _dependencies;
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly IGaussDBSingletonOptions _GaussDBSingletonOptions;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBQuerySqlGeneratorFactory(
        QuerySqlGeneratorDependencies dependencies,
        IRelationalTypeMappingSource typeMappingSource,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
    {
        _dependencies = dependencies;
        _typeMappingSource = typeMappingSource;
        _GaussDBSingletonOptions = GaussDBSingletonOptions;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual QuerySqlGenerator Create()
        => new GaussDBQuerySqlGenerator(
            _dependencies,
            _typeMappingSource,
            _GaussDBSingletonOptions.ReverseNullOrderingEnabled,
            _GaussDBSingletonOptions.PostgresVersion);
}
