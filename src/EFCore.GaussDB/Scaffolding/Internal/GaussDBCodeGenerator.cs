using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Scaffolding.Internal;

/// <summary>
///     The default code generator for GaussDB.
/// </summary>
public class GaussDBCodeGenerator : ProviderCodeGenerator
{
    private static readonly MethodInfo _useGaussDBMethodInfo
        = typeof(GaussDBDbContextOptionsBuilderExtensions).GetRequiredRuntimeMethod(
            nameof(GaussDBDbContextOptionsBuilderExtensions.UseGaussDB),
            typeof(DbContextOptionsBuilder),
            typeof(string),
            typeof(Action<GaussDBDbContextOptionsBuilder>));

    /// <summary>
    ///     Constructs an instance of the <see cref="GaussDBCodeGenerator" /> class.
    /// </summary>
    /// <param name="dependencies">The dependencies.</param>
    public GaussDBCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment? providerOptions)
        => new(
            _useGaussDBMethodInfo,
            providerOptions is null
                ? [connectionString]
                : [connectionString, new NestedClosureCodeFragment("x", providerOptions)]);
}
