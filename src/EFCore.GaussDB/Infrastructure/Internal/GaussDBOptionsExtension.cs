using System.Data.Common;
using System.Globalization;
using System.Net.Security;
using System.Text;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

/// <summary>
///     Represents options managed by the GaussDB.
/// </summary>
public class GaussDBOptionsExtension : RelationalOptionsExtension
{
    private DbContextOptionsExtensionInfo? _info;
    private readonly List<UserRangeDefinition> _userRangeDefinitions;

    private Version? _postgresVersion;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static readonly Version DefaultPostgresVersion = new(14, 0);

    /// <summary>
    ///     The backend version to target.
    /// </summary>
    public virtual Version PostgresVersion
        => _postgresVersion ?? DefaultPostgresVersion;

    /// <summary>
    ///     The backend version to target, but returns <see langword="null" /> unless the user explicitly specified a version.
    /// </summary>
    public virtual bool IsPostgresVersionSet
        => _postgresVersion is not null;

    /// <summary>
    ///     The <see cref="DbDataSource" />, or <see langword="null" /> if a connection string or <see cref="DbConnection" /> was used
    ///     instead of a <see cref="DbDataSource" />.
    /// </summary>
    public virtual DbDataSource? DataSource { get; private set; }

    /// <summary>
    ///     The name of the database for administrative operations.
    /// </summary>
    public virtual string? AdminDatabase { get; private set; }

    /// <summary>
    ///     Whether to target Redshift.
    /// </summary>
    public virtual bool UseRedshift { get; private set; }

    /// <summary>
    ///     The list of range mappings specified by the user.
    /// </summary>
    public virtual IReadOnlyList<UserRangeDefinition> UserRangeDefinitions
        => _userRangeDefinitions;

    /// <summary>
    ///     The specified <see cref="ProvideClientCertificatesCallback" />.
    /// </summary>
    public virtual ProvideClientCertificatesCallback? ProvideClientCertificatesCallback { get; private set; }

    /// <summary>
    ///     The specified <see cref="RemoteCertificateValidationCallback" />.
    /// </summary>
    public virtual RemoteCertificateValidationCallback? RemoteCertificateValidationCallback { get; private set; }

    /// <summary>
    ///     The specified <see cref="ProvidePasswordCallback" />.
    /// </summary>
#pragma warning disable CS0618 // ProvidePasswordCallback is obsolete
    public virtual ProvidePasswordCallback? ProvidePasswordCallback { get; private set; }
#pragma warning restore CS0618

    /// <summary>
    ///     True if reverse null ordering is enabled; otherwise, false.
    /// </summary>
    public virtual bool ReverseNullOrdering { get; private set; }

    /// <summary>
    ///     Initializes an instance of <see cref="GaussDBOptionsExtension" /> with the default settings.
    /// </summary>
    public GaussDBOptionsExtension()
    {
        _userRangeDefinitions = new List<UserRangeDefinition>();
    }

    // NB: When adding new options, make sure to update the copy ctor below.
    /// <summary>
    ///     Initializes an instance of <see cref="GaussDBOptionsExtension" /> by copying the specified instance.
    /// </summary>
    /// <param name="copyFrom">The instance to copy.</param>
    public GaussDBOptionsExtension(GaussDBOptionsExtension copyFrom)
        : base(copyFrom)
    {
        DataSource = copyFrom.DataSource;
        AdminDatabase = copyFrom.AdminDatabase;
        _postgresVersion = copyFrom._postgresVersion;
        UseRedshift = copyFrom.UseRedshift;
        _userRangeDefinitions = new List<UserRangeDefinition>(copyFrom._userRangeDefinitions);
        ProvideClientCertificatesCallback = copyFrom.ProvideClientCertificatesCallback;
        RemoteCertificateValidationCallback = copyFrom.RemoteCertificateValidationCallback;
        ProvidePasswordCallback = copyFrom.ProvidePasswordCallback;
        ReverseNullOrdering = copyFrom.ReverseNullOrdering;
    }

    // The following is a hack to set the default minimum batch size to 2 in GaussDB
    // See https://github.com/aspnet/EntityFrameworkCore/pull/10091
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override int? MinBatchSize
        => base.MinBatchSize ?? 2;

    /// <summary>
    ///     Creates a new instance with all options the same as for this instance, but with the given option changed.
    ///     It is unusual to call this method directly. Instead use <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    /// <param name="dataSource">The option to change.</param>
    /// <returns>A new instance with the option changed.</returns>
    public virtual RelationalOptionsExtension WithDataSource(DbDataSource? dataSource)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.DataSource = dataSource;

        return clone;
    }

    /// <inheritdoc />
    public override RelationalOptionsExtension WithConnectionString(string? connectionString)
    {
        var clone = (GaussDBOptionsExtension)base.WithConnectionString(connectionString);

        clone.DataSource = null;

        return clone;
    }

    /// <inheritdoc />
    public override RelationalOptionsExtension WithConnection(DbConnection? connection)
    {
        var clone = (GaussDBOptionsExtension)base.WithConnection(connection);

        clone.DataSource = null;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance configured with the specified range mapping.
    /// </summary>
    public virtual GaussDBOptionsExtension WithUserRangeDefinition<TSubtype>(
        string rangeName,
        string? schemaName = null,
        string? subtypeName = null)
        => WithUserRangeDefinition(rangeName, schemaName, typeof(TSubtype), subtypeName);

    /// <summary>
    ///     Returns a copy of the current instance configured with the specified range mapping.
    /// </summary>
    public virtual GaussDBOptionsExtension WithUserRangeDefinition(
        string rangeName,
        string? schemaName,
        Type subtypeClrType,
        string? subtypeName)
    {
        Check.NotEmpty(rangeName, nameof(rangeName));
        Check.NotNull(subtypeClrType, nameof(subtypeClrType));

        var clone = (GaussDBOptionsExtension)Clone();

        clone._userRangeDefinitions.Add(new UserRangeDefinition(rangeName, schemaName, subtypeClrType, subtypeName));

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance configured to use the specified administrative database.
    /// </summary>
    /// <param name="adminDatabase">The name of the database for administrative operations.</param>
    public virtual GaussDBOptionsExtension WithAdminDatabase(string? adminDatabase)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.AdminDatabase = adminDatabase;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance with the specified PostgreSQL version.
    /// </summary>
    /// <param name="postgresVersion">The backend version to target.</param>
    /// <returns>
    ///     A copy of the current instance with the specified PostgreSQL version.
    /// </returns>
    public virtual GaussDBOptionsExtension WithPostgresVersion(Version? postgresVersion)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone._postgresVersion = postgresVersion;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance with the specified Redshift settings.
    /// </summary>
    /// <param name="useRedshift">Whether to target Redshift.</param>
    /// <returns>
    ///     A copy of the current instance with the specified Redshift setting.
    /// </returns>
    public virtual GaussDBOptionsExtension WithRedshift(bool useRedshift)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.UseRedshift = useRedshift;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance configured with the specified value..
    /// </summary>
    /// <param name="reverseNullOrdering">True to enable reverse null ordering; otherwise, false.</param>
    internal virtual GaussDBOptionsExtension WithReverseNullOrdering(bool reverseNullOrdering)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.ReverseNullOrdering = reverseNullOrdering;

        return clone;
    }

    /// <inheritdoc />
    public override void Validate(IDbContextOptions options)
    {
        base.Validate(options);

        // If we don't have an explicitly-configured data source, try to get one from the application service provider.
        var dataSource = DataSource
            ?? options.FindExtension<CoreOptionsExtension>()?.ApplicationServiceProvider?.GetService<GaussDBDataSource>();

        if (dataSource is not null)
        {
            if (ProvideClientCertificatesCallback is not null)
            {
                throw new InvalidOperationException(
                    "When passing an GaussDBDataSource to UseGaussDB(), call 'ProvideClientCertificatesCallback' on GaussDBDataSourceBuilder rather than in UseGaussDB().");
            }

            if (RemoteCertificateValidationCallback is not null)
            {
                throw new InvalidOperationException(
                    "When passing an GaussDBDataSource to UseGaussDB(), call 'RemoteCertificateValidationCallback' on GaussDBDataSourceBuilder rather than in UseGaussDB().");
            }

            if (ProvidePasswordCallback is not null)
            {
                throw new InvalidOperationException(
                    "When passing an GaussDBDataSource to UseGaussDB(), 'ProviderPasswordCallback' cannot be used in UseGaussDB(). See https://www.GaussDB.org/doc/security.html for configuring passwords and token rotation on GaussDBDataSourceBuilder.");
            }
        }

        if (UseRedshift && _postgresVersion is not null)
        {
            throw new InvalidOperationException($"{nameof(UseRedshift)} and {nameof(PostgresVersion)} cannot both be set");
        }
    }

    #region Authentication

    /// <summary>
    ///     Returns a copy of the current instance with the specified <see cref="ProvideClientCertificatesCallback" />.
    /// </summary>
    /// <param name="callback">The specified callback.</param>
    public virtual GaussDBOptionsExtension WithProvideClientCertificatesCallback(ProvideClientCertificatesCallback? callback)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.ProvideClientCertificatesCallback = callback;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance with the specified <see cref="RemoteCertificateValidationCallback" />.
    /// </summary>
    /// <param name="callback">The specified callback.</param>
    public virtual GaussDBOptionsExtension WithRemoteCertificateValidationCallback(RemoteCertificateValidationCallback? callback)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.RemoteCertificateValidationCallback = callback;

        return clone;
    }

    /// <summary>
    ///     Returns a copy of the current instance with the specified <see cref="ProvidePasswordCallback" />.
    /// </summary>
    /// <param name="callback">The specified callback.</param>
#pragma warning disable CS0618 // ProvidePasswordCallback is obsolete
    public virtual GaussDBOptionsExtension WithProvidePasswordCallback(ProvidePasswordCallback? callback)
    {
        var clone = (GaussDBOptionsExtension)Clone();

        clone.ProvidePasswordCallback = callback;

        return clone;
    }
#pragma warning restore CS0618

    #endregion Authentication

    #region Infrastructure

    /// <inheritdoc />
    protected override RelationalOptionsExtension Clone()
        => new GaussDBOptionsExtension(this);

    /// <inheritdoc />
    public override void ApplyServices(IServiceCollection services)
        => services.AddEntityFrameworkGaussDB();

    /// <inheritdoc />
    public override DbContextOptionsExtensionInfo Info
        => _info ??= new ExtensionInfo(this);

    private sealed class ExtensionInfo : RelationalExtensionInfo
    {
        private int? _serviceProviderHash;
        private string? _logFragment;

        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        private new GaussDBOptionsExtension Extension
            => (GaussDBOptionsExtension)base.Extension;

        public override bool IsDatabaseProvider
            => true;

        public override string LogFragment
        {
            get
            {
                if (_logFragment is not null)
                {
                    return _logFragment;
                }

                var builder = new StringBuilder(base.LogFragment);

                if (Extension.AdminDatabase is not null)
                {
                    builder.Append(nameof(Extension.AdminDatabase)).Append("=").Append(Extension.AdminDatabase).Append(' ');
                }

                if (Extension._postgresVersion is not null)
                {
                    builder.Append(nameof(Extension.PostgresVersion)).Append("=").Append(Extension.PostgresVersion).Append(' ');
                }

                if (Extension.UseRedshift)
                {
                    builder.Append(nameof(Extension.UseRedshift)).Append(' ');
                }

                if (Extension.ProvideClientCertificatesCallback is not null)
                {
                    builder.Append(nameof(Extension.ProvideClientCertificatesCallback)).Append(" ");
                }

                if (Extension.RemoteCertificateValidationCallback is not null)
                {
                    builder.Append(nameof(Extension.RemoteCertificateValidationCallback)).Append(" ");
                }

                if (Extension.ProvidePasswordCallback is not null)
                {
                    builder.Append(nameof(Extension.ProvidePasswordCallback)).Append(" ");
                }

                if (Extension.ReverseNullOrdering)
                {
                    builder.Append(nameof(Extension.ReverseNullOrdering)).Append(" ");
                }

                if (Extension.UserRangeDefinitions.Count > 0)
                {
                    builder.Append(nameof(Extension.UserRangeDefinitions)).Append("=[");
                    foreach (var item in Extension.UserRangeDefinitions)
                    {
                        builder.Append(item.SubtypeClrType).Append("=>");

                        if (item.SchemaName is not null)
                        {
                            builder.Append(item.SchemaName).Append(".");
                        }

                        builder.Append(item.RangeName);

                        if (item.SubtypeName is not null)
                        {
                            builder.Append("(").Append(item.SubtypeName).Append(")");
                        }

                        builder.Append(";");
                    }

                    builder.Length -= 1;
                    builder.Append("] ");
                }

                return _logFragment = builder.ToString();
            }
        }

        public override int GetServiceProviderHashCode()
        {
            if (_serviceProviderHash is null)
            {
                var hashCode = new HashCode();

                foreach (var userRangeDefinition in Extension._userRangeDefinitions)
                {
                    hashCode.Add(userRangeDefinition);
                }

                if (Extension.DataSource is not null)
                {
                    hashCode.Add(Extension.DataSource.ConnectionString);
                }

                hashCode.Add(Extension.AdminDatabase);
                hashCode.Add(Extension.PostgresVersion);
                hashCode.Add(Extension.UseRedshift);
                hashCode.Add(Extension.ProvideClientCertificatesCallback);
                hashCode.Add(Extension.RemoteCertificateValidationCallback);
                hashCode.Add(Extension.ProvidePasswordCallback);
                hashCode.Add(Extension.ReverseNullOrdering);

                _serviceProviderHash = hashCode.ToHashCode();
            }

            return _serviceProviderHash.Value;
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo otherInfo
                && Extension.PostgresVersion == otherInfo.Extension.PostgresVersion
                && ReferenceEquals(Extension.DataSource, otherInfo.Extension.DataSource)
                && Extension.ReverseNullOrdering == otherInfo.Extension.ReverseNullOrdering
                && Extension.UserRangeDefinitions.SequenceEqual(otherInfo.Extension.UserRangeDefinitions)
                && Extension.UseRedshift == otherInfo.Extension.UseRedshift;

        /// <inheritdoc />
        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.UseAdminDatabase)]
                = (Extension.AdminDatabase?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.SetPostgresVersion)]
                = Extension.PostgresVersion.GetHashCode().ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.UseRedshift)]
                = Extension.UseRedshift.GetHashCode().ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.ReverseNullOrdering)]
                = Extension.ReverseNullOrdering.GetHashCode().ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.RemoteCertificateValidationCallback)]
                = (Extension.RemoteCertificateValidationCallback?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.ProvideClientCertificatesCallback)]
                = (Extension.ProvideClientCertificatesCallback?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);

            debugInfo["GaussDB.EntityFrameworkCore.PostgreSQL:" + nameof(GaussDBDbContextOptionsBuilder.ProvidePasswordCallback)]
                = (Extension.ProvidePasswordCallback?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);

            foreach (var rangeDefinition in Extension._userRangeDefinitions)
            {
                debugInfo[
                        "GaussDB.EntityFrameworkCore.PostgreSQL:"
                        + nameof(GaussDBDbContextOptionsBuilder.MapRange)
                        + ":"
                        + rangeDefinition.SubtypeClrType.Name]
                    = rangeDefinition.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    #endregion Infrastructure
}

/// <summary>
///     A definition for a user-defined PostgreSQL range to be mapped.
/// </summary>
public record UserRangeDefinition
{
    /// <summary>
    ///     The name of the PostgreSQL range type to be mapped.
    /// </summary>
    public virtual string RangeName { get; }

    /// <summary>
    ///     The PostgreSQL schema in which the range is defined. If null, the default schema is used
    ///     (which is public unless changed on the model).
    /// </summary>
    public virtual string? SchemaName { get; }

    /// <summary>
    ///     The CLR type of the range's subtype (or element).
    ///     The actual mapped type will be an <see cref="GaussDBRange{T}" /> over this type.
    /// </summary>
    public virtual Type SubtypeClrType { get; }

    /// <summary>
    ///     Optionally, the name of the range's PostgreSQL subtype (or element).
    ///     This is usually not needed - the subtype will be inferred based on <see cref="SubtypeClrType" />.
    /// </summary>
    public virtual string? SubtypeName { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public UserRangeDefinition(
        string rangeName,
        string? schemaName,
        Type subtypeClrType,
        string? subtypeName)
    {
        RangeName = Check.NotEmpty(rangeName, nameof(rangeName));
        SchemaName = schemaName;
        SubtypeClrType = Check.NotNull(subtypeClrType, nameof(subtypeClrType));
        SubtypeName = subtypeName;
    }
}
