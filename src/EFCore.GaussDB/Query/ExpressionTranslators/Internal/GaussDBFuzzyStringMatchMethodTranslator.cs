namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBFuzzyStringMatchMethodTranslator : IMethodCallTranslator
{
    private static readonly Dictionary<MethodInfo, string> Functions = new()
    {
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchSoundex), typeof(DbFunctions), typeof(string))]
            = "soundex",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDifference), typeof(DbFunctions), typeof(string), typeof(string))]
            = "difference",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshtein), typeof(DbFunctions), typeof(string), typeof(string))]
            = "levenshtein",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshtein), typeof(DbFunctions), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int))]
            = "levenshtein",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshteinLessEqual), typeof(DbFunctions), typeof(string), typeof(string), typeof(int))]
            = "levenshtein_less_equal",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchLevenshteinLessEqual), typeof(DbFunctions), typeof(string), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int))]
            = "levenshtein_less_equal",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchMetaphone), typeof(DbFunctions), typeof(string), typeof(int))]
            = "metaphone",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDoubleMetaphone), typeof(DbFunctions), typeof(string))]
            = "dmetaphone",
        [GetRuntimeMethod(nameof(GaussDBFuzzyStringMatchDbFunctionsExtensions.FuzzyStringMatchDoubleMetaphoneAlt), typeof(DbFunctions), typeof(string))]
            = "dmetaphone_alt"
    };

    private static MethodInfo GetRuntimeMethod(string name, params Type[] parameters)
        => typeof(GaussDBFuzzyStringMatchDbFunctionsExtensions).GetRuntimeMethod(name, parameters)!;

    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;

    private static readonly bool[][] TrueArrays =
    [
        [],
        [true],
        [true, true],
        [true, true, true],
        [true, true, true, true],
        [true, true, true, true, true],
        [true, true, true, true, true, true]
    ];

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBFuzzyStringMatchMethodTranslator(GaussDBSqlExpressionFactory sqlExpressionFactory)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        => Functions.TryGetValue(method, out var function)
            ? _sqlExpressionFactory.Function(
                function,
                arguments.Skip(1),
                nullable: true,
                argumentsPropagateNullability: TrueArrays[arguments.Count - 1],
                method.ReturnType)
            : null;
}
