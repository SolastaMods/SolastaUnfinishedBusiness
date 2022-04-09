using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionalPowerBuilder
        : FeatureDefinitionPowerBuilder<FeatureDefinitionConditionalPower, FeatureDefinitionConditionalPowerBuilder>
    {
        #region Constructors
        protected FeatureDefinitionConditionalPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(FeatureDefinitionConditionalPower original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(FeatureDefinitionConditionalPower original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionConditionalPowerBuilder SetIsActive(IsActiveConditionalPowerDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
