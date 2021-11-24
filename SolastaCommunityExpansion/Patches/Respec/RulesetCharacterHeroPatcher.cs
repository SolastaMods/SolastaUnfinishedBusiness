using HarmonyLib;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches
{
    // use this patch to enable the after rest actions
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
    internal static class RulesetCharacterHero_EnumerateAfterRestActions
    {
        internal static void Postfix(RuleDefinitions.RestType restType, List<RestActivityDefinition> ___afterRestActions)
        {
            if (Main.Settings.EnableRespec && restType == RuleDefinitions.RestType.LongRest)
            {
                foreach (var restActivityDefinition in DatabaseRepository.GetDatabase<RestActivityDefinition>().GetAllElements())
                {
                    if (restActivityDefinition.Condition == Settings.ActivityConditionCanRespec)
                    {
                        ___afterRestActions.Add(restActivityDefinition);
                    }
                }
            }
        }
    }
}
