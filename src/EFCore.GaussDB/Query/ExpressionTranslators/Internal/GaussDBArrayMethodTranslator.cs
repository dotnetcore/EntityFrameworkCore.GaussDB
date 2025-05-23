using GaussDB.EntityFrameworkCore.PostgreSQL.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using static GaussDB.EntityFrameworkCore.PostgreSQL.Utilities.Statics;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     Translates method and property calls on arrays/lists into their corresponding PostgreSQL operations.
/// </summary>
/// <remarks>
///     https://www.postgresql.org/docs/current/static/functions-array.html
/// </remarks>
public class GaussDBArrayMethodTranslator : IMethodCallTranslator
{
    #region Methods

    // ReSharper disable InconsistentNaming
    private static readonly MethodInfo Array_IndexOf1 =
        typeof(Array).GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(m => m is { Name: nameof(Array.IndexOf), IsGenericMethod: true } && m.GetParameters().Length == 2);

    private static readonly MethodInfo Array_IndexOf2 =
        typeof(Array).GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(m => m is { Name: nameof(Array.IndexOf), IsGenericMethod: true } && m.GetParameters().Length == 3);

    private static readonly MethodInfo Enumerable_ElementAt =
        typeof(Enumerable).GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(
                m => m.Name == nameof(Enumerable.ElementAt)
                    && m.GetParameters().Length == 2
                    && m.GetParameters()[1].ParameterType == typeof(int));

    private static readonly MethodInfo Enumerable_SequenceEqual =
        typeof(Enumerable).GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(m => m.Name == nameof(Enumerable.SequenceEqual) && m.GetParameters().Length == 2);
    // ReSharper restore InconsistentNaming

    #endregion Methods

    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;
    private readonly GaussDBJsonPocoTranslator _jsonPocoTranslator;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBArrayMethodTranslator(GaussDBSqlExpressionFactory sqlExpressionFactory, GaussDBJsonPocoTranslator jsonPocoTranslator)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _jsonPocoTranslator = jsonPocoTranslator;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        // During preprocessing, ArrayIndex and List[] get normalized to ElementAt; so we handle indexing into array/list here
        if (method.IsClosedFormOf(Enumerable_ElementAt))
        {
            // Indexing over bytea is special, we have to use function rather than subscript
            if (arguments[0].TypeMapping is GaussDBByteArrayTypeMapping)
            {
                return _sqlExpressionFactory.Function(
                    "get_byte",
                    new[] { arguments[0], arguments[1] },
                    nullable: true,
                    argumentsPropagateNullability: TrueArrays[2],
                    typeof(byte));
            }

            // Try translating indexing inside JSON column
            // Note that Length over PG arrays (not within JSON) gets translated by QueryableMethodTranslatingEV, since arrays are primitive
            // collections
            return _jsonPocoTranslator.TranslateMemberAccess(arguments[0], arguments[1], method.ReturnType);
        }

        if (method.IsClosedFormOf(Enumerable_SequenceEqual)
            && arguments[0].Type.IsArrayOrGenericList()
            && !IsMappedToNonArray(arguments[0])
            && arguments[1].Type.IsArrayOrGenericList()
            && !IsMappedToNonArray(arguments[1]))
        {
            return _sqlExpressionFactory.Equal(arguments[0], arguments[1]);
        }

        // Translate instance methods on List
        if (instance is not null && instance.Type.IsGenericList() && !IsMappedToNonArray(instance))
        {
            return TranslateCommon(instance, arguments);
        }

        // Translate extension methods over array or List
        if (instance is null && arguments.Count > 0 && arguments[0].Type.IsArrayOrGenericList() && !IsMappedToNonArray(arguments[0]))
        {
            return TranslateCommon(arguments[0], arguments.Slice(1));
        }

        return null;

        // The array/list CLR type may be mapped to a non-array database type (e.g. byte[] to bytea, or just
        // value converters) - we don't want to translate for those cases.
        static bool IsMappedToNonArray(SqlExpression arrayOrList)
            => arrayOrList.TypeMapping is { } and not (GaussDBArrayTypeMapping or GaussDBJsonTypeMapping);

#pragma warning disable CS8321
        SqlExpression? TranslateCommon(SqlExpression arrayOrList, IReadOnlyList<SqlExpression> arguments)
#pragma warning restore CS8321
        {
            if (method.IsClosedFormOf(Array_IndexOf1)
                || method.Name == nameof(List<int>.IndexOf)
                && method.DeclaringType.IsGenericList()
                && method.GetParameters().Length == 1)
            {
                var (item, array) = _sqlExpressionFactory.ApplyTypeMappingsOnItemAndArray(arguments[0], arrayOrList);

                return _sqlExpressionFactory.Coalesce(
                    _sqlExpressionFactory.Subtract(
                        _sqlExpressionFactory.Function(
                            "array_next",
                            new[] { array, item },
                            nullable: true,
                            TrueArrays[2],
                            arrayOrList.Type),
                        _sqlExpressionFactory.Constant(1)),
                    _sqlExpressionFactory.Constant(-1));
            }

            if (method.IsClosedFormOf(Array_IndexOf2)
                || method.Name == nameof(List<int>.IndexOf)
                && method.DeclaringType.IsGenericList()
                && method.GetParameters().Length == 2)
            {
                var (item, array) = _sqlExpressionFactory.ApplyTypeMappingsOnItemAndArray(arguments[0], arrayOrList);
                var startIndex = _sqlExpressionFactory.GenerateOneBasedIndexExpression(arguments[1]);

                return _sqlExpressionFactory.Coalesce(
                    _sqlExpressionFactory.Subtract(
                        _sqlExpressionFactory.Function(
                            "array_next",
                            new[] { array, item, startIndex },
                            nullable: true,
                            TrueArrays[3],
                            arrayOrList.Type),
                        _sqlExpressionFactory.Constant(1)),
                    _sqlExpressionFactory.Constant(-1));
            }

            return null;
        }
    }
}
