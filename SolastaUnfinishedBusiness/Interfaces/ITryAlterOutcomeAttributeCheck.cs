using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttributeCheck
{
    [UsedImplicitly]
    IEnumerator OnTryAlterAttributeCheck(
        GameLocationBattleManager battleManager,
        AbilityCheckData abilityCheckData,
        GameLocationCharacter defender,
        GameLocationCharacter helper);
}

public sealed class AbilityCheckData
{
    public int AbilityCheckRoll { get; set; }
    public RollOutcome AbilityCheckRollOutcome { get; set; }
    public int AbilityCheckSuccessDelta { get; set; }
    public ActionModifier AbilityCheckActionModifier { get; set; }
    public CharacterAction Action { get; set; }
}

internal static class TryAlterOutcomeAttributeCheck
{
    // this is vanilla code converted to an IEnumerator
    public static IEnumerator ResolveRolls(
        GameLocationCharacter actor,
        GameLocationCharacter opponent,
        ActionDefinitions.Id actionId,
        AbilityCheckData abilityCheckData,
        AbilityCheckData opponentAbilityCheckData,
        string opponentAbilityScoreName = "")
    {
        var actionModifierActorStrength = new ActionModifier();
        var abilityCheckBonusActorStrength = actor.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Strength, actionModifierActorStrength.AbilityCheckModifierTrends,
            SkillDefinitions.Athletics);

        var contextFieldActor = 0;

        if (!actor.RulesetCharacter.IsWearingHeavyArmor())
        {
            contextFieldActor |= 64;
        }

        actor.ComputeAbilityCheckActionModifier(
            AttributeDefinitions.Strength, SkillDefinitions.Athletics, actionModifierActorStrength, contextFieldActor);

        actor.RulesetCharacter.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(
            actor.RulesetCharacter.FeaturesToBrowse, actor.RulesetCharacter.FeaturesOrigin);

        foreach (var key in actor.RulesetCharacter.FeaturesToBrowse)
        {
            foreach (var executionModifier in (key as IActionPerformanceProvider)!.ActionExecutionModifiers)
            {
                if (executionModifier.actionId != actionId ||
                    !actor.RulesetCharacter.IsMatchingEquipementCondition(executionModifier.equipmentContext) ||
                    executionModifier.advantageType == AdvantageType.None)
                {
                    continue;
                }

                var num = executionModifier.advantageType == AdvantageType.Advantage ? 1 : -1;
                var featureOrigin = actor.RulesetCharacter.FeaturesOrigin[key];

                actionModifierActorStrength.AbilityCheckAdvantageTrends.Add(new TrendInfo(
                    num, featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
            }
        }

        //
        // handle opponent
        //

        var actionModifierOpponentStrength = new ActionModifier();
        var actionModifierOpponentDexterity = new ActionModifier();

        var abilityCheckBonusOpponentStrength = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Strength, actionModifierOpponentStrength.AbilityCheckModifierTrends,
            SkillDefinitions.Athletics);
        var abilityCheckBonusOpponentDexterity = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
            AttributeDefinitions.Dexterity, actionModifierOpponentDexterity.AbilityCheckModifierTrends,
            SkillDefinitions.Acrobatics);

        var contextFieldOpponent = 1;

        if (!opponent.RulesetCharacter.IsWearingHeavyArmor())
        {
            contextFieldOpponent |= 64;
        }

        opponent.ComputeAbilityCheckActionModifier(AttributeDefinitions.Strength, SkillDefinitions.Athletics,
            actionModifierOpponentStrength, contextFieldOpponent);
        opponent.ComputeAbilityCheckActionModifier(AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics,
            actionModifierOpponentDexterity, contextFieldOpponent);

        var actionModifierOpponent = actionModifierOpponentStrength;
        var opponentBaseBonus = abilityCheckBonusOpponentStrength;
        var opponentProficiencyName = SkillDefinitions.Athletics;

        switch (opponentAbilityScoreName)
        {
            case "":
            {
                opponentAbilityScoreName = AttributeDefinitions.Strength;

                if (abilityCheckBonusOpponentDexterity + actionModifierOpponentDexterity.AbilityCheckModifier +
                    (actionModifierOpponentDexterity.AbilityCheckAdvantageTrend * 5) >
                    abilityCheckBonusOpponentStrength +
                    actionModifierOpponentStrength.AbilityCheckModifier +
                    (actionModifierOpponentStrength.AbilityCheckAdvantageTrend * 5))
                {
                    opponentAbilityScoreName = AttributeDefinitions.Dexterity;
                    opponentProficiencyName = SkillDefinitions.Acrobatics;
                    actionModifierOpponent = actionModifierOpponentDexterity;
                    opponentBaseBonus = abilityCheckBonusOpponentDexterity;
                }

                break;
            }
            case AttributeDefinitions.Dexterity:
                opponentProficiencyName = SkillDefinitions.Acrobatics;
                actionModifierOpponent = actionModifierOpponentDexterity;
                opponentBaseBonus = abilityCheckBonusOpponentDexterity;
                break;
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
            abilityCheckData,
            opponentAbilityCheckData);
    }

    private static int ExtendedRollDie(
        [NotNull] RulesetCharacter rulesetCharacter,
        DieType dieType,
        RollContext rollContext,
        bool isProficient,
        AdvantageType advantageType,
        bool enumerateFeatures,
        bool canRerollDice,
        string skill,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<TrendInfo> advantageTrends,
        List<TrendInfo> modifierTrends)
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
        List<TrendInfo> advantageTrends,
        List<TrendInfo> modifierTrends,
        RulesetCharacter opponent,
        int opponentBaseBonus,
        int opponentRollModifier,
        string opponentAbilityScoreName,
        string opponentProficiencyName,
        List<TrendInfo> opponentAdvantageTrends,
        List<TrendInfo> opponentModifierTrends,
        AbilityCheckData abilityCheckData,
        AbilityCheckData opponentAbilityCheckData,
        bool notify = true)
    {
        var advantageActor = ComputeAdvantage(advantageTrends);
        var isProficientActor = rulesetCharacter.IsProficient(proficiencyName);

        foreach (var modifierTrend in modifierTrends)
        {
            if (modifierTrend.dieFlag == TrendInfoDieFlag.None ||
                modifierTrend.value <= 0)
            {
                continue;
            }

            var abilityCheckDieRolled = rulesetCharacter.AdditionalAbilityCheckDieRolled;

            abilityCheckDieRolled?.Invoke(rulesetCharacter, modifierTrend);
        }

        var rawRoll = ExtendedRollDie(
            rulesetCharacter,
            DieType.D20, RollContext.AbilityCheck,
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

        var advantageOpponent = ComputeAdvantage(opponentAdvantageTrends);
        var isProficientOpponent = opponent.IsProficient(opponentProficiencyName);

        foreach (var opponentModifierTrend in opponentModifierTrends)
        {
            if (opponentModifierTrend.dieFlag == TrendInfoDieFlag.None ||
                opponentModifierTrend.value <= 0)
            {
                continue;
            }

            var abilityCheckDieRolled = opponent.AdditionalAbilityCheckDieRolled;

            abilityCheckDieRolled?.Invoke(opponent, opponentModifierTrend);
        }

        var opponentRawRoll = ExtendedRollDie(
            opponent,
            DieType.D20, RollContext.AbilityCheck,
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
        var actionModifierHero = new ActionModifier();

        abilityCheckData.AbilityCheckRoll = rawRoll;
        abilityCheckData.AbilityCheckSuccessDelta = totalRoll - opponentTotalRoll;
        abilityCheckData.AbilityCheckRollOutcome = totalRoll != DiceMinValue[8]
            ? totalRoll <= opponentTotalRoll ? RollOutcome.Failure :
            rawRoll != DiceMaxValue[8] ? RollOutcome.Success :
            RollOutcome.CriticalSuccess
            : RollOutcome.CriticalFailure;
        abilityCheckData.AbilityCheckActionModifier = actionModifierHero;

        yield return HandleITryAlterOutcomeAttributeCheck(
            GameLocationCharacter.GetFromActor(rulesetCharacter), abilityCheckData);

        totalRoll = totalRoll - rawRoll + abilityCheckData.AbilityCheckRoll +
                    abilityCheckData.AbilityCheckActionModifier.AbilityCheckModifier;
        rawRoll = abilityCheckData.AbilityCheckRoll;
        modifierTrends.AddRange(abilityCheckData.AbilityCheckActionModifier.AbilityCheckModifierTrends);

        // handle opponent interruptions
        var actionModifierEnemy = new ActionModifier();

        opponentAbilityCheckData.AbilityCheckRoll = opponentRawRoll;
        opponentAbilityCheckData.AbilityCheckSuccessDelta = opponentTotalRoll - rawRoll;
        opponentAbilityCheckData.AbilityCheckRollOutcome = opponentRawRoll != DiceMinValue[8]
            ? opponentTotalRoll <= totalRoll ? RollOutcome.Failure :
            opponentRawRoll != DiceMaxValue[8] ? RollOutcome.Success :
            RollOutcome.CriticalSuccess
            : RollOutcome.CriticalFailure;
        opponentAbilityCheckData.AbilityCheckActionModifier = actionModifierEnemy;

        yield return HandleITryAlterOutcomeAttributeCheck(
            GameLocationCharacter.GetFromActor(opponent), opponentAbilityCheckData);

        opponentTotalRoll = opponentTotalRoll - opponentRawRoll + opponentAbilityCheckData.AbilityCheckRoll +
                            opponentAbilityCheckData.AbilityCheckActionModifier.AbilityCheckModifier;
        opponentRawRoll = opponentAbilityCheckData.AbilityCheckRoll;
        opponentModifierTrends.AddRange(opponentAbilityCheckData.AbilityCheckActionModifier.AbilityCheckModifierTrends);

        // calculate final results
        abilityCheckData.AbilityCheckRoll = rawRoll;
        abilityCheckData.AbilityCheckSuccessDelta = totalRoll - opponentTotalRoll;
        abilityCheckData.AbilityCheckRollOutcome = totalRoll != DiceMinValue[8]
            ? totalRoll <= opponentTotalRoll ? RollOutcome.Failure :
            rawRoll != DiceMaxValue[8] ? RollOutcome.Success :
            RollOutcome.CriticalSuccess
            : RollOutcome.CriticalFailure;

        opponentAbilityCheckData.AbilityCheckRoll = opponentRawRoll;
        opponentAbilityCheckData.AbilityCheckSuccessDelta = opponentTotalRoll - rawRoll;
        opponentAbilityCheckData.AbilityCheckRollOutcome = opponentRawRoll != DiceMinValue[8]
            ? opponentTotalRoll <= totalRoll ? RollOutcome.Failure :
            opponentRawRoll != DiceMaxValue[8] ? RollOutcome.Success :
            RollOutcome.CriticalSuccess
            : RollOutcome.CriticalFailure;

        rulesetCharacter.ProcessConditionsMatchingInterruption(ConditionInterruption.AbilityCheck);
        opponent.ProcessConditionsMatchingInterruption(ConditionInterruption.AbilityCheck);

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
        AbilityCheckData abilityCheckData)
    {
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
            as GameLocationBattleManager;

        yield return HandleBardicRollOnFailure(battleManager, actingCharacter, abilityCheckData);

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToArray())
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
                    battleManager, abilityCheckData, actingCharacter, unit);
            }
        }
    }

    private static IEnumerator HandleBardicRollOnFailure(
        GameLocationBattleManager battleManager,
        GameLocationCharacter actingCharacter,
        AbilityCheckData abilityCheckData)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        if (abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure)
        {
            yield break;
        }

        battleManager.GetBestParametersForBardicDieRoll(
            actingCharacter,
            out var bestDie,
            out var bestModifier,
            out var sourceCondition,
            out var forceMaxRoll,
            out var advantage);

        if (bestDie <= DieType.D1 ||
            actingCharacter.RulesetCharacter == null ||
            DiceMaxValue[(int)bestDie] < Mathf.Abs(abilityCheckData.AbilityCheckSuccessDelta))
        {
            yield break;
        }

        var reactionParams =
            new CharacterActionParams(actingCharacter, ActionDefinitions.Id.UseBardicInspiration)
            {
                IntParameter = (int)bestDie, IntParameter2 = (int)BardicInspirationUsageType.AbilityCheck
            };
        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToUseBardicInspiration(reactionParams);

        yield return battleManager.WaitForReactions(actingCharacter, actionService, previousReactionCount);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        var roll = actingCharacter.RulesetCharacter.RollBardicInspirationDie(
            sourceCondition, abilityCheckData.AbilityCheckSuccessDelta, forceMaxRoll, advantage);

        abilityCheckData.AbilityCheckSuccessDelta += roll;

        var action = abilityCheckData.Action;

        if (action != null)
        {
            action.BardicDieType = bestDie;
            action.FeatureName = bestModifier.Name;
        }

        var actionModifier = abilityCheckData.AbilityCheckActionModifier;

        if (actionModifier != null)
        {
            actionModifier.AbilityCheckModifier += roll;
            actionModifier.AbilityCheckModifierTrends.Add(new TrendInfo(
                roll, FeatureSourceType.CharacterFeature,
                DatabaseHelper.FeatureDefinitionPowers.PowerBardGiveBardicInspiration.Name, null));
        }

        // change roll to success if appropriate
        if (abilityCheckData.AbilityCheckSuccessDelta >= 0)
        {
            abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
        }
    }
}
