using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaUnfinishedBusiness.Models.SaveByLocationContext;

namespace SolastaUnfinishedBusiness.Patches;

public static class TacticalAdventuresApplicationPatcher
{
    [HarmonyPatch(typeof(TacticalAdventuresApplication), "SaveGameDirectory", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveGameDirectory_Getter_Patch
    {
        public static bool Prefix(ref string __result)
        {
            //PATCH: EnableSaveByLocation
            if (!Main.Settings.EnableSaveByLocation)
            {
                return true;
            }

            // Modify the value returned by TacticalAdventuresApplication.SaveGameDirectory so that saves
            // end up where we want them (by location/campaign)

            var selectedCampaignService = ServiceRepository.GetService<SelectedCampaignService>();

            __result = selectedCampaignService?.SaveGameDirectory ?? DefaultSaveGameDirectory;

            return false;
        }
    }
}
