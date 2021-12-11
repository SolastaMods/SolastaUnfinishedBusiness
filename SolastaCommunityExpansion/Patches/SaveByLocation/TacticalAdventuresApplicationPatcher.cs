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

            var saves = Path.Combine(TacticalAdventuresApplication.GameDirectory, "Saves");
            __result = Path.Combine(saves, selectedCampaignService?.GetFolderName() ?? string.Empty);

            Main.Log($"GetSaveFolder={saves}, {__result}");

            return false;
        }
    }
}
