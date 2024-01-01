using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal sealed class UpgradeRangeBasedOnWeaponReach : IModifyEffectDescription
{
    private readonly BaseDefinition _baseDefinition;

    public UpgradeRangeBasedOnWeaponReach(BaseDefinition baseDefinition)
    {
        _baseDefinition = baseDefinition;
    }

    public bool IsValid(
        BaseDefinition definition,
        RulesetCharacter character,
        EffectDescription effectDescription)
    {
        if (_baseDefinition != definition)
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
