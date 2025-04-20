using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;

/// <summary>
///     A builder for building conventions for GaussDB.
/// </summary>
/// <remarks>
///     <para>
///         The service lifetime is <see cref="ServiceLifetime.Scoped" /> and multiple registrations are allowed. This means that each
///         <see cref="DbContext" /> instance will use its own set of instances of this service. The implementations may depend on other
///         services registered with any lifetime. The implementations do not need to be thread-safe.
///     </para>
///     <para>
///         See <see href="https://aka.ms/efcore-docs-conventions">Model building conventions</see>, and
///     </para>
/// </remarks>
public class GaussDBConventionSetBuilder : RelationalConventionSetBuilder
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly Version _postgresVersion;
    private readonly IReadOnlyList<EnumDefinition> _enumDefinitions;

    /// <summary>
    ///     Creates a new <see cref="GaussDBConventionSetBuilder" /> instance.
    /// </summary>
    /// <param name="dependencies">The core dependencies for this service.</param>
    /// <param name="relationalDependencies">The relational dependencies for this service.</param>
    /// <param name="typeMappingSource">The type mapping source to use.</param>
    /// <param name="GaussDBSingletonOptions">The singleton options to use.</param>
    public GaussDBConventionSetBuilder(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies,
        IRelationalTypeMappingSource typeMappingSource,
        IGaussDBSingletonOptions GaussDBSingletonOptions)
        : base(dependencies, relationalDependencies)
    {
        _typeMappingSource = typeMappingSource;
        _postgresVersion = GaussDBSingletonOptions.PostgresVersion;
        _enumDefinitions = GaussDBSingletonOptions.EnumDefinitions;
    }

    /// <inheritdoc />
    public override ConventionSet CreateConventionSet()
    {
        var conventionSet = base.CreateConventionSet();

        var valueGenerationStrategyConvention =
            new GaussDBValueGenerationStrategyConvention(Dependencies, RelationalDependencies, _postgresVersion);
        conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);
        conventionSet.ModelInitializedConventions.Add(
            new RelationalMaxIdentifierLengthConvention(63, Dependencies, RelationalDependencies));

        conventionSet.PropertyAddedConventions.Add(new GaussDBJsonElementHackConvention());

        ValueGenerationConvention valueGenerationConvention = new GaussDBValueGenerationConvention(Dependencies, RelationalDependencies);
        ReplaceConvention(conventionSet.EntityTypeBaseTypeChangedConventions, valueGenerationConvention);

        ReplaceConvention(
            conventionSet.EntityTypeAnnotationChangedConventions, (RelationalValueGenerationConvention)valueGenerationConvention);

        ReplaceConvention(conventionSet.EntityTypePrimaryKeyChangedConventions, valueGenerationConvention);

        ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGenerationConvention);

        ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGenerationConvention);

        var storeGenerationConvention =
            new GaussDBStoreGenerationConvention(Dependencies, RelationalDependencies);
        ReplaceConvention(conventionSet.PropertyAnnotationChangedConventions, storeGenerationConvention);
        ReplaceConvention(
            conventionSet.PropertyAnnotationChangedConventions, (RelationalValueGenerationConvention)valueGenerationConvention);

        conventionSet.ModelFinalizingConventions.Add(valueGenerationStrategyConvention);
        conventionSet.ModelFinalizingConventions.Add(new GaussDBPostgresModelFinalizingConvention(_typeMappingSource, _enumDefinitions));
        ReplaceConvention(conventionSet.ModelFinalizingConventions, storeGenerationConvention);
        ReplaceConvention(
            conventionSet.ModelFinalizingConventions,
            (SharedTableConvention)new GaussDBSharedTableConvention(Dependencies, RelationalDependencies));

        ReplaceConvention(
            conventionSet.ModelFinalizedConventions,
            (RuntimeModelConvention)new GaussDBRuntimeModelConvention(Dependencies, RelationalDependencies));

        return conventionSet;
    }

    /// <summary>
    ///     <para>
    ///         Call this method to build a <see cref="ConventionSet" /> for GaussDB when using
    ///         the <see cref="ModelBuilder" /> outside of <see cref="DbContext.OnModelCreating" />.
    ///     </para>
    ///     <para>
    ///         Note that it is unusual to use this method.
    ///         Consider using <see cref="DbContext" /> in the normal way instead.
    ///     </para>
    /// </summary>
    /// <returns> The convention set. </returns>
    public static ConventionSet Build()
    {
        using var serviceScope = CreateServiceScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
        return ConventionSet.CreateConventionSet(context);
    }

    /// <summary>
    ///     <para>
    ///         Call this method to build a <see cref="ModelBuilder" /> for GaussDB outside of <see cref="DbContext.OnModelCreating" />.
    ///     </para>
    ///     <para>
    ///         Note that it is unusual to use this method.
    ///         Consider using <see cref="DbContext" /> in the normal way instead.
    ///     </para>
    /// </summary>
    /// <returns> The convention set. </returns>
    public static ModelBuilder CreateModelBuilder()
    {
        using var serviceScope = CreateServiceScope();
        using var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
        return new ModelBuilder(ConventionSet.CreateConventionSet(context), context.GetService<ModelDependencies>());
    }

    private static IServiceScope CreateServiceScope()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkGaussDB()
            .AddDbContext<DbContext>(
                (p, o) =>
                    o.UseGaussDB("Server=.")
                        .UseInternalServiceProvider(p))
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
    }
}
