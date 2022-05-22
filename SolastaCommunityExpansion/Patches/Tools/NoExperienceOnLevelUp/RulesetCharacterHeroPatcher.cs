using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Tools.NoExperienceOnLevelUp
{
    // use this patch to enable the No Experience on Level up cheat
    [HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_CanLevelUp
    {
        internal static bool Prefix(RulesetCharacterHero __instance, ref bool __result)
        {
            if (Main.Settings.NoExperienceOnLevelUp)
            {
                __result = __instance.ClassesHistory.Count < Main.Settings.MaxAllowedLevels;

                return false;
            }

            // If the game doesn't know how much XP to reach the next level it uses -1 to determine if the character can level up
            // When a character is level 20, this ends up meaning the character can now level up forever unless we stop it here
            if (__instance.ClassesHistory.Count >= Main.Settings.MaxAllowedLevels)
            {
                __result = false;

                return false;
            }

            return true;
        }
    }
}
