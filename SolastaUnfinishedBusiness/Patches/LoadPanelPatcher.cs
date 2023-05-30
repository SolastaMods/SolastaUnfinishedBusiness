using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Models.SaveByLocationContext;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class LoadPanelPatcher
{
    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] LoadPanel __instance)
        {
            //PATCH: EnableSaveByLocation
            if (Main.Settings.EnableSaveByLocation && !__instance.ImportSaveMode)
            {
                return LoadPanelOnBeginShowSaveByLocationBehavior(__instance);
            }

            //PATCH: Allow import any campaign if override min max level is on
            // if (Main.Settings.OverrideMinMaxLevel)
            // {
            //     __instance.CampaignForImportSaveMode.minStartLevel = 1;
            //     __instance.CampaignForImportSaveMode.maxStartLevel = 20;
            //     __instance.CampaignForImportSaveMode.maxLevelImport = 20;
            // }

#pragma warning disable IDE0031
            if (Dropdown != null && Dropdown.activeSelf)
#pragma warning restore IDE0031
            {
                Dropdown.SetActive(false);
            }

            return true;
        }
    }
}
