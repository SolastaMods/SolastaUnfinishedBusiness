using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionalPowerBuilder
        : FeatureDefinitionPowerBuilder<FeatureDefinitionConditionalPower, FeatureDefinitionConditionalPowerBuilder>
    {
        protected FeatureDefinitionConditionalPowerBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionConditionalPowerBuilder SetIsActive(IsActiveConditionalPowerDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
