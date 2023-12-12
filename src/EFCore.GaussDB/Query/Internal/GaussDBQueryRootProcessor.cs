using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBQueryRootProcessor : RelationalQueryRootProcessor
{
    private readonly bool _supportsUnnest;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBQueryRootProcessor(
        QueryTranslationPreprocessorDependencies dependencies,
        RelationalQueryTranslationPreprocessorDependencies relationalDependencies,
        QueryCompilationContext queryCompilationContext,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
        _supportsUnnest = !GaussDBSingletonOptions.UseRedshift;
    }

    /// <summary>
    ///     Converts a <see cref="ParameterExpression" /> to a <see cref="ParameterQueryRootExpression" />, to be later translated to
    ///     PostgreSQL <c>unnest</c> over an array parameter.
    /// </summary>
    /// <remarks>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </remarks>
    protected override bool ShouldConvertToParameterQueryRoot(ParameterExpression parameterExpression)
        => _supportsUnnest;
}
