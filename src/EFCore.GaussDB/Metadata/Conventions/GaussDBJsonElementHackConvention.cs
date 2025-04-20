using System.Text.Json;
using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;

/// <summary>
///     This convention is a hack around https://github.com/dotnet/efcore/issues/32192. To support the EF owned entity JSON support, EF
///     requires
///     that a lookup of the CLR type <see cref="JsonElement" /> return the provider's special <see cref="JsonTypeMapping" />. But GaussDB has
///     its own JSON DOM support, where actually mapping <see cref="JsonElement" /> is allowed as a weakly-typed mapping strategy. The two
///     JSON type mappings are incompatible notably because EF's <see cref="JsonTypeMapping" /> is expected to return UTF8 byte data which is
///     then parsed via <see cref="Utf8JsonWriter" /> (and not a string). So for properties actually typed as <see cref="JsonElement" />, we
///     hack here and set the type mapping rather than going through the regular type mapping process.
/// </summary>
public class GaussDBJsonElementHackConvention : IPropertyAddedConvention
{
    private GaussDBJsonTypeMapping? _jsonTypeMapping;

    /// <inheritdoc />
    public void ProcessPropertyAdded(IConventionPropertyBuilder propertyBuilder, IConventionContext<IConventionPropertyBuilder> context)
    {
        var property = propertyBuilder.Metadata;

        if (property.ClrType.UnwrapNullableType() == typeof(JsonElement) && property.GetColumnType() is null)
        {
            property.SetTypeMapping(_jsonTypeMapping ??= new GaussDBJsonTypeMapping("jsonb", typeof(JsonElement)));
        }
    }
}
