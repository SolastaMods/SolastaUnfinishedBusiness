using HarmonyLib;

namespace SolastaContentExpansion.Patches.GameUi
{
    [HarmonyPatch(typeof(BattleState_Victory), "Update")]
    internal static class BattleState_VictoryUpdatePatcher
    {
        public static void Postfix()
        {
            if (!Main.Settings.AutoPauseOnVictory) return;

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null) return;
            if (battleService.Battle != null) return;

            var campaign = ServiceRepository.GetService<IGameService>()?.Game?.GameCampaign;

            if (campaign == null) return;

            campaign.GameTime.Pause();
        }
    }
}
