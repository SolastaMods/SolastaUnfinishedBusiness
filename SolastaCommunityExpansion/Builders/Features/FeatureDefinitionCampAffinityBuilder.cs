using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionCampAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionCampAffinity,
        FeatureDefinitionCampAffinityBuilder>

    {
        public FeatureDefinitionCampAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionCampAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionCampAffinityBuilder(FeatureDefinitionCampAffinity original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionCampAffinityBuilder(FeatureDefinitionCampAffinity original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }
    }
}
