using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal sealed class PerformAttackAfterMagicEffectUse : IPerformAttackAfterMagicEffectUse
{
    private const RuleDefinitions.RollOutcome MinOutcomeToAttack = RuleDefinitions.RollOutcome.Success;
    private const RuleDefinitions.RollOutcome MinSaveOutcomeToAttack = RuleDefinitions.RollOutcome.Failure;
    internal static readonly IPerformAttackAfterMagicEffectUse MeleeAttack = new PerformAttackAfterMagicEffectUse(1);

    internal static readonly IPerformAttackAfterMagicEffectUse MeleeAttackCanTwin =
        new PerformAttackAfterMagicEffectUse(2);

    private readonly int maxAttacks;

    private PerformAttackAfterMagicEffectUse(int maxAttacks)
    {
        this.maxAttacks = maxAttacks;
        CanAttack = CanMeleeAttack;
        CanBeUsedToAttack = DefaultCanUseHandler;
        PerformAttackAfterUse = DefaultAttackHandler;
    }

    public IPerformAttackAfterMagicEffectUse.CanUseHandler CanBeUsedToAttack { get; }
    public IPerformAttackAfterMagicEffectUse.GetAttackAfterUseHandler PerformAttackAfterUse { get; }
    public IPerformAttackAfterMagicEffectUse.CanAttackHandler CanAttack { get; }

    private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
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

        evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
            target.LocationPosition, attackModifier);

        return battleService.CanAttack(evalParams);
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

        if (caster == null || targets.Empty())
        {
            return attacks;
        }

        var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

        if (attackMode == null)
        {
            return attacks;
        }
        
        //get copy to be sure we don't break existing mode
        var tmp = RulesetAttackMode.AttackModesPool.Get();
        tmp.Copy(attackMode);
        attackMode = tmp;

        //set action type to be same as the one used for the magic effect
        attackMode.ActionType = effect.ActionType;

        var attackModifier = new ActionModifier();

        foreach (var target in targets.Where(t => CanMeleeAttack(caster, t)))
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

    private static bool DefaultCanUseHandler([NotNull] CursorLocationSelectTarget targeting,
        GameLocationCharacter caster,
        GameLocationCharacter target, [NotNull] out string failure)
    {
        failure = String.Empty;

        //TODO: implement setting to tell how many targets must meet weapon attack requirements
        var maxTargets = targeting.maxTargets;
        var remainingTargets = targeting.remainingTargets;
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
}
