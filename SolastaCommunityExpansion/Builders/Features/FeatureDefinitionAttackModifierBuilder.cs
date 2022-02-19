using System;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttackModifierBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionAttackModifier, FeatureDefinitionAttackModifierBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original) : base(original)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionAttackModifierBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAttackModifierBuilder(name, namespaceGuid);
        }
    }
}
