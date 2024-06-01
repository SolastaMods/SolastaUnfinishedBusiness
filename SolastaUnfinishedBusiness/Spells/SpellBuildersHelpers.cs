using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MetamagicOptionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    internal const string PhysicalAttackFromCantrip = "PhysicalAttackFromCantrip";

    private static readonly (string, IMagicEffect)[] DamagesAndEffects =
    [
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold), (DamageTypeFire, FireBolt),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray), (DamageTypeThunder, Shatter)
    ];

    public interface IAttackAfterMagicEffect
    {
        public delegate bool CanAttackHandler(GameLocationCharacter caster, GameLocationCharacter target);

        public delegate bool CanUseHandler(
            CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target,
            out string failure);

        [CanBeNull]
        public delegate IEnumerable<CharacterActionParams> GetAttackAfterUseHandler(
            CharacterActionMagicEffect actionMagicEffect);

        public CanUseHandler CanBeUsedToAttack { get; }
        public GetAttackAfterUseHandler PerformAttackAfterUse { get; }
        public CanAttackHandler CanAttack { get; }
    }

    private sealed class AttackAfterMagicEffect : IAttackAfterMagicEffect
    {
        private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
        private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

        internal static readonly IAttackAfterMagicEffect BoomingBladeAttack =
            new AttackAfterMagicEffect(1);

        internal static readonly IAttackAfterMagicEffect ResonatingStrikeAttack =
            new AttackAfterMagicEffect(1);

        internal static readonly IAttackAfterMagicEffect SunlitBladeAttack =
            new AttackAfterMagicEffect(1);

        private readonly int _maxAttacks;

        private AttackAfterMagicEffect(int maxAttacks)
        {
            _maxAttacks = maxAttacks;
            CanAttack = CanMeleeAttack;
            CanBeUsedToAttack = DefaultCanUseHandler;
            PerformAttackAfterUse = DefaultAttackHandler;
        }

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; }
        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; }
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; }

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [NotNull]
        private List<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect actionMagicEffect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = actionMagicEffect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            //Spell got countered or it failed
            if (actionMagicEffect.Countered ||
                actionMagicEffect.ExecutionFailed)
            {
                return attacks;
            }

            //Attack outcome is worse that required
            if (actionMagicEffect.AttackRollOutcome > MinOutcomeToAttack)
            {
                return attacks;
            }

            //Target rolled saving throw and got better result
            if (actionMagicEffect.RolledSaveThrow && actionMagicEffect.SaveOutcome < MinSaveOutcomeToAttack)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters
                .Where(t => CanMeleeAttack(caster, t))
                .ToList();

            if (targets.Count == 0)
            {
                return attacks;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return attacks;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);
            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = actionMagicEffect.ActionType;

            //mark this attack for proper integration with War Magic
            attackMode.AttackTags.TryAdd(PhysicalAttackFromCantrip);

            var twinned =
                actionMagicEffect is CharacterActionCastSpell castSpell &&
                castSpell.ActiveSpell.MetamagicOption == MetamagicTwinnedSpell;
            var maxAttacks = _maxAttacks + (twinned ? 1 : 0);

            var attackModifier = new ActionModifier();

            caster.BurnOneMainAttack();

            foreach (var target in targets)
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);

                if (attackActionParams.TargetCharacters.Count >= maxAttacks)
                {
                    break;
                }
            }

            return attacks;
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target,
            [NotNull] out string failure)
        {
            failure = String.Empty;

            var maxTargets = targeting.maxTargets;
            var remainingTargets = targeting.remainingTargets;
            var selectedTargets = maxTargets - remainingTargets;

            if (selectedTargets > 0)
            {
                return true;
            }

            var canAttack = CanMeleeAttack(caster, target);

            if (!canAttack)
            {
                failure = "Failure/&FailureFlagTargetMeleeWeaponError";
            }

            return canAttack;
        }
    }
}
