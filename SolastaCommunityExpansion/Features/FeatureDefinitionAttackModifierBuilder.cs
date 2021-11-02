
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
    {
        public FeatureDefinitionAttackModifierBuilder(string name, string guid, RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
        int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
        int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag,
        GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetAttackRollModifierMethod(attackRollModifierMethod);
            Definition.SetAttackRollModifier(attackRollModifier);
            Definition.SetAttackRollAbilityScore(attackRollAbilityScore);
            Definition.SetDamageRollModifierMethod(damageRollModifierMethod);
            Definition.SetDamageRollModifier(damageRollModifier);
            Definition.SetDamageRollAbilityScore(damageRollAbilityScore);
            Definition.SetCanAddAbilityBonusToSecondary(canAddAbilityBonusToSecondary);
            Definition.SetAdditionalAttackTag(additionalAttackTag);

            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
