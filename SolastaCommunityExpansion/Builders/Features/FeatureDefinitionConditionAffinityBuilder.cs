using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionConditionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionConditionAffinity>
    {
        private FeatureDefinitionConditionAffinityBuilder(
            FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionConditionAffinityBuilder Create(
            FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid, Category category = Category.None)
        {
            return new FeatureDefinitionConditionAffinityBuilder(original, name, namespaceGuid, category);
        }
    }
}
