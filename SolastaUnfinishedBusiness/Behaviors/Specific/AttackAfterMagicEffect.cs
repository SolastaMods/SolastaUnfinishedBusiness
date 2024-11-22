using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal sealed class AttackAfterMagicEffect(AttackAfterMagicEffect.AttackType attackType) : IFilterTargetingCharacter
{
    internal const string AttackAfterMagicEffectTag = "AttackAfterMagicEffectTag";

    private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
    private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

    internal static readonly AttackAfterMagicEffect MarkerAnyWeaponAttack =
        new(AttackType.Melee | AttackType.Ranged | AttackType.Thrown);

    internal static readonly AttackAfterMagicEffect MarkerMeleeWeaponAttack = new(AttackType.Melee);
    internal static readonly AttackAfterMagicEffect MarkerRangedWeaponAttack = new(AttackType.Ranged);

    internal readonly bool AllowMelee = attackType.HasFlag(AttackType.Melee);
    internal readonly bool AllowRanged = attackType.HasFlag(AttackType.Ranged);
    internal readonly bool AllowThrown = attackType.HasFlag(AttackType.Thrown);

    public bool EnforceFullSelection => false;

    public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
    {
        // only enforce weapon reach or 5 ft on first target
        if (__instance.SelectionService.SelectedTargets.Count != 0)
        {
            return true;
        }

        if (CanAttack(
                __instance.ActionParams.ActingCharacter, target,
                 AllowMelee, AllowRanged, AllowThrown, out var allowReach))
        {
            return true;
        }

        var text = allowReach ? "Feedback/&WithinReach" : "Feedback/&Within5Ft";

        __instance.actionModifier.FailureFlags.Add(Gui.Format("Failure/&TargetMeleeWeaponError", text));

        return false;
    }

    internal static bool CanAttack(
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        bool allowMelee,
        bool allowRanged,
        bool allowThrown,
        out bool allowReach)
    {
        allowReach = Main.Settings.AllowBladeCantripsToUseReach;

        var attackMode = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        if (attackMode == null)
        {
            return false;
        }

        var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var attackModifier = new ActionModifier();
        var evalParams = new BattleDefinitions.AttackEvaluationParams();
        var ranged = attackMode.Ranged;
        var thrown = attackMode.Thrown;
        var canAttack = false;

        if (allowRanged && ranged || allowThrown && thrown)
        {
            evalParams.FillForPhysicalRangeAttack(
                attacker, attacker.LocationPosition, attackMode, defender, defender.LocationPosition, attackModifier);

            canAttack = battleService.CanAttack(evalParams);
        }
        else if (allowMelee && !ranged)
        {
            evalParams.FillForPhysicalReachAttack(
                attacker, attacker.LocationPosition, attackMode, defender, defender.LocationPosition, attackModifier);

            canAttack = battleService.CanAttack(evalParams) && (allowReach || attacker.IsWithinRange(defender, 1));
        }

        return canAttack;
    }

    internal static List<CharacterActionParams> PerformAttackAfterUse(CharacterActionMagicEffect actionMagicEffect)
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
        //At this point it's safe to pass true to allowMelee and allowRanged as validations already happened
        var targets = actionParams.TargetCharacters
            .Where(t => CanAttack(caster, t, true, true, true, out _))
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
