using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GaussDB.EntityFrameworkCore.PostgreSQL.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBRowValueTranslator : IMethodCallTranslator
{
    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;

    private static readonly MethodInfo GreaterThan =
        typeof(GaussDBDbFunctionsExtensions).GetRuntimeMethod(
            nameof(GaussDBDbFunctionsExtensions.GreaterThan),
            [typeof(DbFunctions), typeof(ITuple), typeof(ITuple)])!;

    private static readonly MethodInfo LessThan =
        typeof(GaussDBDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(GaussDBDbFunctionsExtensions.LessThan));

    private static readonly MethodInfo GreaterThanOrEqual =
        typeof(GaussDBDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(GaussDBDbFunctionsExtensions.GreaterThanOrEqual));

    private static readonly MethodInfo LessThanOrEqual =
        typeof(GaussDBDbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(GaussDBDbFunctionsExtensions.LessThanOrEqual));

    private static readonly Dictionary<MethodInfo, ExpressionType> ComparisonMethods = new()
    {
        { GreaterThan, ExpressionType.GreaterThan },
        { LessThan, ExpressionType.LessThan },
        { GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual },
        { LessThanOrEqual, ExpressionType.LessThanOrEqual }
    };

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussDBRowValueTranslator" /> class.
    /// </summary>
    public GaussDBRowValueTranslator(GaussDBSqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <inheritdoc />
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(ValueType))] // For ValueTuple.Create
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // Translate ValueTuple.Create
        if (method.DeclaringType == typeof(ValueTuple) && method is { IsStatic: true, Name: nameof(ValueTuple.Create) })
        {
            return new PgRowValueExpression(arguments, method.ReturnType);
        }

        // Translate EF.Functions.GreaterThan and other comparisons
        if (method.DeclaringType != typeof(GaussDBDbFunctionsExtensions) || !ComparisonMethods.TryGetValue(method, out var expressionType))
        {
            return null;
        }

        var leftCount = arguments[1] is PgRowValueExpression leftRowValue
            ? leftRowValue.Values.Count
            : arguments[1] is SqlConstantExpression { Value : ITuple leftTuple }
                ? (int?)leftTuple.Length
                : null;

        var rightCount = arguments[2] is PgRowValueExpression rightRowValue
            ? rightRowValue.Values.Count
            : arguments[2] is SqlConstantExpression { Value : ITuple rightTuple }
                ? (int?)rightTuple.Length
                : null;

        if (leftCount is null || rightCount is null)
        {
            return null;
        }

        if (leftCount != rightCount)
        {
            throw new ArgumentException(GaussDBStrings.RowValueComparisonRequiresTuplesOfSameLength);
        }

        return _sqlExpressionFactory.MakeBinary(expressionType, arguments[1], arguments[2], typeMapping: null);
    }
}
