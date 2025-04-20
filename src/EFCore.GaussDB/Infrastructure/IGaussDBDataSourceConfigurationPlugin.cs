namespace GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;

/// <summary>
///     Represents a plugin that configures an <see cref="GaussDBDataSource" /> via <see cref="GaussDBDataSourceBuilder" />.
/// </summary>
/// <remarks>
///     <para>
///         The service lifetime is <see cref="ServiceLifetime.Singleton" /> and multiple registrations
///         are allowed. This means a single instance of each service is used by many <see cref="DbContext" />
///         instances. The implementation must be thread-safe.
///         This service cannot depend on services registered as <see cref="ServiceLifetime.Scoped" />.
///     </para>
///     <para>
///         See <see href="https://aka.ms/efcore-docs-providers">Implementation of database providers and extensions</see>
///         for more information and examples.
///     </para>
/// </remarks>
public interface IGaussDBDataSourceConfigurationPlugin
{
    /// <summary>
    ///     Applies the plugin configuration on the given <paramref name="GaussDBDataSourceBuilder" />.
    /// </summary>
    void Configure(GaussDBDataSourceBuilder GaussDBDataSourceBuilder);
}
