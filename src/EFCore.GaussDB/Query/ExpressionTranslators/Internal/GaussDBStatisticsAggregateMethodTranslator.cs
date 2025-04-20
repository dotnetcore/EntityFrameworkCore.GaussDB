using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using static GaussDB.EntityFrameworkCore.PostgreSQL.Utilities.Statics;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBStatisticsAggregateMethodTranslator : IAggregateMethodCallTranslator
{
    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;
    private readonly RelationalTypeMapping _doubleTypeMapping, _longTypeMapping;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBStatisticsAggregateMethodTranslator(
        GaussDBSqlExpressionFactory sqlExpressionFactory,
        IRelationalTypeMappingSource typeMappingSource)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _doubleTypeMapping = typeMappingSource.FindMapping(typeof(double))!;
        _longTypeMapping = typeMappingSource.FindMapping(typeof(long))!;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual SqlExpression? Translate(
        MethodInfo method,
        EnumerableExpression source,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // Docs: https://www.postgresql.org/docs/current/functions-aggregate.html#FUNCTIONS-AGGREGATE-STATISTICS-TABLE

        if (method.DeclaringType != typeof(GaussDBAggregateDbFunctionsExtensions)
            || source.Selector is not SqlExpression sqlExpression)
        {
            return null;
        }

        // These four functions are simple and take a single enumerable argument
        var functionName = method.Name switch
        {
            nameof(GaussDBAggregateDbFunctionsExtensions.StandardDeviationSample) => "stddev_samp",
            nameof(GaussDBAggregateDbFunctionsExtensions.StandardDeviationPopulation) => "stddev_pop",
            nameof(GaussDBAggregateDbFunctionsExtensions.VarianceSample) => "var_samp",
            nameof(GaussDBAggregateDbFunctionsExtensions.VariancePopulation) => "var_pop",
            _ => null
        };

        if (functionName is not null)
        {
            return _sqlExpressionFactory.AggregateFunction(
                functionName,
                [sqlExpression],
                source,
                nullable: true,
                argumentsPropagateNullability: FalseArrays[1],
                typeof(double),
                _doubleTypeMapping);
        }

        functionName = method.Name switch
        {
            nameof(GaussDBAggregateDbFunctionsExtensions.Correlation) => "corr",
            nameof(GaussDBAggregateDbFunctionsExtensions.CovariancePopulation) => "covar_pop",
            nameof(GaussDBAggregateDbFunctionsExtensions.CovarianceSample) => "covar_samp",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrAverageX) => "regr_avgx",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrAverageY) => "regr_avgy",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrCount) => "regr_count",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrIntercept) => "regr_intercept",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrR2) => "regr_r2",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrSlope) => "regr_slope",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrSXX) => "regr_sxx",
            nameof(GaussDBAggregateDbFunctionsExtensions.RegrSXY) => "regr_sxy",
            _ => null
        };

        if (functionName is not null)
        {
            // These methods accept two enumerable (column) arguments; this is represented in LINQ as a projection from the grouping
            // to a tuple of the two columns. Since we generally translate tuples to PostgresRowValueExpression, we take it apart here.
            if (source.Selector is not PgRowValueExpression rowValueExpression)
            {
                return null;
            }

            var (y, x) = (rowValueExpression.Values[0], rowValueExpression.Values[1]);

            return method.Name == nameof(GaussDBAggregateDbFunctionsExtensions.RegrCount)
                ? _sqlExpressionFactory.AggregateFunction(
                    functionName,
                    [y, x],
                    source,
                    nullable: true,
                    argumentsPropagateNullability: FalseArrays[2],
                    typeof(long),
                    _longTypeMapping)
                : _sqlExpressionFactory.AggregateFunction(
                    functionName,
                    [y, x],
                    source,
                    nullable: true,
                    argumentsPropagateNullability: FalseArrays[2],
                    typeof(double),
                    _doubleTypeMapping);
        }

        return null;
    }
}
