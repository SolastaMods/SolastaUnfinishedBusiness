using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

internal sealed class UpgradeSpellRangeBasedOnWeaponReach(BaseDefinition baseDefinition) : IModifyEffectDescription
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

        if (caster == null || attackMode is not { SourceObject: RulesetItem })
        {
            return false;
        }

        if (attackMode.Ranged || !attackMode.Reach)
        {
            return false;
        }

        var reach = attackMode.reachRange;

        return reach > 1;
    }

    public EffectDescription GetEffectDescription(
        BaseDefinition definition,
        EffectDescription effectDescription,
        RulesetCharacter character,
        RulesetEffect rulesetEffect)
    {
        var caster = GameLocationCharacter.GetFromActor(character);

        if (caster == null)
        {
            return effectDescription;
        }

        var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
        var reach = attackMode.reachRange;

        effectDescription.rangeParameter = reach;

        return effectDescription;
    }
}
