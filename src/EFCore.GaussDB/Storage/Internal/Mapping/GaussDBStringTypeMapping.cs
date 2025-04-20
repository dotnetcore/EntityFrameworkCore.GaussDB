using System.Data.Common;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

/// <summary>
///     The base class for mapping GaussDB-specific string types. It configures parameters with the
///     <see cref="GaussDBDbType" /> provider-specific type enum.
/// </summary>
public class GaussDBStringTypeMapping : StringTypeMapping, IGaussDBTypeMapping
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static new GaussDBStringTypeMapping Default { get; } = new("text", GaussDBDbType.Text);

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual GaussDBDbType GaussDBDbType { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBStringTypeMapping(string storeType, GaussDBDbType gaussDBDbType)
        : base(storeType, System.Data.DbType.String)
    {
        GaussDBDbType = gaussDBDbType;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBStringTypeMapping(
        RelationalTypeMappingParameters parameters,
        GaussDBDbType gaussDBDbType)
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
    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new GaussDBStringTypeMapping(parameters, GaussDBDbType);

    /// <summary>
    ///     This method exists only to support the compiled model.
    /// </summary>
    /// <remarks>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </remarks>
    public virtual GaussDBStringTypeMapping Clone(GaussDBDbType GaussDBDbType)
        => new(Parameters, GaussDBDbType);

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
}
