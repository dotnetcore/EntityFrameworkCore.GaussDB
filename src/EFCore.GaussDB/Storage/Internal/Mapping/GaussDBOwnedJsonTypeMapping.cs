using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     Supports the standard EF JSON support, which relies on owned entity modeling.
///     See <see cref="GaussDBJsonTypeMapping" /> for the older GaussDB-specific support, which allows mapping json/jsonb to text, to e.g.
///     <see cref="JsonElement" /> (weakly-typed mapping) or to arbitrary POCOs (but without them being modeled).
/// </summary>
public class GaussDBOwnedJsonTypeMapping : JsonTypeMapping
{
    /// <summary>
    ///     The database type used by GaussDB (<see cref="GaussDBDbType.Json" /> or <see cref="GaussDBDbType.Jsonb" />.
    /// </summary>
    public virtual GaussDBDbType GaussDBDbType { get; }

    private static readonly MethodInfo GetStringMethod
        = typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), [typeof(int)])!;

    private static readonly PropertyInfo UTF8Property
        = typeof(Encoding).GetProperty(nameof(Encoding.UTF8))!;

    private static readonly MethodInfo EncodingGetBytesMethod
        = typeof(Encoding).GetMethod(nameof(Encoding.GetBytes), [typeof(string)])!;

    private static readonly ConstructorInfo MemoryStreamConstructor
        = typeof(MemoryStream).GetConstructor([typeof(byte[])])!;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBOwnedJsonTypeMapping(string storeType)
        : base(storeType, typeof(JsonElement), dbType: null)
    {
        GaussDBDbType = storeType switch
        {
            "json" => GaussDBDbType.Json,
            "jsonb" => GaussDBDbType.Jsonb,
            _ => throw new ArgumentException("Only the json and jsonb types are supported", nameof(storeType))
        };
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override MethodInfo GetDataReaderMethod()
        => GetStringMethod;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression CustomizeDataReaderExpression(Expression expression)
        => Expression.New(
            MemoryStreamConstructor,
            Expression.Call(
                Expression.Property(null, UTF8Property),
                EncodingGetBytesMethod,
                expression));

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBOwnedJsonTypeMapping(RelationalTypeMappingParameters parameters, GaussDBDbType gaussDBDbType)
        : base(parameters)
    {
        GaussDBDbType = gaussDBDbType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override void ConfigureParameter(DbParameter parameter)
    {
        if (parameter is not GaussDBParameter GaussDBParameter)
        {
            throw new InvalidOperationException(
                $"GaussDB-specific type mapping {nameof(GaussDBOwnedJsonTypeMapping)} being used with non-GaussDB parameter type {parameter.GetType().Name}");
        }

        base.ConfigureParameter(parameter);
        GaussDBParameter.GaussDBDbType = GaussDBDbType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual string EscapeSqlLiteral(string literal)
        => literal.Replace("'", "''");

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"'{EscapeSqlLiteral(JsonSerializer.Serialize(value))}'";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new GaussDBOwnedJsonTypeMapping(parameters, GaussDBDbType);
}
