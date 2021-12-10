using HarmonyLib;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    internal static class GameLocationManager_LoadLocationAsync
    {
        public static void Prefix(GameLocationManager __instance, string userLocationName)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            var sessionService = ServiceRepository.GetService<ISessionService>();

            if (sessionService != null && sessionService.Session != null)
            {
                var session = sessionService.Session;
                var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

                selectedCampaignService.Campaign = session.CampaignDefinitionName;
                selectedCampaignService.Location = session.UserLocationName;
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }
}
