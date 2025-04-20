using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Provides EF Core extension methods for GaussDB full-text search types.
/// </summary>
[SuppressMessage("ReSharper", "UnusedParameter.Global")]
public static class GaussDBFullTextSearchLinqExtensions
{
    /// <summary>
    ///     AND tsquerys together. Generates the "&amp;&amp;" operator.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery And(this GaussDBTsQuery query1, GaussDBTsQuery query2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(And)));

    /// <summary>
    ///     OR tsquerys together. Generates the "||" operator.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery Or(this GaussDBTsQuery query1, GaussDBTsQuery query2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(Or)));

    /// <summary>
    ///     Negate a tsquery. Generates the "!!" operator.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery ToNegative(this GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(ToNegative)));

    /// <summary>
    ///     Returns whether <paramref name="query1" /> contains <paramref name="query2" />.
    ///     Generates the "@&gt;" operator.
    ///     http://www.postgresql.org/docs/current/static/functions-textsearch.html
    /// </summary>
    public static bool Contains(this GaussDBTsQuery query1, GaussDBTsQuery query2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(Contains)));

    /// <summary>
    ///     Returns whether <paramref name="query1" /> is contained within <paramref name="query2" />.
    ///     Generates the "&lt;@" operator.
    ///     http://www.postgresql.org/docs/current/static/functions-textsearch.html
    /// </summary>
    public static bool IsContainedIn(this GaussDBTsQuery query1, GaussDBTsQuery query2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(IsContainedIn)));

    /// <summary>
    ///     Returns the number of lexemes plus operators in <paramref name="query" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static int GetNodeCount(this GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(GetNodeCount)));

    /// <summary>
    ///     Get the indexable part of <paramref name="query" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static string GetQueryTree(this GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(GetQueryTree)));

    /// <summary>
    ///     Returns a string suitable for display containing a query match.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-HEADLINE
    /// </summary>
    public static string GetResultHeadline(this GaussDBTsQuery query, string document)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(GetResultHeadline)));

    /// <summary>
    ///     Returns a string suitable for display containing a query match.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-HEADLINE
    /// </summary>
    public static string GetResultHeadline(this GaussDBTsQuery query, string document, string options)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(GetResultHeadline)));

    /// <summary>
    ///     Returns a string suitable for display containing a query match using the text
    ///     search configuration specified by <paramref name="config" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-HEADLINE
    /// </summary>
    public static string GetResultHeadline(this GaussDBTsQuery query, string config, string document, string options)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(GetResultHeadline)));

    /// <summary>
    ///     Searches <paramref name="query" /> for occurrences of <paramref name="target" />, and replaces
    ///     each occurrence with a <paramref name="substitute" />. All parameters are of type tsquery.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery Rewrite(this GaussDBTsQuery query, GaussDBTsQuery target, GaussDBTsQuery substitute)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(Rewrite)));

    /// <summary>
    ///     For each row of the SQL <paramref name="select" /> result, occurrences of the first column value (the target)
    ///     are replaced by the second column value (the substitute) within the current <paramref name="query" /> value.
    ///     The <paramref name="select" /> must yield two columns of tsquery type.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery Rewrite(this GaussDBTsQuery query, string select)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(Rewrite)));

    /// <summary>
    ///     Returns a tsquery that searches for a match to <paramref name="query1" /> followed by a match
    ///     to <paramref name="query2" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery ToPhrase(this GaussDBTsQuery query1, GaussDBTsQuery query2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(ToPhrase)));

    /// <summary>
    ///     Returns a tsquery that searches for a match to <paramref name="query1" /> followed by a match
    ///     to <paramref name="query2" /> at a distance of <paramref name="distance" /> lexemes using
    ///     the &lt;N&gt; tsquery operator
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSQUERY
    /// </summary>
    public static GaussDBTsQuery ToPhrase(this GaussDBTsQuery query1, GaussDBTsQuery query2, int distance)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsQuery) + "." + nameof(ToPhrase)));

    /// <summary>
    ///     This method generates the "@@" match operator. The <paramref name="query" /> parameter is
    ///     assumed to be a plain search query and will be converted to a tsquery using plainto_tsquery.
    ///     http://www.postgresql.org/docs/current/static/textsearch-intro.html#TEXTSEARCH-MATCHING
    /// </summary>
    public static bool Matches(this GaussDBTsVector vector, string query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Matches)));

    /// <summary>
    ///     This method generates the "@@" match operator.
    ///     http://www.postgresql.org/docs/current/static/textsearch-intro.html#TEXTSEARCH-MATCHING
    /// </summary>
    public static bool Matches(this GaussDBTsVector vector, GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Matches)));

    /// <summary>
    ///     Returns a vector which combines the lexemes and positional information of <paramref name="vector1" />
    ///     and <paramref name="vector2" /> using the || tsvector operator. Positions and weight labels are retained
    ///     during the concatenation.
    ///     https://www.postgresql.org/docs/10/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector Concat(this GaussDBTsVector vector1, GaussDBTsVector vector2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Concat)));

    /// <summary>
    ///     Assign weight to each element of <paramref name="vector" /> and return a new
    ///     weighted tsvector.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector SetWeight(this GaussDBTsVector vector, GaussDBTsVector.Lexeme.Weight weight)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(SetWeight)));

    /// <summary>
    ///     Assign weight to elements of <paramref name="vector" /> that are in <paramref name="lexemes" /> and
    ///     return a new weighted tsvector.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector SetWeight(this GaussDBTsVector vector, GaussDBTsVector.Lexeme.Weight weight, string[] lexemes)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(SetWeight)));

    /// <summary>
    ///     Assign weight to each element of <paramref name="vector" /> and return a new
    ///     weighted tsvector.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector SetWeight(this GaussDBTsVector vector, char weight)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(SetWeight)));

    /// <summary>
    ///     Assign weight to elements of <paramref name="vector" /> that are in <paramref name="lexemes" /> and
    ///     return a new weighted tsvector.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector SetWeight(this GaussDBTsVector vector, char weight, string[] lexemes)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(SetWeight)));

    /// <summary>
    ///     Return a new vector with <paramref name="lexeme" /> removed from <paramref name="vector" />
    ///     https://www.postgresql.org/docs/current/static/functions-textsearch.html
    /// </summary>
    public static GaussDBTsVector Delete(this GaussDBTsVector vector, string lexeme)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Delete)));

    /// <summary>
    ///     Return a new vector with <paramref name="lexemes" /> removed from <paramref name="vector" />
    ///     https://www.postgresql.org/docs/current/static/functions-textsearch.html
    /// </summary>
    public static GaussDBTsVector Delete(this GaussDBTsVector vector, string[] lexemes)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Delete)));

    /// <summary>
    ///     Returns a new vector with only lexemes having weights specified in <paramref name="weights" />.
    ///     https://www.postgresql.org/docs/current/static/functions-textsearch.html
    /// </summary>
    public static GaussDBTsVector Filter(this GaussDBTsVector vector, char[] weights)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Filter)));

    /// <summary>
    ///     Returns the number of lexemes in <paramref name="vector" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static int GetLength(this GaussDBTsVector vector)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(GetLength)));

    /// <summary>
    ///     Removes weights and positions from <paramref name="vector" /> and returns
    ///     a new stripped tsvector.
    ///     http://www.postgresql.org/docs/current/static/textsearch-features.html#TEXTSEARCH-MANIPULATE-TSVECTOR
    /// </summary>
    public static GaussDBTsVector ToStripped(this GaussDBTsVector vector)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(ToStripped)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float Rank(this GaussDBTsVector vector, GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Rank)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> while normalizing
    ///     the result according to the behaviors specified by <paramref name="normalization" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float Rank(this GaussDBTsVector vector, GaussDBTsQuery query, GaussDBTsRankingNormalization normalization)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Rank)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> with custom
    ///     weighting for word instances depending on their labels (D, C, B or A).
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float Rank(this GaussDBTsVector vector, float[] weights, GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Rank)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> while normalizing
    ///     the result according to the behaviors specified by <paramref name="normalization" />
    ///     and using custom weighting for word instances depending on their labels (D, C, B or A).
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float Rank(
        this GaussDBTsVector vector,
        float[] weights,
        GaussDBTsQuery query,
        GaussDBTsRankingNormalization normalization)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(Rank)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover
    ///     density method.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float RankCoverDensity(this GaussDBTsVector vector, GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(RankCoverDensity)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover
    ///     density method while normalizing the result according to the behaviors specified by
    ///     <paramref name="normalization" />.
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float RankCoverDensity(this GaussDBTsVector vector, GaussDBTsQuery query, GaussDBTsRankingNormalization normalization)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(RankCoverDensity)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover
    ///     density method with custom weighting for word instances depending on their labels (D, C, B or A).
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float RankCoverDensity(this GaussDBTsVector vector, float[] weights, GaussDBTsQuery query)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(RankCoverDensity)));

    /// <summary>
    ///     Calculates the rank of <paramref name="vector" /> for <paramref name="query" /> using the cover density
    ///     method while normalizing the result according to the behaviors specified by <paramref name="normalization" />
    ///     and using custom weighting for word instances depending on their labels (D, C, B or A).
    ///     http://www.postgresql.org/docs/current/static/textsearch-controls.html#TEXTSEARCH-RANKING
    /// </summary>
    public static float RankCoverDensity(
        this GaussDBTsVector vector,
        float[] weights,
        GaussDBTsQuery query,
        GaussDBTsRankingNormalization normalization)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GaussDBTsVector) + "." + nameof(RankCoverDensity)));
}
