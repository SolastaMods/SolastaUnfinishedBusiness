using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        public FeatureDefinitionAttributeModifierBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        // TODO: drop these ctors
        public FeatureDefinitionAttributeModifierBuilder(string name, string guid, AttributeModifierOperation modifierType,
            string attribute, int amount, GuiPresentation guiPresentation) : base(name, guid, guiPresentation)
        {
            SetModifier(modifierType, attribute, amount);
        }

        public FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid, AttributeModifierOperation modifierType,
            string attribute, int amount, string category) : base(name, namespaceGuid, category)
        {
            SetModifier(modifierType, attribute, amount);
        }
        // -- to here

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
    }
}
