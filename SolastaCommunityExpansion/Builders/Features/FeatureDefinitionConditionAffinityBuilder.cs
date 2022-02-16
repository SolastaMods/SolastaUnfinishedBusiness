using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionConditionAffinityBuilder : DefinitionBuilder<FeatureDefinitionConditionAffinity>
    {
        private FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public static FeatureDefinitionConditionAffinityBuilder Create(FeatureDefinitionConditionAffinity original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionConditionAffinityBuilder(original, name, namespaceGuid);
        }
    }
}
