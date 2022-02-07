using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionDamageAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionDamageAffinity>
    {
        /*        private FeatureDefinitionDamageAffinityBuilder(string name, string guid)
                    : base(name, guid)
                {
                }

                private FeatureDefinitionDamageAffinityBuilder(string name, Guid namespaceGuid, Category category = Category.None)
                    : base(name, namespaceGuid, category)
                {
                }

                private FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, string guid)
                    : base(original, name, guid)
                {
                }
        */
        private FeatureDefinitionDamageAffinityBuilder(FeatureDefinitionDamageAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid, Category.None)
        {
        }

        public static FeatureDefinitionDamageAffinityBuilder Create(FeatureDefinitionDamageAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionDamageAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
