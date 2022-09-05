using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

//PATCH: FasterTimeModifier
[HarmonyPatch(typeof(GameTime), "SetTimeScale")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameTime_SetTimeScale
{
    internal static bool Prefix(GameTime __instance)
    {
        Time.timeScale = Math.Abs(__instance.networkTimeScale - 1.0) > 0.001f
            ? __instance.networkTimeScale
            : __instance.timeScale * (__instance.fasterTimeMode ? Main.Settings.FasterTimeModifier : 1f);

        return false;
    }
}
