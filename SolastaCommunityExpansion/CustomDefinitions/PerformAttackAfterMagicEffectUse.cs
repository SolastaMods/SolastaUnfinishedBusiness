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
        delegate bool CanAttackHandler(GameLocationCharacter caster, GameLocationCharacter target);

        CanUseHandler CanBeUsedToAttack { get; set; }
        GetAttackAfterUseHandler PerformAttackAfterUse { get; set; }
        CanAttackHandler CanAttack { get; set; }
    }

    public class PerformAttackAfterMagicEffectUse : IPerformAttackAfterMagicEffectUse
    {
        public RuleDefinitions.RollOutcome minOutcomeToAttack = RuleDefinitions.RollOutcome.Success;
        public RuleDefinitions.RollOutcome minSaveOutcomeToAttack = RuleDefinitions.RollOutcome.Failure;

        public CanUseHandler CanBeUsedToAttack { get; set; }

        public GetAttackAfterUseHandler PerformAttackAfterUse { get; set; }
        public CanAttackHandler CanAttack { get; set; }

        public PerformAttackAfterMagicEffectUse()
        {
            CanAttack = CanMeleeAttack;
            CanBeUsedToAttack = DefaultCanUseHandler;
            PerformAttackAfterUse = DefautlAttackHandler;
        }

        private bool CanMeleeAttack(GameLocationCharacter caster, GameLocationCharacter target)
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

            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
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
            if (outcome < minOutcomeToAttack) { return null; }

            if (effect.SaveOutcome < minSaveOutcomeToAttack) { return null; }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;

            if (caster == null || targets.Empty()) { return null; }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
            if (attackMode == null) { return null; }

            var attackModifier = new ActionModifier();

            //TODO: option to limit attack to select target, while effect can have multiple
            var target = targets
                .FirstOrDefault(t => CanMeleeAttack(caster, t));

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
            //TODO: implement setting to tell how many targets must meet weapon attack requirements
            var maxTargets = targeting.GetField<int>("maxTargets");
            var remainingTargets = targeting.GetField<int>("remainingTargets");
            var selectedTargets = maxTargets - remainingTargets;
            if (selectedTargets > 0)
            {
                return true;
            }

            //TODO: add option for ranged attacks
            var canAttack = CanMeleeAttack(caster, target);
            if (!canAttack)
            {
                failure = "Failure/&FailureFlagTargetMeleeWeaponError";
            }

            return canAttack;
        }


        public static readonly IPerformAttackAfterMagicEffectUse MeleeAttack = new PerformAttackAfterMagicEffectUse();
    }
}
