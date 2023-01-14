using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class RangedAttackInMeleeDisadvantageRemover
{
    private readonly IsWeaponValidHandler isWeaponValid;
    private readonly IsCharacterValidHandler[] validators;

    internal RangedAttackInMeleeDisadvantageRemover(
        IsWeaponValidHandler isWeaponValid,
        params IsCharacterValidHandler[] validators)
    {
        this.isWeaponValid = isWeaponValid;
        this.validators = validators;
    }

    internal RangedAttackInMeleeDisadvantageRemover(params IsCharacterValidHandler[] validators)
        : this(ValidatorsWeapon.AlwaysValid, validators)
    {
    }

    private bool CanApply(RulesetCharacter character, RulesetAttackMode attackMode)
    {
        if (isWeaponValid != null && !isWeaponValid.Invoke(attackMode, null, character))
        {
            return false;
        }

        return character.IsValid(validators);
    }

    /**
     * Patches `GameLocationBattleManager.CanAttack`
     * Removes ranged attack in melee disadvantage if there's specific feature present and active
     */
    internal static void CheckToRemoveRangedDisadvantage(BattleDefinitions.AttackEvaluationParams attackParams)
    {
        if (attackParams.attackProximity != BattleDefinitions.AttackProximity.PhysicalRange)
        {
            return;
        }

        var character = attackParams.attacker?.RulesetCharacter;

        if (character == null)
        {
            return;
        }

        var features = character.GetSubFeaturesByType<RangedAttackInMeleeDisadvantageRemover>();

        if (!features.Any(f => f.CanApply(character, attackParams.attackMode)))
        {
            return;
        }

        attackParams.attackModifier.attackAdvantageTrends.RemoveAll(t =>
            t.value == -1
            && t is
            {
                sourceType: RuleDefinitions.FeatureSourceType.Proximity,
                sourceName: RuleDefinitions.ProximityRangeEnemyNearby
            });
    }
}
