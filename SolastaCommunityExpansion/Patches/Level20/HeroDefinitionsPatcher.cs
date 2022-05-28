using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // overrides the max experience returned
    [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HeroDefinitions_MaxHeroExperience
    {
        internal static bool Prefix(ref int __result)
        {
            __result = Main.Settings.EnableLevel20 ? Level20Context.MOD_MAX_EXPERIENCE : Level20Context.GAME_MAX_EXPERIENCE;

            return false;
        }
    }
}
