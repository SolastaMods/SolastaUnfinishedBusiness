using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.SaveByLocationContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TacticalAdventuresApplicationPatcher
{
    private static bool EnableSaveByLocation(ref string __result)
    {
        //PATCH: EnableSaveByLocation
        if (!Main.Settings.EnableSaveByLocation)
        {
            return true;
        }

        // Modify the value returned by TacticalAdventuresApplication.SaveGameDirectory so that saves
        // end up where we want them (by location/campaign)
        var selectedCampaignService = ServiceRepository.GetService<SelectedCampaignService>();

        // handle exception when saving from world map or encounters on a user campaign
        if (Gui.GameCampaign?.campaignDefinition?.IsUserCampaign == true &&
            selectedCampaignService is { LocationType: LocationType.StandardCampaign })
        {
            (__result, _) = GetMostRecent();

            return false;
        }

        __result = selectedCampaignService?.SaveGameDirectory ?? DefaultSaveGameDirectory;

        return false;
    }

    [HarmonyPatch(typeof(TacticalAdventuresApplication), nameof(TacticalAdventuresApplication.SaveGameDirectory),
        MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SaveGameDirectory_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref string __result)
        {
            return EnableSaveByLocation(ref __result);
        }
    }

    [HarmonyPatch(typeof(TacticalAdventuresApplication),
        nameof(TacticalAdventuresApplication.MultiplayerFilesDirectory),
        MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MultiplayerFilesDirectory_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref string __result)
        {
            return EnableSaveByLocation(ref __result);
        }
    }
}
