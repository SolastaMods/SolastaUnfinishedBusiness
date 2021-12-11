using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion.Patches.SaveByLocation
{
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationManager_LoadLocationAsync
    {
        public static void Prefix(GameLocationManager __instance,
            string locationDefinitionName, string userLocationName, string userCampaignName)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            Main.Log($"LoadLocationAsync-Params: ld={locationDefinitionName}, ul={userLocationName}, uc={userCampaignName}");

            var sessionService = ServiceRepository.GetService<ISessionService>();

            if (sessionService != null && sessionService.Session != null)
            {
                var session = sessionService.Session;
                var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();

                Main.Log($"Campaign-ss: Campaign={session.CampaignDefinitionName}, Location: {session.UserLocationName}");

                selectedCampaignService.Campaign = userCampaignName;
                selectedCampaignService.Location = userLocationName;
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }
}
