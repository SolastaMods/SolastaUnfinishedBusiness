using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionLightAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionLightAffinity, FeatureDefinitionLightAffinityBuilder>
    {
        private FeatureDefinitionLightAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private FeatureDefinitionLightAffinityBuilder(FeatureDefinitionLightAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionLightAffinityBuilder Create(
            string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionLightAffinityBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionLightAffinityBuilder Create(
            FeatureDefinitionLightAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionLightAffinityBuilder(original, name, namespaceGuid);
        }


        public FeatureDefinitionLightAffinityBuilder AddLightingEffectAndCondition(FeatureDefinitionLightAffinity.LightingEffectAndCondition lightingEffectAndCondition)
        {
            Definition.AddLightingEffectAndConditionList(lightingEffectAndCondition);
            return this;
        }


    }
}
