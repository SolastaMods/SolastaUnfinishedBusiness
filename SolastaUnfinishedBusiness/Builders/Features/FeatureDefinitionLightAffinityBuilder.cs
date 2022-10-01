using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionLightAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionLightAffinity,
    FeatureDefinitionLightAffinityBuilder>
{
    public FeatureDefinitionLightAffinityBuilder AddLightingEffectAndCondition(
        FeatureDefinitionLightAffinity.LightingEffectAndCondition lightingEffectAndCondition)
    {
        Definition.LightingEffectAndConditionList.Add(lightingEffectAndCondition);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionLightAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionLightAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionLightAffinityBuilder(FeatureDefinitionLightAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionLightAffinityBuilder(FeatureDefinitionLightAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
