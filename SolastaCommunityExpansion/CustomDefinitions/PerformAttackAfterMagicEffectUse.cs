using System;
using System.Linq;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.CustomDefinitions.IPerformAttackAfterMagicEffectUse;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IPerformAttackAfterMagicEffectUse
    {
        delegate CharacterActionParams GetAttackAfterUseHandler(CharacterActionMagicEffect actionMagicEffect);

        delegate bool CanUseHandler(CursorLocationSelectTarget targeting, GameLocationCharacter caster, GameLocationCharacter target, out string failure);

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

            var actionParams = effect.ActionParams;
            if (actionParams == null) { return null; }

            if (effect.Countered || effect.GetProperty<bool>("ExecutionFailed"))
            {
                return null;
            }

            var outcome = effect.GetProperty<RuleDefinitions.RollOutcome>("Outcome");
            if (outcome < minOutcomeToAttack) { return null;}

            if (effect.SaveOutcome < minSaveOutcomeToAttack) { return null;}

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;

            if (caster == null || targets.Empty()) { return null; }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            if (attackMode == null) { return null; }

            var attackModifier = new ActionModifier();
            var eval = new BattleDefinitions.AttackEvaluationParams();

            //TODO: option to limit attack to select target, while effect can have multiple
            var target = targets.Where(t =>
                {
                    //TODO: make options for range attacks
                    eval.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, t, t.LocationPosition,
                        attackModifier);
                    return battleService.CanAttack(eval);
                })
                .FirstOrDefault();


            if (target != null)
            {
                var attackActionParams = new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                {
                    AttackMode = attackMode
                };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);

                return attackActionParams;
            }

            return null;
        }

        private bool DefaultCanUseHandler(CursorLocationSelectTarget targeting, GameLocationCharacter caster, GameLocationCharacter target, out string failure)
        {
            failure = String.Empty;
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            if (attackMode == null)
            {
                return false;
            }

            //TODO: implement setting to tell how many targets must meet weapon attack requirements
            var maxTargets = targeting.GetField<int>("maxTargets");
            var remainingTargets = targeting.GetField<int>("remainingTargets");
            var selectedTargets = maxTargets - remainingTargets;
            if (selectedTargets > 0)
            {
                return true;
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

            var canAttack = battleService.CanAttack(evalParams);
            if (!canAttack)
            {
                failure = "Failure/&FailureFlagTargetMeleeWeaponError";
            }

            return canAttack;
        }


        public static readonly IPerformAttackAfterMagicEffectUse MeleeAttack = new PerformAttackAfterMagicEffectUse();
    }
}
