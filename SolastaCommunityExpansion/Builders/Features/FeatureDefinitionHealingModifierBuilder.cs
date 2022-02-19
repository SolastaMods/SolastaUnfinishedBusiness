using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionHealingModifierBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionHealingModifier, FeatureDefinitionHealingModifierBuilder>
    {
        #region Constructors
        protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original) : base(original)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
