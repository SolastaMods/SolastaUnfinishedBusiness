using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion.Patches.Tools.SaveByLocation;

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

        Main.Log(
            $"LoadLocationAsync-Params: ld={locationDefinitionName}, ul={userLocationName}, uc={userCampaignName}");

        var sessionService = ServiceRepository.GetService<ISessionService>();

        if (sessionService != null && sessionService.Session != null)
        {
            // Record which campaign/location the latest load game belongs to

#if DEBUG
            var session = sessionService.Session;

            Main.Log(
                $"Campaign-ss: Campaign={session.CampaignDefinitionName}, Location: {session.UserLocationName}");
#endif
            var selectedCampaignService = ServiceRepositoryEx.GetOrCreateService<SelectedCampaignService>();


            selectedCampaignService.SetCampaignLocation(userCampaignName, userLocationName);
        }

        __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
    }
}
