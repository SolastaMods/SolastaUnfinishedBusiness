using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20;

// overrides the max experience returned
[HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class HeroDefinitions_MaxHeroExperience
{
    internal static bool Prefix(ref int __result)
    {
        if (!Main.Settings.EnableLevel20)
        {
            return true;
        }

        __result = MOD_MAX_EXPERIENCE;

        return false;
    }
}
