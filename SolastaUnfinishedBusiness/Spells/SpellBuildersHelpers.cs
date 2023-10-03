using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Spells;

internal static partial class SpellBuilders
{
    internal const string CantripWeaponAttack = "CantripWeaponAttack";

    private static readonly (string, IMagicEffect)[] DamagesAndEffects =
    {
        (DamageTypeAcid, AcidSplash), (DamageTypeCold, ConeOfCold), (DamageTypeFire, FireBolt),
        (DamageTypeLightning, LightningBolt), (DamageTypePoison, PoisonSpray), (DamageTypeThunder, Shatter)
    };

    private sealed class UpgradeRangeBasedOnWeaponReach : IModifyEffectDescription
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

    private sealed class AttackAfterMagicEffect : IAttackAfterMagicEffect
    {
        private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
        private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

        // changed below to 1 based on
        // https://rpg.stackexchange.com/questions/177342/can-the-spell-booming-blade-be-affected-by-the-twinned-spell-metamagic

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

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return gameLocationBattleService.CanAttack(evalParams);
        }

        [NotNull]
        private List<CharacterActionParams> DefaultAttackHandler([CanBeNull] CharacterActionMagicEffect effect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            //Spell got countered or it failed
            if (effect.Countered || effect.ExecutionFailed)
            {
                return attacks;
            }

            //Attack outcome is worse that required
            if (effect.AttackRollOutcome > MinOutcomeToAttack)
            {
                return attacks;
            }

            //Target rolled saving throw and got better result
            if (effect.RolledSaveThrow && effect.SaveOutcome < MinSaveOutcomeToAttack)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;

            if (targets.Empty())
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
            attackMode.ActionType = effect.ActionType;

            //PATCH: add tag so it can be identified by War Magic
            attackMode.AddAttackTagAsNeeded(CantripWeaponAttack);

            //PATCH: ensure we flag cantrip used if action switch enabled
            if (Main.Settings.EnableActionSwitching)
            {
                caster.UsedMainCantrip = true;
            }

            var attackModifier = new ActionModifier();

            foreach (var target in targets.Where(t => CanMeleeAttack(caster, t)))
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);

                if (attackActionParams.TargetCharacters.Count >= _maxAttacks)
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
