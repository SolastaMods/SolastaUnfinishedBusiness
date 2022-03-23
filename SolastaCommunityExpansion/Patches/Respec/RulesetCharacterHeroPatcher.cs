using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use this patch to enable the after rest actions
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_EnumerateAfterRestActions
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            if (Main.Settings.EnableRespec)
            {
                __instance.AfterRestActions.Add(Models.RespecContext.RestActivityRespecBuilder.RestActivityRespec);
            }
        }
    }
}
