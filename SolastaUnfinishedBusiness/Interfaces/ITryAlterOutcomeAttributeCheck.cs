using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttributeCheck
{
    [UsedImplicitly]
    IEnumerator OnTryAlterAttributeCheck(
        GameLocationBattleManager battleManager,
        AbilityCheckData abilityCheckData,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier abilityCheckModifier);
}

public sealed class AbilityCheckData
{
    public int AbilityCheckRoll { get; set; }
    public RuleDefinitions.RollOutcome AbilityCheckRollOutcome { get; set; }
    public int AbilityCheckSuccessDelta { get; set; }
}

internal static class TryAlterOutcomeAttributeCheck
{
    // this is vanilla code converted to an IEnumerator
    public static IEnumerator ResolveRolls(
        GameLocationCharacter actor,
        GameLocationCharacter opponent,
        ActionDefinitions.Id actionId,
        AbilityCheckData abilityCheckData)
    {
        var actionModifierActorStrength = new ActionModifier();
        var actionModifierOpponentStrength = new ActionModifier();
        var actionModifierOpponentDexterity = new ActionModifier();
        var abilityCheckBonusActorStrength = actor.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Strength, actionModifierActorStrength.AbilityCheckModifierTrends,
            SkillDefinitions.Athletics);
        var abilityCheckBonusOpponentStrength = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Strength, actionModifierOpponentStrength.AbilityCheckModifierTrends,
            SkillDefinitions.Athletics);
        var abilityCheckBonusOpponentDexterity = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Dexterity, actionModifierOpponentDexterity.AbilityCheckModifierTrends,
            SkillDefinitions.Acrobatics);

        var contextFieldActor = 0;

        if (!actor.RulesetCharacter.IsWearingHeavyArmor())
        {
            contextFieldActor |= 64;
        }

        actor.ComputeAbilityCheckActionModifier(AttributeDefinitions.Strength, SkillDefinitions.Athletics,
            actionModifierActorStrength, contextFieldActor);

        var contextFieldOpponent = 1;

        if (!opponent.RulesetCharacter.IsWearingHeavyArmor())
        {
            contextFieldOpponent |= 64;
        }

        opponent.ComputeAbilityCheckActionModifier(AttributeDefinitions.Strength, SkillDefinitions.Athletics,
            actionModifierOpponentStrength, contextFieldOpponent);
        opponent.ComputeAbilityCheckActionModifier(AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics,
            actionModifierOpponentDexterity, contextFieldOpponent);

        actor.RulesetCharacter.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(
            actor.RulesetCharacter.FeaturesToBrowse, actor.RulesetCharacter.FeaturesOrigin);

        foreach (var key in actor.RulesetCharacter.FeaturesToBrowse)
        {
            foreach (var executionModifier in (key as IActionPerformanceProvider)!.ActionExecutionModifiers)
            {
                if (executionModifier.actionId != actionId ||
                    !actor.RulesetCharacter.IsMatchingEquipementCondition(executionModifier.equipmentContext) ||
                    executionModifier.advantageType == RuleDefinitions.AdvantageType.None)
                {
                    continue;
                }

                var num = executionModifier.advantageType == RuleDefinitions.AdvantageType.Advantage ? 1 : -1;
                var featureOrigin = actor.RulesetCharacter.FeaturesOrigin[key];

                actionModifierActorStrength.AbilityCheckAdvantageTrends.Add(new RuleDefinitions.TrendInfo(
                    num, featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
            }
        }

        var opponentAbilityScoreName = AttributeDefinitions.Strength;
        var opponentProficiencyName = SkillDefinitions.Athletics;
        var actionModifierOpponent = actionModifierOpponentStrength;
        var opponentBaseBonus = abilityCheckBonusOpponentStrength;

        if (abilityCheckBonusOpponentDexterity + actionModifierOpponentDexterity.AbilityCheckModifier +
            (actionModifierOpponentDexterity.AbilityCheckAdvantageTrend * 5) > abilityCheckBonusOpponentStrength +
            actionModifierOpponentStrength.AbilityCheckModifier +
            (actionModifierOpponentStrength.AbilityCheckAdvantageTrend * 5))
        {
            opponentAbilityScoreName = AttributeDefinitions.Dexterity;
            opponentProficiencyName = SkillDefinitions.Acrobatics;
            actionModifierOpponent = actionModifierOpponentDexterity;
            opponentBaseBonus = abilityCheckBonusOpponentDexterity;
        }

        yield return ResolveContestCheck(
            actor.RulesetCharacter,
            abilityCheckBonusActorStrength,
            actionModifierActorStrength.AbilityCheckModifier,
            AttributeDefinitions.Strength,
            SkillDefinitions.Athletics,
            actionModifierActorStrength.AbilityCheckAdvantageTrends,
            actionModifierActorStrength.AbilityCheckModifierTrends,
            opponent.RulesetCharacter,
            opponentBaseBonus,
            actionModifierOpponent.AbilityCheckModifier,
            opponentAbilityScoreName,
            opponentProficiencyName,
            actionModifierOpponent.AbilityCheckAdvantageTrends,
            actionModifierOpponent.AbilityCheckModifierTrends,
            abilityCheckData);
    }

    private static int ExtendedRollDie(
        [NotNull] RulesetCharacter rulesetCharacter,
        RuleDefinitions.DieType dieType,
        RuleDefinitions.RollContext rollContext,
        bool isProficient,
        RuleDefinitions.AdvantageType advantageType,
        bool enumerateFeatures,
        bool canRerollDice,
        string skill,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        List<RuleDefinitions.TrendInfo> modifierTrends)
    {
        var minRoll = 0;

        foreach (var modifyAbilityCheck in rulesetCharacter.GetSubFeaturesByType<IModifyAbilityCheck>())
        {
            modifyAbilityCheck.MinRoll(
                rulesetCharacter, baseBonus, abilityScoreName, proficiencyName,
                advantageTrends, modifierTrends, ref rollModifier, ref minRoll);
        }

        var roll = rulesetCharacter.RollDie(
            dieType, rollContext, isProficient, advantageType, out _, out _,
            enumerateFeatures, canRerollDice, skill);

        return Math.Max(minRoll, roll);
    }

    // this is vanilla code converted to an IEnumerator and a call to allow reactions on context checks
    internal static IEnumerator ResolveContestCheck(
        RulesetCharacter rulesetCharacter,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        List<RuleDefinitions.TrendInfo> modifierTrends,
        RulesetCharacter opponent,
        int opponentBaseBonus,
        int opponentRollModifier,
        string opponentAbilityScoreName,
        string opponentProficiencyName,
        List<RuleDefinitions.TrendInfo> opponentAdvantageTrends,
        List<RuleDefinitions.TrendInfo> opponentModifierTrends,
        AbilityCheckData abilityCheckData,
        bool notify = true)
    {
        var advantageActor = RuleDefinitions.ComputeAdvantage(advantageTrends);
        var isProficientActor = rulesetCharacter.IsProficient(proficiencyName);

        foreach (var modifierTrend in modifierTrends)
        {
            if (modifierTrend.dieFlag == RuleDefinitions.TrendInfoDieFlag.None ||
                modifierTrend.value <= 0)
            {
                continue;
            }

            var abilityCheckDieRolled = rulesetCharacter.AdditionalAbilityCheckDieRolled;

            abilityCheckDieRolled?.Invoke(rulesetCharacter, modifierTrend);
        }

        var rawRoll = ExtendedRollDie(
            rulesetCharacter,
            RuleDefinitions.DieType.D20, RuleDefinitions.RollContext.AbilityCheck,
            isProficientActor,
            advantageActor,
            true,
            true,
            proficiencyName,
            baseBonus,
            rollModifier,
            abilityScoreName,
            proficiencyName,
            advantageTrends,
            modifierTrends);

        var advantageOpponent = RuleDefinitions.ComputeAdvantage(opponentAdvantageTrends);
        var isProficientOpponent = opponent.IsProficient(opponentProficiencyName);

        foreach (var opponentModifierTrend in opponentModifierTrends)
        {
            if (opponentModifierTrend.dieFlag == RuleDefinitions.TrendInfoDieFlag.None ||
                opponentModifierTrend.value <= 0)
            {
                continue;
            }

            var abilityCheckDieRolled = opponent.AdditionalAbilityCheckDieRolled;

            abilityCheckDieRolled?.Invoke(opponent, opponentModifierTrend);
        }

        var opponentRawRoll = ExtendedRollDie(
            opponent,
            RuleDefinitions.DieType.D20, RuleDefinitions.RollContext.AbilityCheck,
            isProficientOpponent,
            advantageOpponent,
            true,
            true,
            proficiencyName,
            opponentBaseBonus,
            opponentRollModifier,
            opponentAbilityScoreName,
            opponentProficiencyName,
            opponentAdvantageTrends,
            opponentModifierTrends);

        var totalRoll = baseBonus + rawRoll + rollModifier;
        var opponentTotalRoll = opponentBaseBonus + opponentRawRoll + opponentRollModifier;

        // handle actor interruptions
        abilityCheckData.AbilityCheckRoll = rawRoll;
        abilityCheckData.AbilityCheckSuccessDelta = totalRoll - opponentTotalRoll;
        abilityCheckData.AbilityCheckRollOutcome = totalRoll != RuleDefinitions.DiceMinValue[8]
            ? totalRoll <= opponentTotalRoll ? RuleDefinitions.RollOutcome.Failure :
            rawRoll != RuleDefinitions.DiceMaxValue[8] ? RuleDefinitions.RollOutcome.Success :
            RuleDefinitions.RollOutcome.CriticalSuccess
            : RuleDefinitions.RollOutcome.CriticalFailure;

        yield return HandleITryAlterOutcomeAttributeCheck(
            GameLocationCharacter.GetFromActor(rulesetCharacter), abilityCheckData, new ActionModifier());

        totalRoll = totalRoll - rawRoll + abilityCheckData.AbilityCheckRoll;
        rawRoll = abilityCheckData.AbilityCheckRoll;

        // handle opponent interruptions
        abilityCheckData.AbilityCheckRoll = opponentRawRoll;
        abilityCheckData.AbilityCheckSuccessDelta = opponentRawRoll - totalRoll;
        abilityCheckData.AbilityCheckRollOutcome = opponentRawRoll != RuleDefinitions.DiceMinValue[8]
            ? opponentRawRoll <= totalRoll ? RuleDefinitions.RollOutcome.Failure :
            opponentRawRoll != RuleDefinitions.DiceMaxValue[8] ? RuleDefinitions.RollOutcome.Success :
            RuleDefinitions.RollOutcome.CriticalSuccess
            : RuleDefinitions.RollOutcome.CriticalFailure;

        yield return HandleITryAlterOutcomeAttributeCheck(
            GameLocationCharacter.GetFromActor(opponent), abilityCheckData, new ActionModifier());

        opponentTotalRoll = opponentTotalRoll - opponentRawRoll + abilityCheckData.AbilityCheckRoll;
        opponentRawRoll = abilityCheckData.AbilityCheckRoll;

        // calculate final results
        abilityCheckData.AbilityCheckRoll = rawRoll;
        abilityCheckData.AbilityCheckSuccessDelta = totalRoll - opponentTotalRoll;
        abilityCheckData.AbilityCheckRollOutcome = totalRoll != RuleDefinitions.DiceMinValue[8]
            ? totalRoll <= opponentTotalRoll ? RuleDefinitions.RollOutcome.Failure :
            rawRoll != RuleDefinitions.DiceMaxValue[8] ? RuleDefinitions.RollOutcome.Success :
            RuleDefinitions.RollOutcome.CriticalSuccess
            : RuleDefinitions.RollOutcome.CriticalFailure;

        rulesetCharacter.ProcessConditionsMatchingInterruption(RuleDefinitions.ConditionInterruption.AbilityCheck);
        opponent.ProcessConditionsMatchingInterruption(RuleDefinitions.ConditionInterruption.AbilityCheck);

        if (notify)
        {
            rulesetCharacter.ContestCheckRolled?.Invoke(
                rulesetCharacter,
                opponent,
                abilityScoreName,
                proficiencyName,
                opponentAbilityScoreName,
                opponentProficiencyName,
                abilityCheckData.AbilityCheckRollOutcome,
                totalRoll,
                rawRoll,
                opponentTotalRoll,
                opponentRawRoll,
                advantageTrends,
                modifierTrends,
                opponentAdvantageTrends,
                opponentModifierTrends);
        }
    }

    internal static IEnumerator HandleITryAlterOutcomeAttributeCheck(
        GameLocationCharacter actingCharacter,
        AbilityCheckData abilityCheckData,
        ActionModifier actionModifier)
    {
        yield return HandleBardicRollOnFailure(actingCharacter, abilityCheckData);

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
            as GameLocationBattleManager;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            var hasUnit =
                actionService.PendingReactionRequestGroups.Count > 0 &&
                actionService.PendingReactionRequestGroups.Peek().Requests
                    .Any(x => x.Character == unit);

            if (hasUnit)
            {
                continue;
            }

            foreach (var tryAlterOutcomeAttributeCheck in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttributeCheck>())
            {
                yield return tryAlterOutcomeAttributeCheck.OnTryAlterAttributeCheck(
                    battleManager, abilityCheckData, actingCharacter, unit, actionModifier);
            }
        }
    }

    private static IEnumerator HandleBardicRollOnFailure(
        GameLocationCharacter actingCharacter, AbilityCheckData abilityCheckData)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
            as GameLocationBattleManager;

        if (abilityCheckData.AbilityCheckRollOutcome != RuleDefinitions.RollOutcome.Failure)
        {
            yield break;
        }

        battleManager!.GetBestParametersForBardicDieRoll(
            actingCharacter,
            out var bestDie,
            out _,
            out var sourceCondition,
            out var forceMaxRoll,
            out var advantage);

        if (bestDie <= RuleDefinitions.DieType.D1 ||
            actingCharacter.RulesetCharacter == null)
        {
            yield break;
        }

        // Is the die enough to overcome the failure?
        if (RuleDefinitions.DiceMaxValue[(int)bestDie] < Mathf.Abs(abilityCheckData.AbilityCheckSuccessDelta))
        {
            yield break;
        }

        var reactionParams =
            new CharacterActionParams(actingCharacter, ActionDefinitions.Id.UseBardicInspiration)
            {
                IntParameter = (int)bestDie,
                IntParameter2 = (int)RuleDefinitions.BardicInspirationUsageType.AbilityCheck
            };
        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToUseBardicInspiration(reactionParams);

        yield return battleManager.WaitForReactions(actingCharacter, actionService, previousReactionCount);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        // Now we have a shot at succeeding on the ability check
        var roll = actingCharacter.RulesetCharacter.RollBardicInspirationDie(
            sourceCondition, abilityCheckData.AbilityCheckSuccessDelta, forceMaxRoll, advantage);

        if (roll < Mathf.Abs(abilityCheckData.AbilityCheckSuccessDelta))
        {
            yield break;
        }

        // The roll is now a success!
        abilityCheckData.AbilityCheckSuccessDelta += roll;
        abilityCheckData.AbilityCheckRollOutcome = RuleDefinitions.RollOutcome.Success;
    }
}
