using System.Net;
using System.Net.NetworkInformation;
using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Expressions;
using static GaussDB.EntityFrameworkCore.PostgreSQL.Utilities.Statics;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     Provides translation services for operators and functions of PostgreSQL network typess (cidr, inet, macaddr, macaddr8).
/// </summary>
/// <remarks>
///     See: https://www.postgresql.org/docs/current/static/functions-net.html
/// </remarks>
public class GaussDBNetworkTranslator : IMethodCallTranslator
{
    private static readonly MethodInfo IPAddressParse =
        typeof(IPAddress).GetRuntimeMethod(nameof(IPAddress.Parse), new[] { typeof(string) })!;

    private static readonly MethodInfo PhysicalAddressParse =
        typeof(PhysicalAddress).GetRuntimeMethod(nameof(PhysicalAddress.Parse), new[] { typeof(string) })!;

    private readonly GaussDBSqlExpressionFactory _sqlExpressionFactory;

    private readonly RelationalTypeMapping _inetMapping;
    private readonly RelationalTypeMapping _cidrMapping;
    private readonly RelationalTypeMapping _macaddr8Mapping;
    private readonly RelationalTypeMapping _longAddressMapping;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBNetworkTranslator(
        IRelationalTypeMappingSource typeMappingSource,
        GaussDBSqlExpressionFactory sqlExpressionFactory,
        IModel model)
    {
        _sqlExpressionFactory = sqlExpressionFactory;
        _inetMapping = typeMappingSource.FindMapping("inet")!;
        _cidrMapping = typeMappingSource.FindMapping("cidr")!;
        _macaddr8Mapping = typeMappingSource.FindMapping("macaddr8")!;
        _longAddressMapping = typeMappingSource.FindMapping(typeof(long), model)!;
    }

    /// <inheritdoc />
    public virtual SqlExpression? Translate(
        SqlExpression? instance,
        MethodInfo method,
        IReadOnlyList<SqlExpression> arguments,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        if (method == IPAddressParse)
        {
            return _sqlExpressionFactory.Convert(arguments[0], typeof(IPAddress));
        }

        if (method == PhysicalAddressParse)
        {
            return _sqlExpressionFactory.Convert(arguments[0], typeof(PhysicalAddress));
        }

        if (method.DeclaringType == typeof(GaussDBNetworkDbFunctionsExtensions))
        {
            var paramType = method.GetParameters()[1].ParameterType;

            if (paramType == typeof(GaussDBInet))
            {
                return TranslateInetExtensionMethod(method, arguments);
            }

            if (paramType == typeof(GaussDBCidr))
            {
                return TranslateCidrExtensionMethod(method, arguments);
            }

            if (paramType == typeof(PhysicalAddress))
            {
                return TranslateMacaddrExtensionMethod(method, arguments);
            }
        }

        return null;
    }

    private SqlExpression? TranslateInetExtensionMethod(MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        => method.Name switch
        {
            nameof(GaussDBNetworkDbFunctionsExtensions.LessThan)
                => new SqlBinaryExpression(
                    ExpressionType.LessThan,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.LessThanOrEqual)
                => new SqlBinaryExpression(
                    ExpressionType.LessThanOrEqual,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.GreaterThanOrEqual)
                => new SqlBinaryExpression(
                    ExpressionType.GreaterThanOrEqual,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.GreaterThan)
                => new SqlBinaryExpression(
                    ExpressionType.GreaterThan,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.ContainedBy)
                => _sqlExpressionFactory.ContainedBy(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.ContainedByOrEqual)
                => _sqlExpressionFactory.MakePostgresBinary(
                    PgExpressionType.NetworkContainedByOrEqual, arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.Contains)
                => _sqlExpressionFactory.Contains(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.ContainsOrEqual)
                => _sqlExpressionFactory.MakePostgresBinary(PgExpressionType.NetworkContainsOrEqual, arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.ContainsOrContainedBy)
                => _sqlExpressionFactory.MakePostgresBinary(
                    PgExpressionType.NetworkContainsOrContainedBy, arguments[1], arguments[2]),

            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseNot)
                => new SqlUnaryExpression(
                    ExpressionType.Not,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseAnd)
                => new SqlBinaryExpression(
                    ExpressionType.And,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseOr)
                => new SqlBinaryExpression(
                    ExpressionType.Or,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.Add)
                => new SqlBinaryExpression(
                    ExpressionType.Add,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.Subtract) when arguments[2].Type == typeof(long)
                => new SqlBinaryExpression(
                    ExpressionType.Subtract,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(GaussDBInet),
                    _inetMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.Subtract)
                => new SqlBinaryExpression(
                    ExpressionType.Subtract,
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]),
                    _sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[2]),
                    typeof(long),
                    _longAddressMapping),

            nameof(GaussDBNetworkDbFunctionsExtensions.Abbreviate)
                => NullPropagatingFunction("abbrev", new[] { arguments[1] }, typeof(string)),
            nameof(GaussDBNetworkDbFunctionsExtensions.Broadcast)
                => NullPropagatingFunction("broadcast", new[] { arguments[1] }, typeof(IPAddress), _inetMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.Family)
                => NullPropagatingFunction("family", new[] { arguments[1] }, typeof(int)),
            nameof(GaussDBNetworkDbFunctionsExtensions.Host)
                => NullPropagatingFunction("host", new[] { arguments[1] }, typeof(string)),
            nameof(GaussDBNetworkDbFunctionsExtensions.HostMask)
                => NullPropagatingFunction("hostmask", new[] { arguments[1] }, typeof(IPAddress), _inetMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.MaskLength)
                => NullPropagatingFunction("masklen", new[] { arguments[1] }, typeof(int)),
            nameof(GaussDBNetworkDbFunctionsExtensions.Netmask)
                => NullPropagatingFunction("netmask", new[] { arguments[1] }, typeof(IPAddress), _inetMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.Network)
                => NullPropagatingFunction("network", new[] { arguments[1] }, typeof((IPAddress Address, int Subnet)), _cidrMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.SetMaskLength)
                => NullPropagatingFunction(
                    "set_masklen", new[] { arguments[1], arguments[2] }, arguments[1].Type, arguments[1].TypeMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.Text)
                => NullPropagatingFunction("text", new[] { arguments[1] }, typeof(string)),
            nameof(GaussDBNetworkDbFunctionsExtensions.SameFamily)
                => NullPropagatingFunction("inet_same_family", new[] { arguments[1], arguments[2] }, typeof(bool)),
            nameof(GaussDBNetworkDbFunctionsExtensions.Merge)
                => NullPropagatingFunction(
                    "inet_merge", new[] { arguments[1], arguments[2] }, typeof((IPAddress Address, int Subnet)), _cidrMapping),

            _ => null
        };

    private SqlExpression? TranslateCidrExtensionMethod(MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        => method.Name switch
        {
            nameof(GaussDBNetworkDbFunctionsExtensions.Abbreviate)
                => NullPropagatingFunction("abbrev", new[] { arguments[1] }, typeof(string)),
            nameof(GaussDBNetworkDbFunctionsExtensions.SetMaskLength)
                => NullPropagatingFunction(
                    "set_masklen", new[] { arguments[1], arguments[2] }, arguments[1].Type, arguments[1].TypeMapping),

            _ => null
        };

    private SqlExpression? TranslateMacaddrExtensionMethod(MethodInfo method, IReadOnlyList<SqlExpression> arguments)
        => method.Name switch
        {
            nameof(GaussDBNetworkDbFunctionsExtensions.LessThan)
                => _sqlExpressionFactory.LessThan(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.LessThanOrEqual)
                => _sqlExpressionFactory.LessThanOrEqual(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.GreaterThanOrEqual)
                => _sqlExpressionFactory.GreaterThanOrEqual(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.GreaterThan)
                => _sqlExpressionFactory.GreaterThan(arguments[1], arguments[2]),

            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseNot)
                => _sqlExpressionFactory.Not(arguments[1]),
            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseAnd)
                => _sqlExpressionFactory.And(arguments[1], arguments[2]),
            nameof(GaussDBNetworkDbFunctionsExtensions.BitwiseOr)
                => _sqlExpressionFactory.Or(arguments[1], arguments[2]),

            nameof(GaussDBNetworkDbFunctionsExtensions.Truncate) => NullPropagatingFunction(
                "trunc", new[] { arguments[1] }, typeof(PhysicalAddress), arguments[1].TypeMapping),
            nameof(GaussDBNetworkDbFunctionsExtensions.Set7BitMac8) => NullPropagatingFunction(
                "macaddr8_set7bit", new[] { arguments[1] }, typeof(PhysicalAddress), _macaddr8Mapping),

            _ => null
        };

    private SqlFunctionExpression NullPropagatingFunction(
        string name,
        SqlExpression[] arguments,
        Type returnType,
        RelationalTypeMapping? typeMapping = null)
        => _sqlExpressionFactory.Function(
            name,
            arguments,
            nullable: true,
            argumentsPropagateNullability: TrueArrays[arguments.Length],
            returnType,
            typeMapping);
}
