namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     Translates <see cref="T:DbFunctionsExtensions.Like" /> methods into PostgreSQL LIKE expressions.
/// </summary>
public class GaussDBLikeTranslator : IMethodCallTranslator
{
    private static readonly MethodInfo Like =
        typeof(DbFunctionsExtensions).GetRuntimeMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

    private static readonly MethodInfo LikeWithEscape =
        typeof(DbFunctionsExtensions).GetRuntimeMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string), typeof(string)])!;

    // ReSharper disable once InconsistentNaming
    private static readonly MethodInfo ILike =
        typeof(GaussDBDbFunctionsExtensions).GetRuntimeMethod(
            nameof(GaussDBDbFunctionsExtensions.ILike),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

    // ReSharper disable once InconsistentNaming
    private static readonly MethodInfo ILikeWithEscape =
        typeof(GaussDBDbFunctionsExtensions).GetRuntimeMethod(
            nameof(GaussDBDbFunctionsExtensions.ILike),
            [typeof(DbFunctions), typeof(string), typeof(string), typeof(string)])!;

    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GaussDBMathTranslator" /> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">The SQL expression factory to use when generating expressions..</param>
    public GaussDBLikeTranslator(GaussDBSqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method == LikeWithEscape)
        {
            return _sqlExpressionFactory.Like(arguments[1], arguments[2], arguments[3]);
        }

        if (method == ILikeWithEscape)
        {
            return _sqlExpressionFactory.ILike(arguments[1], arguments[2], arguments[3]);
        }

        bool sensitive;
        if (method == Like)
        {
            sensitive = true;
        }
        else if (method == ILike)
        {
            sensitive = false;
        }
        else
        {
            return null;
        }

        // PostgreSQL has backslash as the default LIKE escape character, but EF Core expects
        // no escape character unless explicitly requested (https://github.com/aspnet/EntityFramework/issues/8696).

        // If we have a constant expression, we check that there are no backslashes in order to render with
        // an ESCAPE clause (better SQL). If we have a constant expression with backslashes or a non-constant
        // expression, we render an ESCAPE clause to disable backslash escaping.

        var (match, pattern) = (arguments[1], arguments[2]);

        if (pattern is SqlConstantExpression { Value: string patternValue }
            && !patternValue.Contains('\\'))
        {
            return sensitive
                ? _sqlExpressionFactory.Like(match, pattern)
                : _sqlExpressionFactory.ILike(match, pattern);
        }

        return sensitive
            ? _sqlExpressionFactory.Like(match, pattern, _sqlExpressionFactory.Constant(string.Empty))
            : _sqlExpressionFactory.ILike(match, pattern, _sqlExpressionFactory.Constant(string.Empty));
    }
}
