using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     The base class for mapping GaussDB-specific types. It configures parameters with the
///     <see cref="GaussDBDbType" /> provider-specific type enum.
/// </summary>
public abstract class GaussDBTypeMapping : RelationalTypeMapping, IGaussDBTypeMapping
{
    /// <inheritdoc />
    public virtual GaussDBDbType GaussDBDbType { get; }

    // ReSharper disable once PublicConstructorInAbstractClass
    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBTypeMapping" /> class.
    /// </summary>
    /// <param name="storeType">The database type to map.</param>
    /// <param name="clrType">The CLR type to map.</param>
    /// <param name="gaussDBDbType">The database type used by GaussDB.</param>
    /// <param name="jsonValueReaderWriter">Handles reading and writing JSON values for instances of the mapped type.</param>
    public GaussDBTypeMapping(string storeType, Type clrType, GaussDBDbType gaussDBDbType, JsonValueReaderWriter? jsonValueReaderWriter = null)
        : base(storeType, clrType, jsonValueReaderWriter: jsonValueReaderWriter)
    {
        GaussDBDbType = gaussDBDbType;
    }

    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBTypeMapping" /> class.
    /// </summary>
    /// <param name="parameters">The parameters for this mapping.</param>
    /// <param name="gaussDBDbType">The database type of the range subtype.</param>
    protected GaussDBTypeMapping(RelationalTypeMappingParameters parameters, GaussDBDbType gaussDBDbType)
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
                $"GaussDB-specific type mapping {GetType().Name} being used with non-GaussDB parameter type {parameter.GetType().Name}");
        }

        base.ConfigureParameter(parameter);
        GaussDBParameter.GaussDBDbType = GaussDBDbType;
    }

    /// <summary>
    ///     Generates the SQL representation of a literal value meant to be embedded in another literal value, e.g. in a range.
    /// </summary>
    /// <param name="value">The literal value.</param>
    /// <returns>
    ///     The generated string.
    /// </returns>
    public virtual string GenerateEmbeddedSqlLiteral(object? value)
    {
        value = ConvertUnderlyingEnumValueToEnum(value);

        if (Converter != null)
        {
            value = Converter.ConvertToProvider(value);
        }

        return GenerateEmbeddedProviderValueSqlLiteral(value);
    }

    /// <summary>
    ///     Generates the SQL representation of a literal value without conversion, meant to be embedded in another literal value,
    ///     e.g. in a range.
    /// </summary>
    /// <param name="value">The literal value.</param>
    /// <returns>
    ///     The generated string.
    /// </returns>
    public virtual string GenerateEmbeddedProviderValueSqlLiteral(object? value)
        => value == null
            ? "NULL"
            : GenerateEmbeddedNonNullSqlLiteral(value);

    /// <summary>
    ///     Generates the SQL representation of a non-null literal value, meant to be embedded in another literal value, e.g. in a range.
    /// </summary>
    /// <param name="value">The literal value.</param>
    /// <returns>
    ///     The generated string.
    /// </returns>
    protected virtual string GenerateEmbeddedNonNullSqlLiteral(object value)
        => GenerateNonNullSqlLiteral(value);

    // Copied from RelationalTypeMapping
    private object? ConvertUnderlyingEnumValueToEnum(object? value)
        => value?.GetType().IsInteger() == true && ClrType.UnwrapNullableType().IsEnum
            ? Enum.ToObject(ClrType.UnwrapNullableType(), value)
            : value;
}
