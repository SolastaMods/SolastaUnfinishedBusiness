using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationManagerPatcher
{
    //PATCH: EnableSaveByLocation
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class LoadLocationAsync_Patch
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

            if (sessionService is { Session: { } })
            {
                // Record which campaign/location the latest load game belongs to

#if DEBUG
                var session = sessionService.Session;

                Main.Log(
                    $"Campaign-ss: Campaign={session.CampaignDefinitionName}, Location: {session.UserLocationName}");
#endif
                var selectedCampaignService = SaveByLocationContext.ServiceRepositoryEx
                    .GetOrCreateService<SaveByLocationContext.SelectedCampaignService>();


                selectedCampaignService.SetCampaignLocation(userCampaignName, userLocationName);
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }

    //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
    [HarmonyPatch(typeof(GameLocationManager), "ReadyLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReadyLocation_Patch
    {
        internal static void Postfix(GameLocationManager __instance)
        {
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered || Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var worldGadgets = __instance.WorldLocation.WorldSectors.SelectMany(x => x.WorldGadgets);

            foreach (var worldGadget in worldGadgets)
            {
                GameUiContext.SetTeleporterGadgetActiveAnimation(worldGadget);
            }
        }
    }
}
