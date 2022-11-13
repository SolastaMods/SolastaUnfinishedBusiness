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
}
