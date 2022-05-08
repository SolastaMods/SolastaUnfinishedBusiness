using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.CustomDefinitions.IPerformAttackAfterMagicEffectUse;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IPerformAttackAfterMagicEffectUse
    {
        delegate CharacterActionParams GetAttackAfterUseHandler(CharacterActionMagicEffect actionMagicEffect);

        delegate bool CanUseHandler(GameLocationCharacter caster, GameLocationCharacter target);

        CanUseHandler CanBeUsedToAttack { get; set; }
        GetAttackAfterUseHandler PerformAttackAfterUse { get; set; }
    }

    public class PerformAttackAfterMagicEffectUse : IPerformAttackAfterMagicEffectUse
    {
        public RuleDefinitions.RollOutcome minOutcomeToAttack = RuleDefinitions.RollOutcome.Success;
        public RuleDefinitions.RollOutcome minSaveOutcomeToAttack = RuleDefinitions.RollOutcome.Failure;
        
        public CanUseHandler CanBeUsedToAttack { get; set; }

        public GetAttackAfterUseHandler PerformAttackAfterUse { get; set; }

        public PerformAttackAfterMagicEffectUse()
        {
            CanBeUsedToAttack = DefaultCanUseHandler;
            PerformAttackAfterUse = DefautlAttackHandler;
        }

        private CharacterActionParams DefautlAttackHandler(CharacterActionMagicEffect effect)
        {
            if (effect == null) { return null; }

            var actionParams = effect.GetField<CharacterActionParams>("actionParams");
            if (actionParams == null) { return null; }

            if (effect.Countered || effect.GetProperty<bool>("ExecutionFailed"))
            {
                return null;
            }

            var outcome = effect.GetProperty<RuleDefinitions.RollOutcome>("Outcome");
            Main.Log2($"Outcome: {outcome}, save: {effect.SaveOutcome}", true);
            if (outcome < minOutcomeToAttack) { return null;}

            if (effect.SaveOutcome < minSaveOutcomeToAttack) { return null;}

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var caster = actionParams.GetField<GameLocationCharacter>("actingCharacter");
            var targets = actionParams.GetField<List<GameLocationCharacter>>("targetCharacters");

            if (caster == null || targets.Empty()) { return null; }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            if (attackMode == null) { return null; }

            var attackModifier = new ActionModifier();
            var eval = new BattleDefinitions.AttackEvaluationParams();

            //TODO: option to limit attack to only first target, while effect can have multiple
            targets = targets.Where(t =>
                {
                    //TODO: make options for range attacks
                    eval.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, t, t.LocationPosition,
                        attackModifier);
                    return battleService.CanAttack(eval);
                })
                .ToList();


            if (!targets.Empty())
            {
                var attackActionParams = new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                {
                    AttackMode = attackMode
                };

                foreach (var target in targets)
                {
                    attackActionParams.TargetCharacters.Add(target);
                    attackActionParams.ActionModifiers.Add(attackModifier);
                }

                return attackActionParams;
            }

            return null;
        }

        private bool DefaultCanUseHandler(GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            if (battleService == null)
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();
            //TODO: add option for ranged attacks
            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
                target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }


        public static readonly IPerformAttackAfterMagicEffectUse MeleeAttack = new PerformAttackAfterMagicEffectUse();
    }
}
