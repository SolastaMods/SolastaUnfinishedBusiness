using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal sealed class AttackAfterMagicEffect
{
    private const string AttackCantrip = "AttackCantrip";
    private const string QuickenedAttackCantrip = "QuickenedAttackCantrip";
    private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
    private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

    internal static void HandleAttackAfterMagicEffect(GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        if (actionParams.AttackMode == null)
        {
            return;
        }

        if (actionParams.AttackMode.AttackTags.Contains(AttackCantrip))
        {
            character.UsedMainCantrip = true;
        }

        // ReSharper disable once InvertIf
        if (actionParams.AttackMode.AttackTags.Contains(QuickenedAttackCantrip))
        {
            character.UsedMainSpell = true;
            character.SpendActionType(ActionDefinitions.ActionType.Bonus);
        }
    }

    internal static bool CanAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
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

    internal static List<CharacterActionParams> PerformAttackAfterUse(CharacterActionMagicEffect actionMagicEffect)
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
            .Where(t => CanAttack(caster, t))
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
        attackMode.AttackTags.TryAdd(AttackCantrip);

        var twinned = false;

        if (actionMagicEffect is CharacterActionCastSpell actionCastSpell)
        {
            twinned = actionCastSpell.ActiveSpell.MetamagicOption == MetamagicTwinnedSpell;

            //mark this attack for proper integration with Quickened
            if (actionCastSpell.ActiveSpell.MetamagicOption == MetamagicQuickenedSpell)
            {
                attackMode.AttackTags.TryAdd(QuickenedAttackCantrip);
            }
        }

        var maxAttacks = 1 + (twinned ? 1 : 0);

        // this is required to support reaction scenarios where AttackMain won't work
        var actionId = attackMode.ActionType == ActionDefinitions.ActionType.Main
            ? ActionDefinitions.Id.AttackMain
            : ActionDefinitions.Id.AttackFree;

        foreach (var target in targets)
        {
            var attackActionParams =
                new CharacterActionParams(caster, actionId) { AttackMode = attackMode };

            attackActionParams.TargetCharacters.Add(target);
            attackActionParams.ActionModifiers.Add(new ActionModifier());
            attacks.Add(attackActionParams);

            if (attackActionParams.TargetCharacters.Count >= maxAttacks)
            {
                break;
            }
        }

        return attacks;
    }

    internal static bool CanBeUsedToAttack(
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

        var canAttack = CanAttack(caster, target);

        if (!canAttack)
        {
            failure = "Failure/&FailureFlagTargetMeleeWeaponError";
        }

        return canAttack;
    }
}
