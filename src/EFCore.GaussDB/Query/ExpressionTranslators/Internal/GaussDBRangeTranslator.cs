using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Expressions;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using static GaussDB.EntityFrameworkCore.PostgreSQL.Utilities.Statics;
using ExpressionExtensions = Microsoft.EntityFrameworkCore.Query.ExpressionExtensions;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBRangeTranslator : IMethodCallTranslator, IMemberTranslator
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;
    private readonly IModel _model;
    private readonly bool _supportsMultiranges;

    private static readonly MethodInfo EnumerableAnyWithoutPredicate =
        typeof(Enumerable).GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(mi => mi.Name == nameof(Enumerable.Any) && mi.GetParameters().Length == 1);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBRangeTranslator(
        IRelationalTypeMappingSource typeMappingSource,
        GaussDBSqlExpressionFactory GaussDBSqlExpressionFactory,
        IModel model,
        bool supportsMultiranges)
    {
        _typeMappingSource = typeMappingSource;
        _sqlExpressionFactory = GaussDBSqlExpressionFactory;
        _model = model;
        _supportsMultiranges = supportsMultiranges;
    }

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // Any() over multirange -> NOT isempty(). GaussDBRange<T> has IsEmpty which is translated below.
        if (_supportsMultiranges
            && method.IsGenericMethod
            && method.GetGenericMethodDefinition() == EnumerableAnyWithoutPredicate
            && arguments[0].IsMultirange())
        {
            return _sqlExpressionFactory.Not(
                _sqlExpressionFactory.Function(
                    "isempty",
                    new[] { arguments[0] },
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[1],
                    typeof(bool)));
        }

        if (method.DeclaringType != typeof(GaussDBRangeDbFunctionsExtensions)
            && (method.DeclaringType != typeof(GaussDBMultirangeDbFunctionsExtensions) || !_supportsMultiranges))
        {
            return null;
        }

        if (method.Name == nameof(GaussDBRangeDbFunctionsExtensions.Merge))
        {
            if (method.DeclaringType == typeof(GaussDBRangeDbFunctionsExtensions))
            {
                var inferredMapping = ExpressionExtensions.InferTypeMapping(arguments[0], arguments[1]);

                return _sqlExpressionFactory.Function(
                    "range_merge",
                    new[]
                    {
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[0], inferredMapping),
                        _sqlExpressionFactory.ApplyTypeMapping(arguments[1], inferredMapping)
                    },
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[2],
                    method.ReturnType,
                    inferredMapping);
            }

            if (method.DeclaringType == typeof(GaussDBMultirangeDbFunctionsExtensions))
            {
                var returnTypeMapping = arguments[0].TypeMapping is GaussDBMultirangeTypeMapping multirangeTypeMapping
                    ? multirangeTypeMapping.RangeMapping
                    : null;

                return _sqlExpressionFactory.Function(
                    "range_merge",
                    new[] { arguments[0] },
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[1],
                    method.ReturnType,
                    returnTypeMapping);
            }
        }

        return method.Name switch
        {
            nameof(GaussDBRangeDbFunctionsExtensions.Contains)
                => _sqlExpressionFactory.Contains(arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.ContainedBy)
                => _sqlExpressionFactory.ContainedBy(arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.Overlaps)
                => _sqlExpressionFactory.Overlaps(arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.IsStrictlyLeftOf)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeIsStrictlyLeftOf, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.IsStrictlyRightOf)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeIsStrictlyRightOf, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.DoesNotExtendRightOf)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeDoesNotExtendRightOf, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.DoesNotExtendLeftOf)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeDoesNotExtendLeftOf, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.IsAdjacentTo)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeIsAdjacentTo, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.Union)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeUnion, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.Intersect)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeIntersect, arguments[0], arguments[1]),
            nameof(GaussDBRangeDbFunctionsExtensions.Except)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.RangeExcept, arguments[0], arguments[1]),

            _ => null
        };
    }

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MemberInfo member,
        Type returnType,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        var type = member.DeclaringType;
        if (type is null || !type.IsGenericType || type.GetGenericTypeDefinition() != typeof(GaussDBRange<>))
        {
            return null;
        }

        if (member.Name is nameof(GaussDBRange<int>.LowerBound) or nameof(GaussDBRange<int>.UpperBound))
        {
            var typeMapping = instance!.TypeMapping is GaussDBRangeTypeMapping rangeMapping
                ? rangeMapping.SubtypeMapping
                : _typeMappingSource.FindMapping(returnType, _model);

            return _sqlExpressionFactory.Function(
                member.Name == nameof(GaussDBRange<int>.LowerBound) ? "lower" : "upper",
                new[] { instance },
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                returnType,
                typeMapping);
        }

        return member.Name switch
        {
            nameof(GaussDBRange<int>.IsEmpty) => SingleArgBoolFunction("isempty", instance!),
            nameof(GaussDBRange<int>.LowerBoundIsInclusive) => SingleArgBoolFunction("lower_inc", instance!),
            nameof(GaussDBRange<int>.UpperBoundIsInclusive) => SingleArgBoolFunction("upper_inc", instance!),
            nameof(GaussDBRange<int>.LowerBoundInfinite) => SingleArgBoolFunction("lower_inf", instance!),
            nameof(GaussDBRange<int>.UpperBoundInfinite) => SingleArgBoolFunction("upper_inf", instance!),

            _ => null
        };

        SqlFunctionExpression SingleArgBoolFunction(string name, SqlExpression argument)
            => _sqlExpressionFactory.Function(
                name,
                new[] { argument },
                nullable: true,
                argumentsPropagateNullability: TrueArrays[1],
                typeof(bool));
    }
}
