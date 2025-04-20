using System.Collections.Concurrent;
using System.Data.Common;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal;

/// <summary>
///     Manages resolving and creating <see cref="GaussDBDataSource" /> instances.
/// </summary>
/// <remarks>
///     <para>
///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///         the same compatibility standards as public APIs. It may be changed or removed without notice in
///         any release. You should only use it directly in your code with extreme caution and knowing that
///         doing so can result in application failures when updating to a new Entity Framework Core release.
///     </para>
///     <para>
///         The service lifetime is <see cref="ServiceLifetime.Singleton" />. This means a single instance
///         is used by many <see cref="DbContext" /> instances. The implementation must be thread-safe.
///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
///     </para>
///     <para>
///         See <see href="https://aka.ms/efcore-docs-providers">Implementation of database providers and extensions</see>
///         for more information and examples.
///     </para>
/// </remarks>
public class GaussDBDataSourceManager : IDisposable, IAsyncDisposable
{
    private readonly IEnumerable<IGaussDBDataSourceConfigurationPlugin> _plugins;
    private readonly ConcurrentDictionary<string, GaussDBDataSource> _dataSources = new();
    private volatile int _isDisposed;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBDataSourceManager(IEnumerable<IGaussDBDataSourceConfigurationPlugin> plugins)
        => _plugins = plugins.ToArray();

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual DbDataSource? GetDataSource(GaussDBOptionsExtension? GaussDBOptionsExtension, IServiceProvider? applicationServiceProvider)
        => GaussDBOptionsExtension switch
        {
            // If the user has explicitly passed in a data source via UseGaussDB(), use that.
            // Note that in this case, the data source is scoped (not singleton), and so can change between different
            // DbContext instances using the same internal service provider.
            { DataSource: DbDataSource dataSource }
                => GaussDBOptionsExtension.DataSourceBuilderAction is null
                    ? dataSource
                    // If the user has explicitly passed in a data source via UseGaussDB(), but also supplied a data source configuration
                    // lambda, throw - we're unable to apply the configuration lambda to the externally-provided, already-built data source.
                    : throw new NotSupportedException(GaussDBStrings.DataSourceAndConfigNotSupported),

            // If the user has passed in a DbConnection, never use a data source - even if e.g. MapEnum() was called.
            // This is to avoid blocking and allow continuing using enums in conjunction with DbConnections (which
            // must be manually set up by the user for the enum, of course).
            { Connection: not null } => null,

            // If the user hasn't configured anything in UseGaussDB (no data source, no connection, no connection string), check the
            // application service provider to see if a data source is registered there, and return that.
            { ConnectionString: null } when applicationServiceProvider?.GetService<GaussDBDataSource>() is DbDataSource dataSource
                => dataSource,

            // Otherwise if there's no connection string, abort: a connection string is required to create a data source in any case.
            { ConnectionString: null } or null => null,

            // The following are features which require an GaussDBDataSource, since they require configuration on GaussDBDataSourceBuilder.
            { DataSourceBuilderAction: not null } => GetSingletonDataSource(GaussDBOptionsExtension),
            { EnumDefinitions.Count: > 0 } => GetSingletonDataSource(GaussDBOptionsExtension),
            _ when _plugins.Any() => GetSingletonDataSource(GaussDBOptionsExtension),

            // If there's no configured feature which requires us to use a data source internally, don't use one; this causes
            // GaussDBRelationalConnection to use the connection string as before (no data source), allowing switching connection strings
            // with the same service provider etc.
            _ => null
        };

    private DbDataSource GetSingletonDataSource(GaussDBOptionsExtension GaussDBOptionsExtension)
    {
        var connectionString = GaussDBOptionsExtension.ConnectionString;
        Check.DebugAssert(connectionString is not null, "Connection string can't be null");

        if (_dataSources.TryGetValue(connectionString, out var dataSource))
        {
            return dataSource;
        }

        var newDataSource = CreateDataSource(GaussDBOptionsExtension);

        var addedDataSource = _dataSources.GetOrAdd(connectionString, newDataSource);
        if (!ReferenceEquals(addedDataSource, newDataSource))
        {
            newDataSource.Dispose();
        }
        else if (_isDisposed == 1)
        {
            newDataSource.Dispose();
            throw new ObjectDisposedException(nameof(GaussDBDataSourceManager));
        }

        return addedDataSource;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected virtual GaussDBDataSource CreateDataSource(GaussDBOptionsExtension GaussDBOptionsExtension)
    {
        var dataSourceBuilder = new GaussDBDataSourceBuilder(GaussDBOptionsExtension.ConnectionString);

        foreach (var enumDefinition in GaussDBOptionsExtension.EnumDefinitions)
        {
            dataSourceBuilder.MapEnum(
                enumDefinition.ClrType,
                enumDefinition.StoreTypeSchema is null
                    ? enumDefinition.StoreTypeName
                    : enumDefinition.StoreTypeSchema + "." + enumDefinition.StoreTypeName,
                enumDefinition.NameTranslator);
        }

        foreach (var plugin in _plugins)
        {
            plugin.Configure(dataSourceBuilder);
        }

        // Legacy authentication-related callbacks at the EF level; apply these when building a data source as well.
        if (GaussDBOptionsExtension.ProvideClientCertificatesCallback is not null
            || GaussDBOptionsExtension.RemoteCertificateValidationCallback is not null)
        {
            dataSourceBuilder.UseSslClientAuthenticationOptionsCallback(o =>
            {
                if (GaussDBOptionsExtension.ProvideClientCertificatesCallback is not null)
                {
                    o.ClientCertificates ??= new();
                    GaussDBOptionsExtension.ProvideClientCertificatesCallback(o.ClientCertificates);
                }

                o.RemoteCertificateValidationCallback = GaussDBOptionsExtension.RemoteCertificateValidationCallback;
            });
        }

        // Finally, if the user has provided a data source builder configuration action, invoke it.
        // Do this last, to allow the user to override anything set above.
        GaussDBOptionsExtension.DataSourceBuilderAction?.Invoke(dataSourceBuilder);

        return dataSourceBuilder.Build();
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            foreach (var dataSource in _dataSources.Values)
            {
                dataSource.Dispose();
            }
        }
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            foreach (var dataSource in _dataSources.Values)
            {
                await dataSource.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
