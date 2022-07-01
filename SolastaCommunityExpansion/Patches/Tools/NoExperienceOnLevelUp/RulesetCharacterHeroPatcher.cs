using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.NoExperienceOnLevelUp;

// use this patch to enable the No Experience on Level up cheat
[HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_CanLevelUp
{
    internal static bool Prefix(RulesetCharacterHero __instance, ref bool __result)
    {
        if (Main.Settings.NoExperienceOnLevelUp)
        {
            var levelCap = Main.Settings.EnableLevel20
                ? Level20Context.MOD_MAX_LEVEL
                : Level20Context.GAME_MAX_LEVEL;

            __result = __instance.ClassesHistory.Count < levelCap;

            return false;
        }

        if (!Main.Settings.EnableLevel20)
        {
            return true;
        }

        {
            var levelCap = Main.Settings.EnableLevel20
                ? Level20Context.MOD_MAX_LEVEL
                : Level20Context.GAME_MAX_LEVEL;
            // If the game doesn't know how much XP to reach the next level it uses -1 to determine if the character can level up.
            // When a character is level 20, this ends up meaning the character can now level up forever unless we stop it here.
            if (__instance.ClassesHistory.Count < levelCap)
            {
                return true;
            }

            __result = false;

            return false;
        }
    }
}
