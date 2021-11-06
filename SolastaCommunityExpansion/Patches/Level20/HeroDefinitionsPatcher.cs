using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    class HeroDefinitionsPatcher
    {
        // overrides the max experience returned
        [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
        internal static class HeroDefinitions_MaxHeroExperience_Patch
        {
            internal static bool Prefix(ref int __result)
            {
                __result = MAX_CHARACTER_EXPERIENCE;

                return false;
            }
        }
    }
}