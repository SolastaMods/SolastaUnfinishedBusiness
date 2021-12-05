using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHeroPatcher_Respec
    {
        // use this patch to enable the after rest actions
        [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
}
