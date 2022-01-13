using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiBattle
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameTime_SetTimeScale
    {
        internal static bool Prefix(float ___timeScale, float ___networkTimeScale, bool ___fasterTimeMode)
        {
            var isBattleInProgress = ServiceRepository.GetService<IGameLocationBattleService>()?.IsBattleInProgress;

            if (isBattleInProgress == false || ___networkTimeScale != 1.0)
            {
                return true;
            }

            if (Main.Settings.PermanentlySpeedBattleUp)
            {
                Time.timeScale = ___timeScale * Main.Settings.BattleCustomTimeScale;
            }
            else
            {
                Time.timeScale = ___timeScale * (___fasterTimeMode ? Main.Settings.BattleCustomTimeScale : 1f);
            }

            return false;
        }
    }
}
