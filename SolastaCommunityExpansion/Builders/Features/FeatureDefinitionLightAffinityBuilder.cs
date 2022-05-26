using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionLightAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionLightAffinity,
        FeatureDefinitionLightAffinityBuilder>
    {
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

        public FeatureDefinitionLightAffinityBuilder AddLightingEffectAndCondition(
            FeatureDefinitionLightAffinity.LightingEffectAndCondition lightingEffectAndCondition)
        {
            Definition.AddLightingEffectAndConditionList(lightingEffectAndCondition);
            return this;
        }
    }
}
