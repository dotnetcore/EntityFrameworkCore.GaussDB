using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage;

internal static class RelationalTypeMappingExtensions
{
    internal static string GenerateEmbeddedSqlLiteral(this RelationalTypeMapping mapping, object? value)
        => mapping is GaussDBTypeMapping GaussDBTypeMapping
            ? GaussDBTypeMapping.GenerateEmbeddedSqlLiteral(value)
            : mapping.GenerateSqlLiteral(value);
}
