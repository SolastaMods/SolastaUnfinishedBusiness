using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    internal static class GameTimeSetTimeScalePatcher
    {
        [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GameTime_SetTimeScale
        {
            internal static bool Prefix(ref float ___timeScale, ref bool ___fasterTimeMode)
            {
                if (Main.Settings.PermanentSpeedUp)
                {
                    Time.timeScale = ___timeScale * Main.Settings.CustomTimeScale;
                }
                else
                {
                    Time.timeScale = ___timeScale * (___fasterTimeMode ? Main.Settings.CustomTimeScale : 1f);
                }

                return false;
            }
        }
    }
}
