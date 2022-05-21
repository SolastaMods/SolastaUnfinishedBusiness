using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Battle
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameTime_SetTimeScale
    {
        internal static bool Prefix(float ___networkTimeScale, float ___timeScale, bool ___fasterTimeMode)
        {
            Time.timeScale = ___networkTimeScale != 1.0
                ? ___networkTimeScale
                : ___timeScale * (___fasterTimeMode ? Main.Settings.FasterTimeModifier : 1f);

            return false;
        }
    }
}
