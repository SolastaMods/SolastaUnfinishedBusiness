using System;

namespace SolastaUnfinishedBusiness.Builders.Features;

public abstract class
    FeatureDefinitionAffinityBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionAffinity
    where TBuilder : FeatureDefinitionAffinityBuilder<TDefinition, TBuilder>
{
    #region Constructors

    protected FeatureDefinitionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionAffinityBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAffinityBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
