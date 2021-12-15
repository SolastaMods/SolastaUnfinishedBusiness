using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Encounters
{
    [HarmonyPatch(typeof(GameLocationBattle), "GetMyContenders")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattle_GetMyContenders
    {
        internal static void Postfix(GameLocationBattle __instance, ref List<GameLocationCharacter> __result)
        {
            if (Main.Settings.EnableEnemiesControlledByPlayer && !Models.PlayerControllerContext.PlayerCharacters.Contains(__instance.ActiveContender))
            {
                __result = __instance.EnemyContenders;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattle), "GetOpposingContenders")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattle_GetOpposingContenders
    {
        internal static void Postfix(GameLocationBattle __instance, ref List<GameLocationCharacter> __result)
        {
            if (Main.Settings.EnableEnemiesControlledByPlayer && !Models.PlayerControllerContext.PlayerCharacters.Contains(__instance.ActiveContender))
            {
                __result = __instance.PlayerContenders;
            }
        }
    }
}
