namespace GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;

/// <summary>
///     A builder API designed for GaussDB when registering services.
/// </summary>
public class EntityFrameworkGaussDBServicesBuilder : EntityFrameworkRelationalServicesBuilder
{
    private static readonly IDictionary<Type, ServiceCharacteristics> GaussDBServices
        = new Dictionary<Type, ServiceCharacteristics>
        {
            {
                typeof(IGaussDBDataSourceConfigurationPlugin),
                new ServiceCharacteristics(ServiceLifetime.Singleton, multipleRegistrations: true)
            }
        };

    /// <summary>
    ///     Used by relational database providers to create a new <see cref="EntityFrameworkRelationalServicesBuilder" /> for
    ///     registration of provider services.
    /// </summary>
    /// <param name="serviceCollection">The collection to which services will be registered.</param>
    public EntityFrameworkGaussDBServicesBuilder(IServiceCollection serviceCollection)
        : base(serviceCollection)
    {
    }

    /// <summary>
    ///     Gets the <see cref="ServiceCharacteristics" /> for the given service type.
    /// </summary>
    /// <param name="serviceType">The type that defines the service API.</param>
    /// <returns>The <see cref="ServiceCharacteristics" /> for the type or <see langword="null" /> if it's not an EF service.</returns>
    protected override ServiceCharacteristics? TryGetServiceCharacteristics(Type serviceType)
        => GaussDBServices.TryGetValue(serviceType, out var characteristics)
            ? characteristics
            : base.TryGetServiceCharacteristics(serviceType);
}
