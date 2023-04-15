using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

// ReSharper disable once CheckNamespace
namespace TA.AI.Activities;

public class BreakFreeSpellWeb : ActivityBase
{
    public static ContextType GetContextType(DecisionDefinition _)
    {
        return ContextType.Self;
    }

    public static void GetActionId(
        out ActionDefinitions.Id mainId,
        out ActionDefinitions.Id bonusId)
    {
        bonusId = ActionDefinitions.Id.AlwaysAvailable;
        mainId = ActionDefinitions.Id.AlwaysAvailable;
    }

    public override IEnumerator ExecuteImpl(
        AiLocationCharacter character,
        DecisionDefinition decisionDefinitionParam,
        DecisionContext context)
    {
        RulesetCondition restrainingCondition = null;

        var gameLocationCharacter = character.GameLocationCharacter;
        var rulesetCharacter = gameLocationCharacter?.RulesetCharacter;

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

        if (restrainingCondition != null)
        {
            var checkDC = 10;

            var sourceGuid = restrainingCondition.SourceGuid;

            if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetCharacterHero rulesetCharacterHero))
            {
                //TODO: find a better way to calculate this DC
                checkDC = 10 + rulesetCharacterHero.SpellRepertoires
                    .Select(x => x.SaveDC)
                    .Max();
            }

            var actionMod = new ActionModifier();

            rulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Strength, actionMod.AbilityCheckModifierTrends, string.Empty);

            gameLocationCharacter.ComputeAbilityCheckActionModifier(
                AttributeDefinitions.Strength, string.Empty, actionMod);

            gameLocationCharacter.RollAbilityCheck(
                AttributeDefinitions.Strength, string.Empty, checkDC, RuleDefinitions.AdvantageType.None, actionMod,
                false, -1, out var outcome, out var _, true);

            var breakFreeExecuted = rulesetCharacter.BreakFreeExecuted;

            breakFreeExecuted?.Invoke(rulesetCharacter, outcome == RuleDefinitions.RollOutcome.Success);

            if (outcome == RuleDefinitions.RollOutcome.Success)
            {
                rulesetCharacter.RemoveCondition(restrainingCondition);
            }
        }

        yield return null;
    }
}
