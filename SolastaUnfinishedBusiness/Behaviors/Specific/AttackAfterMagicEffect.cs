using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal sealed class AttackAfterMagicEffect : IFilterTargetingCharacter
{
    private const string AttackCantrip = "AttackCantrip";
    private const string QuickenedAttackCantrip = "QuickenedAttackCantrip";
    private const string ReplaceAttackCantrip = "ReplaceAttackCantrip";
    private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
    private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

    public bool EnforceFullSelection => false;

    public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
    {
        var isValid = CanAttack(__instance.ActionParams.ActingCharacter, target);

        if (!isValid)
        {
            __instance.actionModifier.FailureFlags.Add("Tooltip/&TargetMeleeWeaponError");
        }

        return isValid;
    }

    internal static void HandleAttackAfterMagicEffect(GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        if (actionParams.AttackMode == null)
        {
            return;
        }

        var attackTags = actionParams.AttackMode.AttackTags;

        if (!attackTags.Contains(AttackCantrip))
        {
            return;
        }

        // this is required with replace cantrip scenarios
        character.UsedMainCantrip = true;

        var isReplaceAttackWithCantrip = attackTags.Contains(ReplaceAttackCantrip);

        if (attackTags.Contains(QuickenedAttackCantrip))
        {
            character.UsedMainSpell = true;
            character.SpendActionType(ActionDefinitions.ActionType.Bonus);
        }
        else if (!isReplaceAttackWithCantrip)
        {
            character.SpendActionType(ActionDefinitions.ActionType.Main);
        }

        if (isReplaceAttackWithCantrip)
        {
            return;
        }

        // by now Followup Strike or Polearm already triggered. below fixes stats so bonus attack isn't offered
        character.HasAttackedSinceLastTurn = false;
        character.RulesetCharacter.ExecutedAttacks--;
        character.RulesetCharacter.RefreshAttackModes();
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

        //mark this attack for proper integration with Replace Attack with cantrip
        if (actionParams.ActingCharacter.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            attackMode.AttackTags.TryAdd(ReplaceAttackCantrip);
        }

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
}
