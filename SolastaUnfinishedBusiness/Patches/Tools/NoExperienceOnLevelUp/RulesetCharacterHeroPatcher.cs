using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.Tools.NoExperienceOnLevelUp;

//PATCH: enables the No Experience on Level up cheat (NoExperienceOnLevelUp)
[HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_CanLevelUp
{
    internal static bool Prefix(RulesetCharacterHero __instance, ref bool __result)
    {
        if (Main.Settings.NoExperienceOnLevelUp)
        {
            var levelCap = Main.Settings.EnableLevel20
                ? Level20Context.ModMaxLevel
                : Level20Context.GameMaxLevel;

            __result = __instance.ClassesHistory.Count < levelCap;

            return false;
        }

        if (!Main.Settings.EnableLevel20)
        {
            return true;
        }

        {
            var levelCap = Main.Settings.EnableLevel20
                ? Level20Context.ModMaxLevel
                : Level20Context.GameMaxLevel;
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
