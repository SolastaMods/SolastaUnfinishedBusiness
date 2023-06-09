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

            // this is causing issues loading games so had to disable until finding out why

            // if (Main.Settings.OverrideMinMaxLevel)
            // {
            //     __instance.CampaignForImportSaveMode.maxLevelImport = Level20Context.ModMaxLevel;
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
