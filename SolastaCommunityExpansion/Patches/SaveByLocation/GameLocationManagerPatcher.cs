using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
                selectedCampaignService.Location = string.IsNullOrEmpty(session.UserCampaignName) ? session.UserLocationName : session.UserCampaignName;
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }
}
