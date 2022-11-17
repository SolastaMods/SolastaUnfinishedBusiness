using TA;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class GameLocationCharacterExtensions
{
    internal static (RulesetAttackMode mode, ActionModifier modifier) GetFirstMeleeAttackThatCanAttack(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        IGameLocationBattleService service = null)
    {
        service ??= ServiceRepository.GetService<IGameLocationBattleService>();

        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (!mode.Reach)
            {
                continue;
            }

            // Prepare attack evaluation params
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var modifier = new ActionModifier();

            attackParams.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                target, target.LocationPosition, modifier);

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (service.CanAttack(attackParams))
            {
                return (mode, modifier);
            }
        }

        return (null, null);
    }

    /**
     * Finds first melee attack mode that can attack target on positionBefore, but can't on positionAfter
     */
    internal static bool CanPerformOpportunityAttackOnCharacter(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        int3 positionBefore,
        int3 positionAfter,
        out RulesetAttackMode attackMode,
        out ActionModifier attackModifier,
        IGameLocationBattleService service = null,
        bool accountAoOImmunity = false)
    {
        service ??= ServiceRepository.GetService<IGameLocationBattleService>();
        attackMode = null;
        attackModifier = null;

        if (accountAoOImmunity && !service.IsValidAttackerForOpportunityAttackOnCharacter(instance, target))
        {
            return false;
        }

        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (!mode.Reach)
            {
                continue;
            }

            // Prepare attack evaluation params
            var paramsBefore = new BattleDefinitions.AttackEvaluationParams();
            paramsBefore.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                target, positionBefore, new ActionModifier());

            var paramsAfter = new BattleDefinitions.AttackEvaluationParams();
            paramsAfter.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                target, positionAfter, new ActionModifier());

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (service.CanAttack(paramsBefore) && !service.CanAttack(paramsAfter))
            {
                attackMode = mode;
                attackModifier = paramsBefore.attackModifier;
                return true;
            }
        }

        return false;
    }
}
