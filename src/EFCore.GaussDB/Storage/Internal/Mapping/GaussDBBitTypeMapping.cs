﻿using System.Collections;
using System.Text;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Json;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     The type mapping for the PostgreSQL bit string type.
/// </summary>
/// <remarks>
///     See: https://www.postgresql.org/docs/current/static/datatype-bit.html
/// </remarks>
public class GaussDBBitTypeMapping : GaussDBTypeMapping
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static GaussDBBitTypeMapping Default { get; } = new();

    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBBitTypeMapping" /> class.
    /// </summary>
    public GaussDBBitTypeMapping()
        : base("bit", typeof(BitArray), GaussDBDbType.Bit, jsonValueReaderWriter: JsonBitArrayReaderWriter.Instance)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBBitTypeMapping(RelationalTypeMappingParameters parameters)
        : base(parameters, GaussDBDbType.Bit)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new GaussDBBitTypeMapping(parameters);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
    {
        var bits = (BitArray)value;
        var sb = new StringBuilder();
        sb.Append("B'");
        for (var i = 0; i < bits.Count; i++)
        {
            sb.Append(bits[i] ? '1' : '0');
        }

        sb.Append('\'');
        return sb.ToString();
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override Expression GenerateCodeLiteral(object value)
    {
        var bits = (BitArray)value;
        var exprs = new Expression[bits.Count];
        for (var i = 0; i < bits.Count; i++)
        {
            exprs[i] = Expression.Constant(bits[i]);
        }

        return Expression.New(Constructor, Expression.NewArrayInit(typeof(bool), exprs));
    }

    private static readonly ConstructorInfo Constructor =
        typeof(BitArray).GetConstructor([typeof(bool[])])!;
}
