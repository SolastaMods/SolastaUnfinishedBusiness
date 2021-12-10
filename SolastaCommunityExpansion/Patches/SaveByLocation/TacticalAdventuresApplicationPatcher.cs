using HarmonyLib;
using System.IO;
using static SolastaCommunityExpansion.Models.SaveByLocationContext;

namespace SolastaCommunityExpansion
{
    [HarmonyPatch(typeof(TacticalAdventuresApplication), "SaveGameDirectory", MethodType.Getter)]
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
