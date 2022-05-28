using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Battle
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameTime_SetTimeScale
    {
        internal static bool Prefix(GameTime __instance)
        {
            Time.timeScale = __instance.networkTimeScale != 1.0
                ? __instance.networkTimeScale
                : __instance.timeScale * (__instance.fasterTimeMode ? Main.Settings.FasterTimeModifier : 1f);

            return false;
        }
    }
}
