using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using static GaussDB.EntityFrameworkCore.PostgreSQL.Utilities.Statics;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBMiscAggregateMethodTranslator : IAggregateMethodCallTranslator
{
    private static readonly MethodInfo StringJoin
        = typeof(string).GetRuntimeMethod(nameof(string.Join), [typeof(string), typeof(IEnumerable<string>)])!;

    private static readonly MethodInfo StringConcat
        = typeof(string).GetRuntimeMethod(nameof(string.Concat), [typeof(IEnumerable<string>)])!;

    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly IModel _model;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBMiscAggregateMethodTranslator(
        GaussDBSqlExpressionFactory sqlExpressionFactory,
        IRelationalTypeMappingSource typeMappingSource,
        IModel model)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _typeMappingSource = typeMappingSource;
        _model = model;
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
        // Docs: https://www.postgresql.org/docs/current/functions-aggregate.html

        if (source.Selector is not SqlExpression sqlExpression)
        {
            return null;
        }

        if (method == StringJoin || method == StringConcat)
        {
            // string_agg filters out nulls, but string.Join treats them as empty strings; coalesce unless we know we're aggregating over
            // a non-nullable column.
            if (sqlExpression is not ColumnExpression { IsNullable: false })
            {
                sqlExpression = _sqlExpressionFactory.Coalesce(
                    sqlExpression,
                    _sqlExpressionFactory.Constant(string.Empty, typeof(string)));
            }

            // string_agg returns null when there are no rows (or non-null values), but string.Join returns an empty string.
            return _sqlExpressionFactory.Coalesce(
                _sqlExpressionFactory.AggregateFunction(
                    "string_agg",
                    [
                        sqlExpression,
                        method == StringJoin ? arguments[0] : _sqlExpressionFactory.Constant(string.Empty, typeof(string))
                    ],
                    source,
                    nullable: true,
                    // string_agg can return nulls regardless of the nullability of its arguments, since if there's an aggregate predicate
                    // (string_agg(...) WHERE ...), it could cause there to be no elements, in which case string_agg returns null.
                    argumentsPropagateNullability: FalseArrays[2],
                    typeof(string),
                    _typeMappingSource.FindMapping("text")), // Note that string_agg returns text even if its inputs are varchar(x)
                _sqlExpressionFactory.Constant(string.Empty, typeof(string)));
        }

        if (method.DeclaringType == typeof(GaussDBAggregateDbFunctionsExtensions))
        {
            switch (method.Name)
            {
                case nameof(GaussDBAggregateDbFunctionsExtensions.ArrayAgg):
                    return _sqlExpressionFactory.AggregateFunction(
                        "array_agg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: method.ReturnType,
                        typeMapping: sqlExpression.TypeMapping is null
                            ? null
                            : _typeMappingSource.FindMapping(method.ReturnType, _model, sqlExpression.TypeMapping));

                case nameof(GaussDBAggregateDbFunctionsExtensions.JsonAgg):
                    return _sqlExpressionFactory.AggregateFunction(
                        "json_agg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType, "json"));

                case nameof(GaussDBAggregateDbFunctionsExtensions.JsonbAgg):
                    return _sqlExpressionFactory.AggregateFunction(
                        "jsonb_agg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType, "jsonb"));

                case nameof(GaussDBAggregateDbFunctionsExtensions.Sum):
                    return _sqlExpressionFactory.AggregateFunction(
                        "sum",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: sqlExpression.Type,
                        sqlExpression.TypeMapping);

                case nameof(GaussDBAggregateDbFunctionsExtensions.Average):
                    return _sqlExpressionFactory.AggregateFunction(
                        "avg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: sqlExpression.Type,
                        sqlExpression.TypeMapping);

                case nameof(GaussDBAggregateDbFunctionsExtensions.JsonbObjectAgg):
                case nameof(GaussDBAggregateDbFunctionsExtensions.JsonObjectAgg):
                    var isJsonb = method.Name == nameof(GaussDBAggregateDbFunctionsExtensions.JsonbObjectAgg);

                    // These methods accept two enumerable (column) arguments; this is represented in LINQ as a projection from the grouping
                    // to a tuple of the two columns. Since we generally translate tuples to PostgresRowValueExpression, we take it apart
                    // here.
                    if (source.Selector is not PgRowValueExpression rowValueExpression)
                    {
                        return null;
                    }

                    var (keys, values) = (rowValueExpression.Values[0], rowValueExpression.Values[1]);

                    return _sqlExpressionFactory.AggregateFunction(
                        isJsonb ? "jsonb_object_agg" : "json_object_agg",
                        [keys, values],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[2],
                        returnType: method.ReturnType,
                        _typeMappingSource.FindMapping(method.ReturnType, isJsonb ? "jsonb" : "json"));
            }
        }

        if (method.DeclaringType == typeof(GaussDBRangeDbFunctionsExtensions))
        {
            switch (method.Name)
            {
                case nameof(GaussDBRangeDbFunctionsExtensions.RangeAgg):
                    var arrayClrType = sqlExpression.Type.MakeArrayType();

                    return _sqlExpressionFactory.AggregateFunction(
                        "range_agg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: arrayClrType,
                        _typeMappingSource.FindMapping(arrayClrType));

                case nameof(GaussDBRangeDbFunctionsExtensions.RangeIntersectAgg):
                    return _sqlExpressionFactory.AggregateFunction(
                        "range_intersect_agg",
                        [sqlExpression],
                        source,
                        nullable: true,
                        argumentsPropagateNullability: FalseArrays[1],
                        returnType: sqlExpression.Type,
                        sqlExpression.TypeMapping);
            }
        }

        return null;
    }
}
