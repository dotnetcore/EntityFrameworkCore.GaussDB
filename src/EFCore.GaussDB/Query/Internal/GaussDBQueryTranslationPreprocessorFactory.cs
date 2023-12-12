using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBQueryTranslationPreprocessorFactory : IQueryTranslationPreprocessorFactory
{
    private readonly IGaussDBSingletonOptions _GaussDBSingletonOptions;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBQueryTranslationPreprocessorFactory(
        QueryTranslationPreprocessorDependencies dependencies,
        RelationalQueryTranslationPreprocessorDependencies relationalDependencies,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
    {
        Dependencies = dependencies;
        RelationalDependencies = relationalDependencies;
        _GaussDBSingletonOptions = GaussDBSingletonOptions;
    }

    /// <summary>
    ///     Dependencies for this service.
    /// </summary>
    protected virtual QueryTranslationPreprocessorDependencies Dependencies { get; }

    /// <summary>
    ///     Relational provider-specific dependencies for this service.
    /// </summary>
    protected virtual RelationalQueryTranslationPreprocessorDependencies RelationalDependencies { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new GaussDBQueryTranslationPreprocessor(Dependencies, RelationalDependencies, _GaussDBSingletonOptions, queryCompilationContext);
}
