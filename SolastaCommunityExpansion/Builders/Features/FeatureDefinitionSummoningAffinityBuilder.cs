using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSummoningAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSummoningAffinity>
    {
        public FeatureDefinitionSummoningAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }
    }
}
