using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches;

internal static class HeroDefinitionsPatcher
{
    //PATCH: overrides the max experience allowed under Level 20 scenarios
    [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class MaxHeroExperience_Patch
    {
        internal static bool Prefix(ref int __result)
        {
            if (!Main.Settings.EnableLevel20)
            {
                return true;
            }

            __result = ModMaxExperience;

            return false;
        }
    }
}
