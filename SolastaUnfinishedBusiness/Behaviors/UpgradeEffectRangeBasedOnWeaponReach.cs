using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

internal sealed class UpgradeEffectRangeBasedOnWeaponReach(BaseDefinition baseDefinition) : IModifyEffectDescription
{
    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        if (baseDefinition != definition)
        {
            return false;
        }

        var caster = GameLocationCharacter.GetFromActor(character);
        var attackMode = caster?.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        return attackMode is { SourceObject: RulesetItem, Ranged: false, Reach: true, ReachRange: > 1 };
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter rulesetCharacter,
        RulesetEffect rulesetEffect)
    {
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (character == null)
        {
            return effectDescription;
        }

        var attackMode = character.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        effectDescription.rangeParameter = attackMode.ReachRange;

        return effectDescription;
    }
}
