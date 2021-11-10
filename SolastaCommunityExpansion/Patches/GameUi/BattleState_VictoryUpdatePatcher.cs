using HarmonyLib;
using static SolastaModApi.DatabaseHelper.QuestTreeDefinitions;

namespace SolastaCommunityExpansion.Patches.GameUi
{
    [HarmonyPatch(typeof(BattleState_Victory), "Update")]
    internal static class BattleState_VictoryUpdatePatcher
    {
        public static void Postfix()
        {
            if (!Main.Settings.AutoPauseOnVictory) return;

            var gameQuestService = ServiceRepository.GetService<IGameQuestService>();

            if (gameQuestService?.ActiveQuests?.Exists(x => x.QuestTreeDefinition == Tutorial_SUB_Scenario_Tutorial_02_Combat) == true) return;

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null) return;
            if (battleService.Battle != null) return;

            var campaign = ServiceRepository.GetService<IGameService>()?.Game?.GameCampaign;

            if (campaign == null) return;

            campaign.GameTime.Pause();
        }
    }
}
