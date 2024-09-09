using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal sealed class AttackAfterMagicEffect : IFilterTargetingCharacter
{
    internal const string AttackAfterMagicEffectTag = "AttackAfterMagicEffectTag";
    private const RollOutcome MinOutcomeToAttack = RollOutcome.Success;
    private const RollOutcome MinSaveOutcomeToAttack = RollOutcome.Failure;

    public bool EnforceFullSelection => false;

    public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
    {
        // only enforce weapon reach or 5 ft on first target
        if (__instance.SelectionService.SelectedTargets.Count != 0)
        {
            return true;
        }

        if (CanAttack(__instance.ActionParams.ActingCharacter, target))
        {
            return true;
        }

        var text = Main.Settings.AllowBladeCantripsToUseReach ? "Feedback/&WithinReach" : "Feedback/&Within5Ft";

        __instance.actionModifier.FailureFlags.Add(Gui.Format("Tooltip/&TargetMeleeWeaponError", text));

        return false;
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

        return battleService.CanAttack(evalParams) &&
               (Main.Settings.AllowBladeCantripsToUseReach || caster.IsWithinRange(target, 1));
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

        //mark this attack for proper integration with polearm, and follow-up strike
        if (!actionParams.ActingCharacter.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            attackMode.AddAttackTagAsNeeded(AttackAfterMagicEffectTag);
        }

        // always use free attack
        var attackActionParams = new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
        {
            AttackMode = attackMode
        };

        attackActionParams.TargetCharacters.Add(targets[0]);
        attackActionParams.ActionModifiers.Add(new ActionModifier());
        attacks.Add(attackActionParams);

        return attacks;
    }
}
