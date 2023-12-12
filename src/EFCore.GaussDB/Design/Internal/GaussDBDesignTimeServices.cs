using Microsoft.EntityFrameworkCore.Design.Internal;
using GaussDB.EntityFrameworkCore.PostgreSQL.Scaffolding.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Design.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBDesignTimeServices : IDesignTimeServices
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
    {
        Check.NotNull(serviceCollection, nameof(serviceCollection));

        serviceCollection.AddEntityFrameworkGaussDB();
#pragma warning disable EF1001 // Internal EF Core API usage.
        new EntityFrameworkRelationalDesignServicesBuilder(serviceCollection)
            .TryAdd<ICSharpRuntimeAnnotationCodeGenerator, GaussDBCSharpRuntimeAnnotationCodeGenerator>()
#pragma warning restore EF1001 // Internal EF Core API usage.
            .TryAdd<IAnnotationCodeGenerator, GaussDBAnnotationCodeGenerator>()
            .TryAdd<IDatabaseModelFactory, GaussDBDatabaseModelFactory>()
            .TryAdd<IProviderConfigurationCodeGenerator, GaussDBCodeGenerator>()
            .TryAddCoreServices();
    }
}
