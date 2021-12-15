using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiBattle
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameTime_SetTimeScale
    {
        internal static bool Prefix(ref float ___timeScale, ref bool ___fasterTimeMode)
        {
            var isBattleInProgress = ServiceRepository.GetService<IGameLocationBattleService>()?.IsBattleInProgress;

            if (Main.Settings.PermanentSpeedUp && isBattleInProgress == true)
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
