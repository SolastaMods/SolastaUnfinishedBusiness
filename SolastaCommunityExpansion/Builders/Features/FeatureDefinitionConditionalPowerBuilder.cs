using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionConditionalPowerBuilder
        : FeatureDefinitionPowerBuilder<FeatureDefinitionConditionalPower, FeatureDefinitionConditionalPowerBuilder>
    {
        #region Constructors
        protected FeatureDefinitionConditionalPowerBuilder(FeatureDefinitionConditionalPower original) : base(original)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionConditionalPowerBuilder(FeatureDefinitionConditionalPower original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
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
