using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Transactions;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBRelationalConnection : RelationalConnection, IGaussDBRelationalConnection
{
    private readonly ProvideClientCertificatesCallback? _provideClientCertificatesCallback;
    private readonly RemoteCertificateValidationCallback? _remoteCertificateValidationCallback;

#pragma warning disable CS0618 // ProvidePasswordCallback is obsolete
    private readonly ProvidePasswordCallback? _providePasswordCallback;
#pragma warning restore CS0618

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public DbDataSource? DataSource { get; private set; }

    /// <summary>
    ///     Indicates whether the store connection supports ambient transactions
    /// </summary>
    protected override bool SupportsAmbientTransactions
        => true;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBRelationalConnection(
        RelationalConnectionDependencies dependencies,
        GaussDBDataSourceManager dataSourceManager,
        IDbContextOptions options)
        : this(
            dependencies,
            dataSourceManager.GetDataSource(
                options.FindExtension<GaussDBOptionsExtension>(),
                options.FindExtension<CoreOptionsExtension>()?.ApplicationServiceProvider))
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected GaussDBRelationalConnection(RelationalConnectionDependencies dependencies, DbDataSource? dataSource)
        : base(dependencies)
    {
        if (dataSource is not null)
        {
            DataSource = dataSource;

#if DEBUG
            // We validate in GaussDBOptionsExtensions.Validate that DataSource and these callbacks aren't specified together
            if (dependencies.ContextOptions.FindExtension<GaussDBOptionsExtension>() is { } GaussDBOptions)
            {
                Check.DebugAssert(
                    GaussDBOptions?.ProvideClientCertificatesCallback is null,
                    "Both DataSource and ProvideClientCertificatesCallback are non-null");
                Check.DebugAssert(
                    GaussDBOptions?.RemoteCertificateValidationCallback is null,
                    "Both DataSource and RemoteCertificateValidationCallback are non-null");
                Check.DebugAssert(
                    GaussDBOptions?.ProvidePasswordCallback is null,
                    "Both DataSource and ProvidePasswordCallback are non-null");
            }
#endif
        }
        else if (dependencies.ContextOptions.FindExtension<GaussDBOptionsExtension>() is { } GaussDBOptions)
        {
            _provideClientCertificatesCallback = GaussDBOptions.ProvideClientCertificatesCallback;
            _remoteCertificateValidationCallback = GaussDBOptions.RemoteCertificateValidationCallback;
            _providePasswordCallback = GaussDBOptions.ProvidePasswordCallback;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override DbConnection CreateDbConnection()
    {
        if (DataSource is not null)
        {
            return DataSource.CreateConnection();
        }

        var conn = new GaussDBConnection(ConnectionString);

        if (_provideClientCertificatesCallback is not null || _remoteCertificateValidationCallback is not null)
        {
            conn.SslClientAuthenticationOptionsCallback = o =>
            {
                if (_provideClientCertificatesCallback is not null)
                {
                    o.ClientCertificates ??= new();
                    _provideClientCertificatesCallback(o.ClientCertificates);
                }

                o.RemoteCertificateValidationCallback = _remoteCertificateValidationCallback;
            };
        }

        if (_providePasswordCallback is not null)
        {
#pragma warning disable 618 // ProvidePasswordCallback is obsolete
            conn.ProvidePasswordCallback = _providePasswordCallback;
#pragma warning restore 618
        }

        return conn;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    // TODO: Remove after DbDataSource support is added to EF Core (https://github.com/dotnet/efcore/issues/28266)
    public override string? ConnectionString
    {
        get => DataSource is null ? base.ConnectionString : DataSource.ConnectionString;
        set
        {
            base.ConnectionString = value;

            DataSource = null;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    [AllowNull]
    public new virtual GaussDBConnection DbConnection
    {
        get => (GaussDBConnection)base.DbConnection;
        set
        {
            base.DbConnection = value;

            DataSource = null;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual DbDataSource? DbDataSource
    {
        get => DataSource;
        set
        {
            DbConnection = null;
            ConnectionString = null;
            DataSource = value;
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual IGaussDBRelationalConnection CreateAdminConnection()
    {
        if (Dependencies.ContextOptions.FindExtension<GaussDBOptionsExtension>() is not { } GaussDBOptions)
        {
            throw new InvalidOperationException($"{nameof(GaussDBOptionsExtension)} not found in {nameof(CreateAdminConnection)}");
        }

        var adminConnectionString = new GaussDBConnectionStringBuilder(ConnectionString)
        {
            Database = GaussDBOptions.AdminDatabase ?? "postgres",
            Pooling = false,
            Multiplexing = false
        }.ToString();

        var adminGaussDBOptions = DataSource is not null
            ? GaussDBOptions.WithConnection(((GaussDBConnection)CreateDbConnection()).CloneWith(adminConnectionString))
            : GaussDBOptions.Connection is not null
                ? GaussDBOptions.WithConnection(DbConnection.CloneWith(adminConnectionString))
                : GaussDBOptions.WithConnectionString(adminConnectionString);

        var optionsBuilder = new DbContextOptionsBuilder();
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(adminGaussDBOptions);

        return new GaussDBRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options }, dataSource: null);
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    // Accessing Transaction.Current is expensive, so don't do it if Enlist is false in the connection string
    public override Transaction? CurrentAmbientTransaction
        => ConnectionString is null || !ConnectionString.Contains("Enlist=false", StringComparison.InvariantCultureIgnoreCase)
            ? Transaction.Current
            : null;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual async ValueTask<IGaussDBRelationalConnection> CloneWith(
        string connectionString,
        bool async,
        CancellationToken cancellationToken = default)
    {
        var clonedDbConnection = async
            ? await DbConnection.CloneWithAsync(connectionString, cancellationToken).ConfigureAwait(false)
            : DbConnection.CloneWith(connectionString);

        var relationalOptions = RelationalOptionsExtension.Extract(Dependencies.ContextOptions)
            .WithConnectionString(null)
            .WithConnection(clonedDbConnection);

        var optionsBuilder = new DbContextOptionsBuilder();
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(relationalOptions);

        return new GaussDBRelationalConnection(Dependencies with { ContextOptions = optionsBuilder.Options }, dataSource: null);
    }
}
