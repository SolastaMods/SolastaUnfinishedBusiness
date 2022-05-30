using System;

namespace SolastaCommunityExpansion.Builders.Features;

public abstract class FeatureDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinition
    where TBuilder : FeatureDefinitionBuilder<TDefinition, TBuilder>
{
    #region Constructors

    protected FeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion

    // TODO: methods specific to FeatureDefinition
}

public class FeatureDefinitionBuilder : FeatureDefinitionBuilder<FeatureDefinition, FeatureDefinitionBuilder>
{
    #region Constructors

    protected FeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionBuilder(FeatureDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(FeatureDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
