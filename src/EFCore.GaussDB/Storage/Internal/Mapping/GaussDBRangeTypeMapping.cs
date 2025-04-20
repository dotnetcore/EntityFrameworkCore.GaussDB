using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     The type mapping for PostgreSQL range types.
/// </summary>
/// <remarks>
///     See: https://www.postgresql.org/docs/current/static/rangetypes.html
/// </remarks>
public class GaussDBRangeTypeMapping : GaussDBTypeMapping
{
    private PropertyInfo? _isEmptyProperty;
    private PropertyInfo? _lowerProperty;
    private PropertyInfo? _upperProperty;
    private PropertyInfo? _lowerInclusiveProperty;
    private PropertyInfo? _upperInclusiveProperty;
    private PropertyInfo? _lowerInfiniteProperty;
    private PropertyInfo? _upperInfiniteProperty;

    private ConstructorInfo? _rangeConstructor1;
    private ConstructorInfo? _rangeConstructor2;
    private ConstructorInfo? _rangeConstructor3;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static GaussDBRangeTypeMapping Default { get; } = new();

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    ///     The relational type mapping of the range's subtype.
    /// </summary>
    public virtual RelationalTypeMapping SubtypeMapping { get; }

    /// <summary>
    ///     For user-defined ranges, we have no <see cref="GaussDBDbType" /> and so the PG type name is set on
    ///     <see cref="GaussDBParameter.DataTypeName" /> instead.
    /// </summary>
    public virtual string? UnquotedStoreType { get; init; }

    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBRangeTypeMapping" /> class for a built-in range type which has a
    ///     <see cref="GaussDBDbType" /> defined.
    /// </summary>
    /// <param name="rangeStoreType">The database type to map</param>
    /// <param name="rangeClrType">The CLR type to map.</param>
    /// <param name="rangeGaussDBDbType">The <see cref="GaussDBDbType" /> of the built-in range.</param>
    /// <param name="subtypeMapping">The type mapping for the range subtype.</param>
    public static GaussDBRangeTypeMapping CreatBuiltInRangeMapping(
        string rangeStoreType,
        Type rangeClrType,
        GaussDBDbType rangeGaussDBDbType,
        RelationalTypeMapping subtypeMapping)
        => new(rangeStoreType, rangeClrType, rangeGaussDBDbType, subtypeMapping);

    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBRangeTypeMapping" /> class for a user-defined range type which doesn't have a
    ///     <see cref="GaussDBDbType" /> defined.
    /// </summary>
    /// <param name="quotedRangeStoreType">The database type to map, quoted.</param>
    /// <param name="unquotedRangeStoreType">The database type to map, unquoted.</param>
    /// <param name="rangeClrType">The CLR type to map.</param>
    /// <param name="subtypeMapping">The type mapping for the range subtype.</param>
    public static GaussDBRangeTypeMapping CreatUserDefinedRangeMapping(
        string quotedRangeStoreType,
        string unquotedRangeStoreType,
        Type rangeClrType,
        RelationalTypeMapping subtypeMapping)
        => new(quotedRangeStoreType, rangeClrType, rangeGaussDBDbType: GaussDBDbType.Unknown, subtypeMapping)
        {
            UnquotedStoreType = unquotedRangeStoreType
        };

    private GaussDBRangeTypeMapping(
        string rangeStoreType,
        Type rangeClrType,
        GaussDBDbType rangeGaussDBDbType,
        RelationalTypeMapping subtypeMapping)
        : base(rangeStoreType, rangeClrType, rangeGaussDBDbType)
    {
        SubtypeMapping = subtypeMapping;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBRangeTypeMapping(
        RelationalTypeMappingParameters parameters,
        GaussDBDbType GaussDBDbType,
        RelationalTypeMapping subtypeMapping)
        : base(parameters, GaussDBDbType)
    {
        SubtypeMapping = subtypeMapping;
    }

    // This constructor exists only to support the static Default property above, which is necessary to allow code generation for compiled
    // models. The constructor creates a completely blank type mapping, which will get cloned with all the correct details.
    private GaussDBRangeTypeMapping()
        : this("int4range", typeof(GaussDBRange<int>), GaussDBDbType.IntegerRange, subtypeMapping: null!)
    {
    }

    /// <summary>
    ///     This method exists only to support the compiled model.
    /// </summary>
    /// <remarks>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </remarks>
    public virtual GaussDBRangeTypeMapping Clone(GaussDBDbType GaussDBDbType, RelationalTypeMapping subtypeTypeMapping)
        => new(Parameters, GaussDBDbType, subtypeTypeMapping);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new GaussDBRangeTypeMapping(parameters, GaussDBDbType, SubtypeMapping);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override void ConfigureParameter(DbParameter parameter)
    {
        // Built-in range types have an GaussDBDbType, so we just do the normal thing.
        if (UnquotedStoreType is null)
        {
            Check.DebugAssert(GaussDBDbType is not GaussDBDbType.Unknown, "GaussDBDbType is Unknown but no PgDataTypeName is configured");
            base.ConfigureParameter(parameter);
            return;
        }

        Check.DebugAssert(GaussDBDbType is GaussDBDbType.Unknown, "PgDataTypeName is non-null, but GaussDBDbType is " + GaussDBDbType);

        if (parameter is not GaussDBParameter GaussDBParameter)
        {
            throw new InvalidOperationException(
                $"GaussDB-specific type mapping {GetType().Name} being used with non-GaussDB parameter type {parameter.GetType().Name}");
        }

        GaussDBParameter.DataTypeName = UnquotedStoreType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateNonNullSqlLiteral(object value)
        => $"'{GenerateEmbeddedNonNullSqlLiteral(value)}'::{StoreType}";

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string GenerateEmbeddedNonNullSqlLiteral(object value)
    {
        InitializeAccessors(ClrType, SubtypeMapping.ClrType);

        var builder = new StringBuilder();

        if ((bool)_isEmptyProperty.GetValue(value)!)
        {
            builder.Append("empty");
        }
        else
        {
            builder.Append((bool)_lowerInclusiveProperty.GetValue(value)! ? '[' : '(');

            if (!(bool)_lowerInfiniteProperty.GetValue(value)!)
            {
                builder.Append(SubtypeMapping.GenerateEmbeddedSqlLiteral(_lowerProperty.GetValue(value)));
            }

            builder.Append(',');

            if (!(bool)_upperInfiniteProperty.GetValue(value)!)
            {
                builder.Append(SubtypeMapping.GenerateEmbeddedSqlLiteral(_upperProperty.GetValue(value)));
            }

            builder.Append((bool)_upperInclusiveProperty.GetValue(value)! ? ']' : ')');
        }

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
        InitializeAccessors(ClrType, SubtypeMapping.ClrType);

        var lower = _lowerProperty.GetValue(value);
        var upper = _upperProperty.GetValue(value);
        var lowerInclusive = (bool)_lowerInclusiveProperty.GetValue(value)!;
        var upperInclusive = (bool)_upperInclusiveProperty.GetValue(value)!;
        var lowerInfinite = (bool)_lowerInfiniteProperty.GetValue(value)!;
        var upperInfinite = (bool)_upperInfiniteProperty.GetValue(value)!;

        return lowerInfinite || upperInfinite
            ? Expression.New(
                _rangeConstructor3,
                Expression.Constant(lower),
                Expression.Constant(lowerInclusive),
                Expression.Constant(lowerInfinite),
                Expression.Constant(upper),
                Expression.Constant(upperInclusive),
                Expression.Constant(upperInfinite))
            : lowerInclusive && upperInclusive
                ? Expression.New(
                    _rangeConstructor1,
                    Expression.Constant(lower),
                    Expression.Constant(upper))
                : Expression.New(
                    _rangeConstructor2,
                    Expression.Constant(lower),
                    Expression.Constant(lowerInclusive),
                    Expression.Constant(upper),
                    Expression.Constant(upperInclusive));
    }

    [MemberNotNull(
        "_isEmptyProperty",
        "_lowerProperty", "_upperProperty",
        "_lowerInclusiveProperty", "_upperInclusiveProperty",
        "_lowerInfiniteProperty", "_upperInfiniteProperty",
        "_rangeConstructor1", "_rangeConstructor2", "_rangeConstructor3")]
    private void InitializeAccessors(Type rangeClrType, Type subtypeClrType)
    {
        _isEmptyProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.IsEmpty))!;
        _lowerProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.LowerBound))!;
        _upperProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.UpperBound))!;
        _lowerInclusiveProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.LowerBoundIsInclusive))!;
        _upperInclusiveProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.UpperBoundIsInclusive))!;
        _lowerInfiniteProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.LowerBoundInfinite))!;
        _upperInfiniteProperty = rangeClrType.GetProperty(nameof(GaussDBRange<int>.UpperBoundInfinite))!;

        _rangeConstructor1 = rangeClrType.GetConstructor(
            [subtypeClrType, subtypeClrType])!;
        _rangeConstructor2 = rangeClrType.GetConstructor(
            [subtypeClrType, typeof(bool), subtypeClrType, typeof(bool)])!;
        _rangeConstructor3 = rangeClrType.GetConstructor(
            [subtypeClrType, typeof(bool), typeof(bool), subtypeClrType, typeof(bool), typeof(bool)])!;
    }
}
