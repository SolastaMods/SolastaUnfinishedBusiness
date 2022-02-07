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

        public FeatureDefinitionSummoningAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
    }
}
