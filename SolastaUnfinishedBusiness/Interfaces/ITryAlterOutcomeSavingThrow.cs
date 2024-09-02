using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeSavingThrow
{
    IEnumerator OnTryAlterOutcomeSavingThrow(
        GameLocationBattleManager battleManager,
        [CanBeNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        SavingThrowData savingThrowData,
        bool hasHitVisual);
}

public sealed class SavingThrowData
{
    public RollOutcome SaveOutcome { get; set; }
    public int SaveOutcomeDelta { get; set; }
    public ActionModifier SaveActionModifier { get; set; }
    public string SavingThrowAbility { get; set; }
    public int SaveDC { get; set; }
    public int SaveBonusAndRollModifier { get; set; }
    public BaseDefinition SourceDefinition { get; set; }
    public EffectDescription EffectDescription { get; set; }
    public string Title { get; set; }
    [CanBeNull] public CharacterAction Action { get; set; }
}

internal static class TryAlterOutcomeSavingThrow
{
    internal static IEnumerator Handler(
        GameLocationBattleManager battleManager,
        [CanBeNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        SavingThrowData savingThrowData,
        bool hasBorrowedLuck,
        EffectDescription effectDescription)
    {
        // Legendary Resistance or Indomitable?
        if (savingThrowData.SaveOutcome == RollOutcome.Failure)
        {
            yield return HandleFailedSavingThrow(
                battleManager, attacker, defender, savingThrowData, false, hasBorrowedLuck);
        }

        //PATCH: support for `ITryAlterOutcomeSavingThrow`
        foreach (var tryAlterOutcomeSavingThrow in TryAlterOutcomeSavingThrowHandler(
                     battleManager, attacker, defender, savingThrowData, false))
        {
            yield return tryAlterOutcomeSavingThrow;

            if (savingThrowData.Action == null)
            {
                continue;
            }

            savingThrowData.Action.SaveOutcome = savingThrowData.SaveOutcome;
            savingThrowData.Action.SaveOutcomeDelta = savingThrowData.SaveOutcomeDelta;
        }

        defender.RulesetActor.GrantConditionOnSavingThrowOutcome(effectDescription, savingThrowData.SaveOutcome, true);
    }

    private static IEnumerable TryAlterOutcomeSavingThrowHandler(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        SavingThrowData savingThrowData,
        bool hasHitVisual)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeSavingThrow>())
            {
                yield return feature.OnTryAlterOutcomeSavingThrow(
                    battleManager, attacker, defender, unit, savingThrowData, hasHitVisual);
            }
        }
    }

    internal static void TryRerollSavingThrow(
        [CanBeNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        SavingThrowData savingThrowData,
        bool hasHitVisual)
    {
        var action = savingThrowData.Action;
        var saveOutcome = RollOutcome.Neutral;
        var saveOutcomeDelta = 0;

        // save comes from a gadget
        if (action == null)
        {
            var actionModifier = new ActionModifier();
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            implementationService.TryRollSavingThrow(
                null,
                Side.Enemy,
                defender.RulesetActor,
                actionModifier,
                false,
                true,
                savingThrowData.SavingThrowAbility,
                savingThrowData.SaveDC,
                false,
                false,
                false,
                FeatureSourceType.Base,
                [],
                null,
                null,
                string.Empty,
                savingThrowData.SourceDefinition,
                string.Empty,
                null,
                out saveOutcome,
                out saveOutcomeDelta);
        }
        else
        {
            // should never happen
            if (attacker == null)
            {
                return;
            }

            if (action.ActionParams.AttackMode != null)
            {
                action.ActionParams.AttackMode.TryRollSavingThrow(
                    attacker.RulesetCharacter,
                    defender.RulesetActor,
                    savingThrowData.SaveActionModifier,
                    action.ActionParams.AttackMode.EffectDescription.EffectForms,
                    out saveOutcome, out saveOutcomeDelta);
            }
            else
            {
                action.ActionParams.RulesetEffect?.TryRollSavingThrow(
                    attacker.RulesetCharacter,
                    attacker.Side,
                    defender.RulesetActor,
                    savingThrowData.SaveActionModifier,
                    action.ActionParams.RulesetEffect.EffectDescription.EffectForms, hasHitVisual,
                    out saveOutcome, out saveOutcomeDelta);
            }
        }

        savingThrowData.SaveOutcome = saveOutcome;
        savingThrowData.SaveOutcomeDelta = saveOutcomeDelta;
    }

    private static IEnumerator HandleFailedSavingThrow(
        GameLocationBattleManager battleManager,
        [CanBeNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        SavingThrowData savingThrowData,
        bool hasHitVisual,
        bool hasBorrowedLuck)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        CharacterActionParams reactionParams;
        int count;

        if (defender.HasLegendaryResistances &&
            defender.Side == Side.Enemy)
        {
            reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UseLegendaryResistance);
            count = actionService.PendingReactionRequestGroups.Count;
            actionService.ReactToLegendaryResistSavingThrow(reactionParams);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (reactionParams.ReactionValidated)
            {
                savingThrowData.SaveOutcome = RollOutcome.Success;
            }
        }

        if (savingThrowData.SaveOutcome == RollOutcome.Failure &&
            defender.HasIndomitableResistances)
        {
            reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UseIndomitableResistance);
            count = actionService.PendingReactionRequestGroups.Count;
            actionService.ReactToIndomitableResistSavingThrow(reactionParams);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (reactionParams.ReactionValidated)
            {
                TryRerollSavingThrow(attacker, defender, savingThrowData, hasHitVisual);
            }
        }

        if (savingThrowData.SaveOutcome == RollOutcome.Failure &&
            defender.CanBorrowLuck() &&
            !hasBorrowedLuck &&
            ComputeAdvantage(savingThrowData.SaveActionModifier.SavingThrowAdvantageTrends) !=
            AdvantageType.Disadvantage)
        {
            reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.BorrowLuck);
            count = actionService.PendingReactionRequestGroups.Count;
            actionService.ReactToBorrowLuck(reactionParams);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (reactionParams.ReactionValidated)
            {
                TryRerollSavingThrow(attacker, defender, savingThrowData, hasHitVisual);

                if (savingThrowData.SaveOutcome == RollOutcome.Success)
                {
                    defender.RulesetCharacter.AddConditionOfCategory(
                        AttributeDefinitions.TagCombat,
                        RulesetCondition.CreateActiveCondition(
                            defender.Guid,
                            ConditionDomainMischiefBorrowedLuck,
                            DurationType.Dispelled,
                            0,
                            TurnOccurenceType.StartOfTurn,
                            defender.Guid,
                            defender.RulesetCharacter.CurrentFaction.Name));
                }
            }
        }

        if (savingThrowData.SaveOutcome == RollOutcome.Failure &&
            defender.CanUseDiamondSoul())
        {
            reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DiamondSoul);
            count = actionService.PendingReactionRequestGroups.Count;
            actionService.ReactToDiamondSoul(reactionParams);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (reactionParams.ReactionValidated)
            {
                TryRerollSavingThrow(attacker, defender, savingThrowData, hasHitVisual);
            }
        }

        if (savingThrowData.SaveOutcome != RollOutcome.Failure)
        {
            yield break;
        }

        battleManager.GetBestParametersForBardicDieRoll(
            defender,
            out var bestDie,
            out var bestModifier,
            out var sourceCondition,
            out var forceMaxRoll,
            out var advantage);

        if (bestDie <= DieType.D1 ||
            defender.RulesetCharacter == null ||
            DiceMaxValue[(int)bestDie] < Mathf.Abs(savingThrowData.SaveOutcomeDelta))
        {
            yield break;
        }

        reactionParams =
            new CharacterActionParams(defender, ActionDefinitions.Id.UseBardicInspiration)
            {
                IntParameter = (int)bestDie, IntParameter2 = (int)BardicInspirationUsageType.SavingThrow
            };

        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToUseBardicInspiration(reactionParams);

        yield return battleManager.WaitForReactions(defender, actionService, previousReactionCount);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        var roll = defender.RulesetCharacter.RollBardicInspirationDie(
            sourceCondition, savingThrowData.SaveOutcomeDelta, forceMaxRoll, advantage);

        savingThrowData.SaveOutcomeDelta += roll;

        var action = savingThrowData.Action;

        if (action != null)
        {
            action.BardicDieType = bestDie;
            action.FeatureName = bestModifier.Name;
        }

        var actionModifier = savingThrowData.SaveActionModifier;

        if (actionModifier != null)
        {
            actionModifier.SavingThrowModifier += roll;
            actionModifier.SavingThrowModifierTrends.Add(new TrendInfo(
                roll, FeatureSourceType.CharacterFeature, PowerBardGiveBardicInspiration.Name, null));
        }

        // change roll to success if appropriate
        if (savingThrowData.SaveOutcomeDelta >= 0)
        {
            savingThrowData.SaveOutcome = RollOutcome.Success;
        }
    }
}
