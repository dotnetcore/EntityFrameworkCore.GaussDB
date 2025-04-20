using GaussDB.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

/// <summary>
///     A composite member translator that dispatches to multiple specialized member translators specific to GaussDB.
/// </summary>
public class GaussDBMemberTranslatorProvider : RelationalMemberTranslatorProvider
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public virtual GaussDBJsonPocoTranslator JsonPocoTranslator { get; }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBMemberTranslatorProvider(
        RelationalMemberTranslatorProviderDependencies dependencies,
        IModel model,
        IRelationalTypeMappingSource typeMappingSource,
        IDbContextOptions contextOptions)
        : base(dependencies)
    {
        var GaussDBOptions = contextOptions.FindExtension<GaussDBOptionsExtension>() ?? new GaussDBOptionsExtension();
        var supportsMultiranges = GaussDBOptions.PostgresVersion.AtLeast(14);

        var sqlExpressionFactory = (GaussDBSqlExpressionFactory)dependencies.SqlExpressionFactory;
        JsonPocoTranslator = new GaussDBJsonPocoTranslator(typeMappingSource, sqlExpressionFactory, model);

        AddTranslators(
        [
            new GaussDBBigIntegerMemberTranslator(sqlExpressionFactory),
                new GaussDBDateTimeMemberTranslator(typeMappingSource, sqlExpressionFactory),
                new GaussDBJsonDomTranslator(typeMappingSource, sqlExpressionFactory, model),
                new GaussDBLTreeTranslator(typeMappingSource, sqlExpressionFactory, model),
                JsonPocoTranslator,
                new GaussDBRangeTranslator(typeMappingSource, sqlExpressionFactory, model, supportsMultiranges),
                new GaussDBStringMemberTranslator(sqlExpressionFactory),
                new GaussDBTimeSpanMemberTranslator(sqlExpressionFactory)
        ]);
    }
}
