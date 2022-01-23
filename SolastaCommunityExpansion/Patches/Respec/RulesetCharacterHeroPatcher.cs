using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to enable the after rest actions
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_EnumerateAfterRestActions
    {
        internal static void Postfix(List<RestActivityDefinition> ___afterRestActions)
        {
            if (Main.Settings.EnableRespec)
            {
                foreach (var restActivityDefinition in DatabaseRepository.GetDatabase<RestActivityDefinition>().GetAllElements())
                {
                    if (restActivityDefinition.Condition == Models.RespecContext.ActivityConditionCanRespec)
                    {
                        ___afterRestActions.Add(restActivityDefinition);
                    }
                }
            }
        }
    }
}
