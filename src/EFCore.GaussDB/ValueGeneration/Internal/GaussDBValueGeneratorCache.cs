using System.Collections.Concurrent;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.ValueGeneration.Internal;

/// <summary>
///     This API supports the Entity Framework Core infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
public class GaussDBValueGeneratorCache : ValueGeneratorCache, IGaussDBValueGeneratorCache
{
    private readonly ConcurrentDictionary<string, GaussDBSequenceValueGeneratorState> _sequenceGeneratorCache
        = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueGeneratorCache" /> class.
    /// </summary>
    /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
    public GaussDBValueGeneratorCache(ValueGeneratorCacheDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public virtual GaussDBSequenceValueGeneratorState GetOrAddSequenceState(
        IProperty property,
        IRelationalConnection connection)
    {
        var sequence = property.FindHiLoSequence();

        Debug.Assert(sequence is not null);

        return _sequenceGeneratorCache.GetOrAdd(
            GetSequenceName(sequence, connection),
            _ => new GaussDBSequenceValueGeneratorState(sequence));
    }

    private static string GetSequenceName(ISequence sequence, IRelationalConnection connection)
    {
        var dbConnection = connection.DbConnection;

        return dbConnection.Database.ToUpperInvariant()
            + "::"
            + dbConnection.DataSource.ToUpperInvariant()
            + "::"
            + (sequence.Schema is null ? "" : sequence.Schema + ".")
            + sequence.Name;
    }
}
