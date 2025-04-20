using GaussDB.EntityFrameworkCore.PostgreSQL.Storage.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.ValueGeneration.Internal;

/// <summary>
///     This API supports the Entity Framework Core infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
public interface IGaussDBSequenceValueGeneratorFactory
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    ValueGenerator? TryCreate(
        IProperty property,
        Type clrType,
        GaussDBSequenceValueGeneratorState generatorState,
        IGaussDBRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder,
        IRelationalCommandDiagnosticsLogger commandLogger);
}
