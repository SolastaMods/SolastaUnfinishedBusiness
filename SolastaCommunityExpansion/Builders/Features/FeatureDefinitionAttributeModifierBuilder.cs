using System;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttributeModifierBuilder : FeatureDefinitionBuilder<FeatureDefinitionAttributeModifier, FeatureDefinitionAttributeModifierBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttributeModifierBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }


        protected FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionAttributeModifierBuilder SetModifier(AttributeModifierOperation modifierType, string attribute, int amount)
        {
            Definition.SetModifierType2(modifierType);
            Definition.SetModifiedAttribute(attribute);
            Definition.SetModifierValue(amount);
            return this;
        }

        public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore)
        {
            Definition.SetModifierAbilityScore(abilityScore);
            return this;
        }

        public FeatureDefinitionAttributeModifierBuilder SetSituationalContext(RuleDefinitions.SituationalContext situationalContext)
        {
            Definition.SetSituationalContext(situationalContext);
            return this;
        }
    }
}
