using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal sealed class AttackAfterMagicEffect(AttackAfterMagicEffect.AttackType attackType, bool firstTargetOnly)
    : IFilterTargetingCharacter
{
    internal const string AttackAfterMagicEffectTag = "AttackAfterMagicEffectTag";

    private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
    private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

    internal static readonly AttackAfterMagicEffect MarkerAnyWeaponAttack =
        new(AttackType.Melee | AttackType.Ranged | AttackType.Thrown, true);

    internal static readonly AttackAfterMagicEffect MarkerMeleeWeaponAttack = new(AttackType.Melee, true);
    internal static readonly AttackAfterMagicEffect MarkerRangedWeaponAttack = new(AttackType.Ranged, false);

    internal readonly bool AllowMelee = attackType.HasFlag(AttackType.Melee);
    internal readonly bool AllowRanged = attackType.HasFlag(AttackType.Ranged);
    internal readonly bool AllowThrown = attackType.HasFlag(AttackType.Thrown);

    public bool EnforceFullSelection => false;

    public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
    {
        if (!firstTargetOnly && __instance.SelectionService.SelectedTargets.Count != 0)
        {
            return true;
        }

        if (CanAttack(__instance.ActionParams.ActingCharacter, target, AllowMelee, AllowRanged, AllowThrown))
        {
            return true;
        }

        __instance.actionModifier.FailureFlags.Add(Gui.Localize("Failure/&CannotAttackTarget"));

        return false;
    }

    internal static bool CanAttack(
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        bool allowMelee,
        bool allowRanged,
        bool allowThrown)
    {
        var attackMode = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        if (attackMode == null)
        {
            return false;
        }

        var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var attackModifier = new ActionModifier();
        var evalParams = new BattleDefinitions.AttackEvaluationParams();
        var attackerPosition = attacker.LocationPosition;
        var defenderPosition = defender.LocationPosition;
        var canAttack = false;

        switch (attackMode.Ranged)
        {
            case false when allowMelee:
            {
                evalParams.FillForPhysicalReachAttack(
                    attacker, attackerPosition, attackMode, defender, defenderPosition, attackModifier);

                var reach = Main.Settings.AllowBladeCantripsToUseReach ? attackMode.ReachRange : 1;

                canAttack = battleService.CanAttack(evalParams) && attacker.IsWithinRange(defender, reach);

                if (!canAttack && allowThrown)
                {
                    attackMode.ranged = true;
                    evalParams.FillForPhysicalRangeAttack(
                        attacker, attackerPosition, attackMode, defender, defenderPosition, attackModifier);

                    canAttack = battleService.CanAttack(evalParams);
                }

                break;
            }
            case true when allowRanged:
                evalParams.FillForPhysicalRangeAttack(
                    attacker, attackerPosition, attackMode, defender, defenderPosition, attackModifier);

                canAttack = battleService.CanAttack(evalParams);
                break;
        }

        return canAttack;
    }

    internal List<CharacterActionParams> PerformAttackAfterUse(CharacterActionMagicEffect actionMagicEffect)
    {
        var attacks = new List<CharacterActionParams>();
        var actionParams = actionMagicEffect?.ActionParams;

        if (actionParams == null)
        {
            return attacks;
        }

        //Spell got countered or it failed
        if (actionMagicEffect.Countered || actionMagicEffect.ExecutionFailed)
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
            .Where(t => CanAttack(caster, t, AllowMelee, AllowRanged, AllowThrown))
            .ToArray();

        if (targets.Length == 0)
        {
            return attacks;
        }

        var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        if (attackMode == null)
        {
            return attacks;
        }

        var maxTargets = firstTargetOnly ? 1 : targets.Length;

        for (var i = 0; i < maxTargets; i++)
        {
            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);
            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = actionMagicEffect.ActionType;

            //mark this attack for proper integration with polearm, and follow-up strike
            if (!actionParams.ActingCharacter.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
            {
                attackMode.AddAttackTagAsNeeded(AttackAfterMagicEffectTag);
            }

            // always use free attack
            var attackActionParams =
                new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

            attackActionParams.TargetCharacters.Add(targets[0]);
            attackActionParams.ActionModifiers.Add(new ActionModifier());
            attacks.Add(attackActionParams);
        }

        return attacks;
    }

    [Flags]
    internal enum AttackType
    {
        Melee = 1,
        Ranged = 2,
        Thrown = 4
    }
}
