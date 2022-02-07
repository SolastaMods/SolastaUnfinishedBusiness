using System;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionConditionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionConditionAffinity>
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
