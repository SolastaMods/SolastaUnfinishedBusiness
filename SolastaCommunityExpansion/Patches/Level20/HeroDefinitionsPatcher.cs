using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    internal static class HeroDefinitionsPatcher
    {
        // overrides the max experience returned
        [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class HeroDefinitions_MaxHeroExperience_Patch
        {
            internal static bool Prefix(ref int __result)
            {
                if (Main.Settings.EnableLevel20)
                {
                    __result = MAX_CHARACTER_EXPERIENCE;

                    return false;
                }

                return true;
            }
        }
    }
}
