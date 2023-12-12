using GaussDB.EntityFrameworkCore.PostgreSQL.Diagnostics.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;
using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;
using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Migrations;
using GaussDB.EntityFrameworkCore.PostgreSQL.Migrations.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Query;
using GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Query.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Update.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods to configure Entity Framework Core for GaussDB.
/// </summary>
// ReSharper disable once UnusedMember.Global
public static class GaussDBServiceCollectionExtensions
{
    /// <summary>
    ///     <para>
    ///         Registers the given Entity Framework context as a service in the <see cref="IServiceCollection" />
    ///         and configures it to connect to a PostgreSQL database.
    ///     </para>
    ///     <para>
    ///         Use this method when using dependency injection in your application, such as with ASP.NET Core.
    ///         For applications that don't use dependency injection, consider creating <see cref="DbContext" />
    ///         instances directly with its constructor. The <see cref="DbContext.OnConfiguring" /> method can then be
    ///         overridden to configure the SQL Server provider and connection string.
    ///     </para>
    ///     <para>
    ///         To configure the <see cref="DbContextOptions{TContext}" /> for the context, either override the
    ///         <see cref="DbContext.OnConfiguring" /> method in your derived context, or supply
    ///         an optional action to configure the <see cref="DbContextOptions" /> for the context.
    ///     </para>
    ///     <para>
    ///         For more information on how to use this method, see the Entity Framework Core documentation at https://aka.ms/efdocs.
    ///         For more information on using dependency injection, see https://go.microsoft.com/fwlink/?LinkId=526890.
    ///     </para>
    /// </summary>
    /// <typeparam name="TContext"> The type of context to be registered. </typeparam>
    /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
    /// <param name="connectionString"> The connection string of the database to connect to. </param>
    /// <param name="GaussDBOptionsAction"> An optional action to allow additional SQL Server specific configuration. </param>
    /// <param name="optionsAction"> An optional action to configure the <see cref="DbContextOptions" /> for the context. </param>
    /// <returns> The same service collection so that multiple calls can be chained. </returns>
    public static IServiceCollection AddGaussDB<TContext>(
        this IServiceCollection serviceCollection,
        string? connectionString,
        Action<GaussDBDbContextOptionsBuilder>? GaussDBOptionsAction = null,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        return serviceCollection.AddDbContext<TContext>(
            (_, options) =>
            {
                optionsAction?.Invoke(options);
                options.UseGaussDB(connectionString, GaussDBOptionsAction);
            });
    }

    /// <summary>
    ///     <para>
    ///         Adds the services required by the GaussDB database provider for Entity Framework
    ///         to an <see cref="IServiceCollection" />.
    ///     </para>
    ///     <para>
    ///         Calling this method is no longer necessary when building most applications, including those that
    ///         use dependency injection in ASP.NET or elsewhere.
    ///         It is only needed when building the internal service provider for use with
    ///         the <see cref="DbContextOptionsBuilder.UseInternalServiceProvider" /> method.
    ///         This is not recommend other than for some advanced scenarios.
    ///     </para>
    /// </summary>
    /// <param name="serviceCollection"> The <see cref="IServiceCollection" /> to add services to. </param>
    /// <returns>
    ///     The same service collection so that multiple calls can be chained.
    /// </returns>
    public static IServiceCollection AddEntityFrameworkGaussDB(this IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<LoggingDefinitions, GaussDBLoggingDefinitions>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<GaussDBOptionsExtension>>()
            .TryAdd<IValueGeneratorCache>(p => p.GetRequiredService<IGaussDBValueGeneratorCache>())
            .TryAdd<IRelationalTypeMappingSource, GaussDBTypeMappingSource>()
            .TryAdd<ISqlGenerationHelper, GaussDBSqlGenerationHelper>()
            .TryAdd<IRelationalAnnotationProvider, GaussDBAnnotationProvider>()
            .TryAdd<IModelValidator, GaussDBModelValidator>()
            .TryAdd<IMigrator, GaussDBMigrator>()
            .TryAdd<IProviderConventionSetBuilder, GaussDBConventionSetBuilder>()
            .TryAdd<IUpdateSqlGenerator, GaussDBUpdateSqlGenerator>()
            .TryAdd<IModificationCommandFactory, GaussDBModificationCommandFactory>()
            .TryAdd<IModificationCommandBatchFactory, GaussDBModificationCommandBatchFactory>()
            .TryAdd<IValueGeneratorSelector, GaussDBValueGeneratorSelector>()
            .TryAdd<IRelationalConnection>(p => p.GetRequiredService<IGaussDBRelationalConnection>())
            .TryAdd<IMigrationsSqlGenerator, GaussDBMigrationsSqlGenerator>()
            .TryAdd<IRelationalDatabaseCreator, GaussDBDatabaseCreator>()
            .TryAdd<IHistoryRepository, GaussDBHistoryRepository>()
            .TryAdd<ICompiledQueryCacheKeyGenerator, GaussDBCompiledQueryCacheKeyGenerator>()
            .TryAdd<IExecutionStrategyFactory, GaussDBExecutionStrategyFactory>()
            .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, GaussDBQueryableMethodTranslatingExpressionVisitorFactory>()
            .TryAdd<IMethodCallTranslatorProvider, GaussDBMethodCallTranslatorProvider>()
            .TryAdd<IAggregateMethodCallTranslatorProvider, GaussDBAggregateMethodCallTranslatorProvider>()
            .TryAdd<IMemberTranslatorProvider, GaussDBMemberTranslatorProvider>()
            .TryAdd<IEvaluatableExpressionFilter, GaussDBEvaluatableExpressionFilter>()
            .TryAdd<IQuerySqlGeneratorFactory, GaussDBQuerySqlGeneratorFactory>()
            .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, GaussDBSqlTranslatingExpressionVisitorFactory>()
            .TryAdd<IQueryTranslationPreprocessorFactory, GaussDBQueryTranslationPreprocessorFactory>()
            .TryAdd<IQueryTranslationPostprocessorFactory, GaussDBQueryTranslationPostprocessorFactory>()
            .TryAdd<IRelationalParameterBasedSqlProcessorFactory, GaussDBParameterBasedSqlProcessorFactory>()
            .TryAdd<ISqlExpressionFactory, GaussDBSqlExpressionFactory>()
            .TryAdd<ISingletonOptions, IGaussDBSingletonOptions>(p => p.GetRequiredService<IGaussDBSingletonOptions>())
            .TryAdd<IQueryCompilationContextFactory, GaussDBQueryCompilationContextFactory>()
            .TryAddProviderSpecificServices(
                b => b
                    .TryAddSingleton<IGaussDBValueGeneratorCache, GaussDBValueGeneratorCache>()
                    .TryAddSingleton<IGaussDBSingletonOptions, GaussDBSingletonOptions>()
                    .TryAddSingleton<IGaussDBSequenceValueGeneratorFactory, GaussDBSequenceValueGeneratorFactory>()
                    .TryAddScoped<IGaussDBRelationalConnection, GaussDBRelationalConnection>())
            .TryAddCoreServices();

        return serviceCollection;
    }
}
