using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        public FeatureDefinitionAttributeModifierBuilder(string name, string guid, FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
        string attribute, int amount, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetModifierType2(modifierType);
            Definition.SetModifiedAttribute(attribute);
            Definition.SetModifierValue(amount);
            Definition.SetGuiPresentation(guiPresentation);
        }
        public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore)
        {
            Definition.SetModifierAbilityScore(abilityScore);
            return this;
        }
    }

}
