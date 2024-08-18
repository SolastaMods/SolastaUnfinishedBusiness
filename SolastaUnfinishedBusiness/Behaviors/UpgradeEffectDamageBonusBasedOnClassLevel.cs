using System;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

internal sealed class UpgradeEffectDamageBonusBasedOnClassLevel(
    BaseDefinition baseDefinition,
    CharacterClassDefinition characterClassDefinition,
    double modifier = 1) : IModifyEffectDescription
{
    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        return baseDefinition == definition;
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter rulesetCharacter,
        RulesetEffect rulesetEffect)
    {
        var bonus = (int)Math.Ceiling(rulesetCharacter.GetClassLevel(characterClassDefinition) * modifier);

        effectDescription.FindFirstDamageForm().BonusDamage = bonus;

        return effectDescription;
    }
}
