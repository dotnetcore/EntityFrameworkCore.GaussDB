using GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Internal;

namespace GaussDB.EntityFrameworkCore.PostgreSQL.Metadata.Conventions;

/// <summary>
///     A convention that creates an optimized copy of the mutable model.
/// </summary>
public class GaussDBRuntimeModelConvention : RelationalRuntimeModelConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="GaussDBRuntimeModelConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    /// <param name="relationalDependencies">Parameter object containing relational dependencies for this convention.</param>
    public GaussDBRuntimeModelConvention(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc />
    protected override void ProcessModelAnnotations(
        Dictionary<string, object?> annotations,
        IModel model,
        RuntimeModel runtimeModel,
        bool runtime)
    {
        base.ProcessModelAnnotations(annotations, model, runtimeModel, runtime);

        if (!runtime)
        {
            annotations.Remove(GaussDBAnnotationNames.DatabaseTemplate);
            annotations.Remove(GaussDBAnnotationNames.Tablespace);
            annotations.Remove(GaussDBAnnotationNames.CollationDefinitionPrefix);

#pragma warning disable CS0618
            annotations.Remove(GaussDBAnnotationNames.DefaultColumnCollation);
#pragma warning restore CS0618

            foreach (var annotationName in annotations.Keys.Where(
                         k =>
                             k.StartsWith(GaussDBAnnotationNames.PostgresExtensionPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.EnumPrefix, StringComparison.Ordinal)
                             || k.StartsWith(GaussDBAnnotationNames.RangePrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }
    }

    /// <inheritdoc />
    protected override void ProcessEntityTypeAnnotations(
        Dictionary<string, object?> annotations,
        IEntityType entityType,
        RuntimeEntityType runtimeEntityType,
        bool runtime)
    {
        base.ProcessEntityTypeAnnotations(annotations, entityType, runtimeEntityType, runtime);

        if (!runtime)
        {
            annotations.Remove(GaussDBAnnotationNames.UnloggedTable);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }
    }

    /// <inheritdoc />
    protected override void ProcessPropertyAnnotations(
        Dictionary<string, object?> annotations,
        IProperty property,
        RuntimeProperty runtimeProperty,
        bool runtime)
    {
        base.ProcessPropertyAnnotations(annotations, property, runtimeProperty, runtime);

        if (!runtime)
        {
            annotations.Remove(GaussDBAnnotationNames.IdentityOptions);
            annotations.Remove(GaussDBAnnotationNames.TsVectorConfig);
            annotations.Remove(GaussDBAnnotationNames.TsVectorProperties);

            if (!annotations.ContainsKey(GaussDBAnnotationNames.ValueGenerationStrategy))
            {
                annotations[GaussDBAnnotationNames.ValueGenerationStrategy] = property.GetValueGenerationStrategy();
            }
        }
    }

    /// <inheritdoc />
    protected override void ProcessIndexAnnotations(
        Dictionary<string, object?> annotations,
        IIndex index,
        RuntimeIndex runtimeIndex,
        bool runtime)
    {
        base.ProcessIndexAnnotations(annotations, index, runtimeIndex, runtime);

        if (!runtime)
        {
            annotations.Remove(GaussDBAnnotationNames.IndexMethod);
            annotations.Remove(GaussDBAnnotationNames.IndexOperators);
            annotations.Remove(GaussDBAnnotationNames.IndexSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexNullSortOrder);
            annotations.Remove(GaussDBAnnotationNames.IndexInclude);
            annotations.Remove(GaussDBAnnotationNames.CreatedConcurrently);
            annotations.Remove(GaussDBAnnotationNames.NullsDistinct);

            foreach (var annotationName in annotations.Keys.Where(
                         k => k.StartsWith(GaussDBAnnotationNames.StorageParameterPrefix, StringComparison.Ordinal)))
            {
                annotations.Remove(annotationName);
            }
        }
    }
}
