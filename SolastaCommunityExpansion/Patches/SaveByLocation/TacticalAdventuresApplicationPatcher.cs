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

            var selectedCampaignService = ServiceRepository.GetService<SelectedCampaignService>();

            if (selectedCampaignService == null || string.IsNullOrEmpty(selectedCampaignService.Location) || selectedCampaignService.Campaign == MAIN_CAMPAIGN)
            {
                __result = Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");
            }
            else
            {
                __result = Path.Combine(Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves"), selectedCampaignService.Location);
            }

            return false;
        }
    }
}
