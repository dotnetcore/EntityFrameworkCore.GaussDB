using System.Data.Common;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Provides extension methods on <see cref="DbContextOptionsBuilder" /> and <see cref="DbContextOptionsBuilder{T}" />
///     used to configure a <see cref="DbContext" /> to context to a PostgreSQL database with GaussDB.
/// </summary>
public static class GaussDBDbContextOptionsBuilderExtensions
{
    /// <summary>
    ///     <para>
    ///         Configures the context to connect to a PostgreSQL server with GaussDB, but without initially setting any
    ///         <see cref="DbConnection" /> or connection string.
    ///     </para>
    ///     <para>
    ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
    ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
    ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
    ///     </para>
    /// </summary>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder UseGaussDB(
        this DbContextOptionsBuilder optionsBuilder,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(GetOrCreateExtension(optionsBuilder));

        ConfigureWarnings(optionsBuilder);

        GaussDBOptionsAction?.Invoke(new GaussDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="connectionString">The connection string of the database to connect to.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder UseGaussDB(
        this DbContextOptionsBuilder optionsBuilder,
        string? connectionString,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));

        var extension = (GaussDBOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        ConfigureWarnings(optionsBuilder);

        GaussDBOptionsAction?.Invoke(new GaussDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="connection">
    ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
    ///     in the open state then EF will not open or close the connection. If the connection is in the closed
    ///     state then EF will open and close the connection as needed. The caller owns the connection and is
    ///     responsible for its disposal.
    /// </param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder UseGaussDB(
        this DbContextOptionsBuilder optionsBuilder,
        DbConnection connection,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        => UseGaussDB(optionsBuilder, connection, contextOwnsConnection: false, GaussDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="connection">
    ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
    ///     in the open state then EF will not open or close the connection. If the connection is in the closed
    ///     state then EF will open and close the connection as needed.
    /// </param>
    /// <param name="contextOwnsConnection">
    ///     If <see langword="true" />, then EF will take ownership of the connection and will
    ///     dispose it in the same way it would dispose a connection created by EF. If <see langword="false" />, then the caller still
    ///     owns the connection and is responsible for its disposal.
    /// </param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder UseGaussDB(
        this DbContextOptionsBuilder optionsBuilder,
        DbConnection connection,
        bool contextOwnsConnection,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));
        Check.NotNull(connection, nameof(connection));

        var extension = (GaussDBOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection, contextOwnsConnection);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        ConfigureWarnings(optionsBuilder);

        GaussDBOptionsAction?.Invoke(new GaussDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="dataSource">A <see cref="DbDataSource" /> which will be used to get database connections.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder UseGaussDB(
        this DbContextOptionsBuilder optionsBuilder,
        DbDataSource dataSource,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
    {
        Check.NotNull(optionsBuilder, nameof(optionsBuilder));
        Check.NotNull(dataSource, nameof(dataSource));

        var extension = (GaussDBOptionsExtension)GetOrCreateExtension(optionsBuilder).WithDataSource(dataSource);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        ConfigureWarnings(optionsBuilder);

        GaussDBOptionsAction?.Invoke(new GaussDBDbContextOptionsBuilder(optionsBuilder));

        return optionsBuilder;
    }

    /// <summary>
    ///     <para>
    ///         Configures the context to connect to a PostgreSQL server with GaussDB, but without initially setting any
    ///         <see cref="DbConnection" />, <see cref="DbDataSource" /> or connection string.
    ///     </para>
    ///     <para>
    ///         The connection, data source or connection string must be set explicitly or registered in the DI
    ///         before the <see cref="DbContext" /> is used to connect to a database.
    ///         Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />, a data source using
    ///         <see cref="GaussDBDatabaseFacadeExtensions.SetDbDataSource" />, or a connection string using
    ///         <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
    ///     </para>
    /// </summary>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder<TContext> UseGaussDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseGaussDB(
            (DbContextOptionsBuilder)optionsBuilder, GaussDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="connectionString">The connection string of the database to connect to.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder<TContext> UseGaussDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        string? connectionString,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseGaussDB(
            (DbContextOptionsBuilder)optionsBuilder, connectionString, GaussDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="connection">
    ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
    ///     in the open state then EF will not open or close the connection. If the connection is in the closed
    ///     state then EF will open and close the connection as needed. The caller owns the connection and is
    ///     responsible for its disposal.
    /// </param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder<TContext> UseGaussDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        DbConnection connection,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseGaussDB(
            (DbContextOptionsBuilder)optionsBuilder, connection, GaussDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be configured.</typeparam>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="connection">
    ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
    ///     in the open state then EF will not open or close the connection. If the connection is in the closed
    ///     state then EF will open and close the connection as needed.
    /// </param>
    /// <param name="contextOwnsConnection">
    ///     If <see langword="true" />, then EF will take ownership of the connection and will
    ///     dispose it in the same way it would dispose a connection created by EF. If <see langword="false" />, then the caller still
    ///     owns the connection and is responsible for its disposal.
    /// </param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder<TContext> UseGaussDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        DbConnection connection,
        bool contextOwnsConnection,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseGaussDB(
            (DbContextOptionsBuilder)optionsBuilder, connection, contextOwnsConnection, GaussDBOptionsAction);

    /// <summary>
    ///     Configures the context to connect to a PostgreSQL database with GaussDB.
    /// </summary>
    /// <param name="optionsBuilder">A builder for setting options on the context.</param>
    /// <param name="dataSource">A <see cref="DbDataSource" /> which will be used to get database connections.</param>
    /// <param name="GaussDBOptionsAction">An optional action to allow additional GaussDB-specific configuration.</param>
    /// <returns>
    ///     The options builder so that further configuration can be chained.
    /// </returns>
    public static DbContextOptionsBuilder<TContext> UseGaussDB<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder,
        DbDataSource dataSource,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null)
        where TContext : DbContext
        => (DbContextOptionsBuilder<TContext>)UseGaussDB(
            (DbContextOptionsBuilder)optionsBuilder, dataSource, GaussDBOptionsAction);

    /// <summary>
    ///     Returns an existing instance of <see cref="GaussDBOptionsExtension" />, or a new instance if one does not exist.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder" /> to search.</param>
    /// <returns>
    ///     An existing instance of <see cref="GaussDBOptionsExtension" />, or a new instance if one does not exist.
    /// </returns>
    private static GaussDBOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<GaussDBOptionsExtension>() is { } existing
            ? new GaussDBOptionsExtension(existing)
            : new GaussDBOptionsExtension();

    private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
    {
        var coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
            ?? new CoreOptionsExtension();

        coreOptionsExtension = RelationalOptionsExtension.WithDefaultWarningConfiguration(coreOptionsExtension);

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
    }
}
