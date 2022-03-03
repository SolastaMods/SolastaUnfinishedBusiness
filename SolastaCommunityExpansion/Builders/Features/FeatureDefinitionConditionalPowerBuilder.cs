using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionConditionalPowerBuilder
        : FeatureDefinitionPowerBuilder<FeatureDefinitionConditionalPower, FeatureDefinitionConditionalPowerBuilder>
    {
        private FeatureDefinitionConditionalPowerBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public static FeatureDefinitionConditionalPowerBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionConditionalPowerBuilder(name, namespaceGuid);
        }

        public FeatureDefinitionConditionalPowerBuilder SetIsActive(IsActiveConditionalPowerDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
