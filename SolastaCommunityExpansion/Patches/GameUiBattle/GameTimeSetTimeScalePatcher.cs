using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiBattle
{
    [HarmonyPatch(typeof(GameTime), "SetTimeScale")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameTime_SetTimeScale
    {
        internal static void Postfix(ref float ___timeScale, float ___networkTimeScale, bool ___fasterTimeMode)
        {
            var isBattleInProgress = ServiceRepository.GetService<IGameLocationBattleService>()?.IsBattleInProgress;

            if (isBattleInProgress == false)
            {
                return;
                
            }

            ___timeScale = ___networkTimeScale != 1.0 
                ? ___networkTimeScale 
                : ___timeScale * (___fasterTimeMode || Main.Settings.PermanentlySpeedBattleUp ? Main.Settings.BattleCustomTimeScale : 1f);
        }
    }
}
