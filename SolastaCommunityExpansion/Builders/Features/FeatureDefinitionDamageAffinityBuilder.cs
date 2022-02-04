using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionDamageAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
    {
        public FeatureDefinitionDamageAffinityBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }
    }
}
