using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion
{
    [HarmonyPatch(typeof(TacticalAdventuresApplication), "SaveGameDirectory", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TacticalAdventuresApplication_SaveGameDirectory
    {
        public static bool Prefix(ref string __result)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return true;
            }

            // Modify the value returned by TacticalAdventuresApplication.SaveGameDirectory so that saves
            // end up where we want them (by location/campaign).

            var selectedCampaignService = ServiceRepository.GetService<SelectedCampaignService>();

            __result = selectedCampaignService?.SaveGameDirectory ?? DefaultSaveGameDirectory;

            Main.Log($"SaveFolder changed from {DefaultSaveGameDirectory} to {__result}");

            return false;
        }
    }
}
