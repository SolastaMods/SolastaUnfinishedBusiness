using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionActionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
    {
        public FeatureDefinitionActionAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }
    }
}
