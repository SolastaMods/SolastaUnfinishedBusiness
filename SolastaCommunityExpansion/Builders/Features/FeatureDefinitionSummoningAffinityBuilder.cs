using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionSummoningAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSummoningAffinity>
    {
/*        private FeatureDefinitionSummoningAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private FeatureDefinitionSummoningAffinityBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }
*/
        private FeatureDefinitionSummoningAffinityBuilder(FeatureDefinitionSummoningAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public static FeatureDefinitionSummoningAffinityBuilder Create(FeatureDefinitionSummoningAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionSummoningAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
