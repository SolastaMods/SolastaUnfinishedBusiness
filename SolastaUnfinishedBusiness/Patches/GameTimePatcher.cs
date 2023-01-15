using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameTimePatcher
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetTimeScale_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameTime __instance)
        {
            //PATCH: FasterTimeModifier
            Time.timeScale = Math.Abs(__instance.networkTimeScale - 1.0) > 0.001f
                ? __instance.networkTimeScale
                : __instance.timeScale * (__instance.fasterTimeMode ? Main.Settings.FasterTimeModifier : 1f);

            return false;
        }
    }
}
