using System.Text;
using System.Text.Json;
using GaussDBTypes;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     The type mapping for the PostgreSQL pg_lsn type.
/// </summary>
/// <remarks>
///     See: https://www.postgresql.org/docs/current/datatype-pg-lsn.html
/// </remarks>
public class GaussDBPgLsnTypeMapping : GaussDBTypeMapping
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static GaussDBPgLsnTypeMapping Default { get; } = new();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBPgLsnTypeMapping()
        : base(
            "pg_lsn", typeof(GaussDBLogSequenceNumber), GaussDBDbType.PgLsn,
            jsonValueReaderWriter: JsonLogSequenceNumberReaderWriter.Instance)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBPgLsnTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, GaussDBDbType.PgLsn)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new GaussDBPgLsnTypeMapping(parameters);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
    {
        var lsn = (GaussDBLogSequenceNumber)value;
        var builder = new StringBuilder("PG_LSN '")
            .Append(lsn.ToString())
            .Append('\'');
        return builder.ToString();
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression GenerateCodeLiteral(object value)
    {
        var lsn = (GaussDBLogSequenceNumber)value;
        return Expression.New(Constructor, Expression.Constant((ulong)lsn));
    }

    private static readonly ConstructorInfo Constructor =
        typeof(GaussDBLogSequenceNumber).GetConstructor(new[] { typeof(ulong) })!;

    private sealed class JsonLogSequenceNumberReaderWriter : JsonValueReaderWriter<GaussDBLogSequenceNumber>
    {
        public static JsonLogSequenceNumberReaderWriter Instance { get; } = new();

        private JsonLogSequenceNumberReaderWriter()
        {
        }

        public override GaussDBLogSequenceNumber FromJsonTyped(ref Utf8JsonReaderManager manager, object? existingObject = null)
            => GaussDBLogSequenceNumber.Parse(manager.CurrentReader.GetString()!);

        public override void ToJsonTyped(Utf8JsonWriter writer, GaussDBLogSequenceNumber value)
            => writer.WriteStringValue(value.ToString());
    }
}
