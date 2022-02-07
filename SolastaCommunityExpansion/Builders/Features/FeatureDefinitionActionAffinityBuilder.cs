using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionActionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
    {
        /*        private FeatureDefinitionActionAffinityBuilder(string name, string guid)
                    : base(name, guid)
                {
                }

                private FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid, Category category = Category.None)
                    : base(name, namespaceGuid, category)
                {
                }

                private FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, string guid)
                    : base(original, name, guid)
                {
                }
        */

        // Add other standard Create methods and constructors as required.

        private FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionActionAffinityBuilder Create(FeatureDefinitionActionAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionActionAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
