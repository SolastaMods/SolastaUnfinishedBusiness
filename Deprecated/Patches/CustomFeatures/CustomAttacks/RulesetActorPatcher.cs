using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttacks;

//
// IConditionRemovedOnSourceTurnStart
//
[HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_ProcessConditionsMatchingOccurenceType
{
    internal static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
    {
        if (occurenceType != RuleDefinitions.TurnOccurenceType.StartOfTurn)
        {
            return;
        }

        var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (battleService?.Battle == null)
        {
            return;
        }

        foreach (var contender in battleService.Battle.AllContenders
                     .Where(x => x is {destroying: false, destroyedBody: false, RulesetActor: { }}))
        {
            var conditionsToRemove = (from keyValuePair in contender.RulesetActor.ConditionsByCategory
                from rulesetCondition in keyValuePair.Value
                where rulesetCondition.SourceGuid == __instance.Guid &&
                      rulesetCondition.ConditionDefinition is IConditionRemovedOnSourceTurnStart
                select rulesetCondition).ToList();

            foreach (var conditionToRemove in conditionsToRemove)
            {
                contender.RulesetActor.RemoveCondition(conditionToRemove);
            }
        }
    }
}
