using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;

/// <summary>
///     A convention that manipulates names of database objects for entity types that share a table to avoid clashes.
/// </summary>
public class GaussDBSharedTableConvention : SharedTableConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="GaussDBSharedTableConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    /// <param name="relationalDependencies">Parameter object containing relational dependencies for this convention.</param>
    public GaussDBSharedTableConvention(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc />
    protected override bool AreCompatible(IReadOnlyIndex index, IReadOnlyIndex duplicateIndex, in StoreObjectIdentifier storeObject)
        => base.AreCompatible(index, duplicateIndex, storeObject)
            && index.AreCompatibleForGaussDB(duplicateIndex, storeObject, shouldThrow: false);

    /// <inheritdoc />
    protected override bool CheckConstraintsUniqueAcrossTables
        => false;
}
