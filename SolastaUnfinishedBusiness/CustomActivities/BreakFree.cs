using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

// ReSharper disable once CheckNamespace
namespace TA.AI.Activities;

[UsedImplicitly]
public class BreakFree : ActivityBase
{
    [UsedImplicitly]
    public static ContextType GetContextType(DecisionDefinition _)
    {
        return ContextType.Self;
    }

    [UsedImplicitly]
    public static void GetActionId(
        out ActionDefinitions.Id mainId,
        out ActionDefinitions.Id bonusId)
    {
        bonusId = ActionDefinitions.Id.NoAction;
        mainId = ActionDefinitions.Id.BreakFree;
    }

    public override IEnumerator ExecuteImpl(
        AiLocationCharacter character,
        DecisionDefinition decisionDefinitionParam,
        DecisionContext context)
    {
        RulesetCondition restrainingCondition = null;

        var gameLocationCharacter = character.GameLocationCharacter;
        var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

        if (rulesetCharacter == null)
        {
            yield break;
        }

        foreach (var definitionActionAffinity in rulesetCharacter
                     .GetFeaturesByType<FeatureDefinitionActionAffinity>()
                     .Where(x => x.AuthorizedActions.Contains(ActionDefinitions.Id.BreakFree)))
        {
            restrainingCondition = rulesetCharacter.FindFirstConditionHoldingFeature(definitionActionAffinity);
        }

        if (restrainingCondition == null)
        {
            yield break;
        }

        var success = true;

        // no ability check
        if (!decisionDefinitionParam.Decision.boolParameter)
        {
            rulesetCharacter.RemoveCondition(restrainingCondition);
        }
        else
        {
            var checkDC = 10;
            var sourceGuid = restrainingCondition.SourceGuid;

            if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetCharacterHero rulesetCharacterHero))
            {
                checkDC = rulesetCharacterHero.SpellRepertoires
                    .Select(x => x.SaveDC)
                    .Max();
            }

            var actionMod = new ActionModifier();

            rulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Strength, actionMod.AbilityCheckModifierTrends, string.Empty);

            gameLocationCharacter.ComputeAbilityCheckActionModifier(
                AttributeDefinitions.Strength, string.Empty, actionMod);

            gameLocationCharacter.RollAbilityCheck(
                AttributeDefinitions.Strength, string.Empty, checkDC, AdvantageType.None, actionMod,
                false, -1, out var outcome, out _, true);

            success = outcome is RollOutcome.Success or RollOutcome.CriticalSuccess;

            if (success)
            {
                rulesetCharacter.RemoveCondition(restrainingCondition);
            }
        }

        gameLocationCharacter.SpendActionType(ActionDefinitions.ActionType.Main);

        var breakFreeExecuted = rulesetCharacter.BreakFreeExecuted;

        breakFreeExecuted?.Invoke(rulesetCharacter, success);
    }
}
