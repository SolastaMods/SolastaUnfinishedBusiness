using HarmonyLib;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches
{
    internal static class RulesetCharacterHeroPatcher_Respec
    {
        // use this patch to enable the after rest actions
        [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
        internal static class RulesetCharacterHero_EnumerateAfterRestActions
        {
            internal static void Postfix(List<RestActivityDefinition> ___afterRestActions)
            {
                if (Main.Settings.EnableRespec)
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
}