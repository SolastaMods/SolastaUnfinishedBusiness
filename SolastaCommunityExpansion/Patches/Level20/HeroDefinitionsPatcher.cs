using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // overrides the max experience returned
    [HarmonyPatch(typeof(HeroDefinitions), "MaxHeroExperience")]
    internal static class HeroDefinitions_MaxHeroExperience
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
