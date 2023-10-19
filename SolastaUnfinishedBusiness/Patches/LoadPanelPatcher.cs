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
        public static void Prefix([NotNull] LoadPanel __instance)
        {
            //PATCH: EnableSaveByLocation
            if (Main.Settings.EnableSaveByLocation && !__instance.ImportSaveMode)
            {
                LoadPanelOnBeginShowSaveByLocationBehavior(__instance);
            }
            else
            {
                Dropdown?.SetActive(false);
            }

            //PATCH: Allow import any campaign if override min max level is on

            // this is causing issues loading games so had to disable until finding out why

            // if (Main.Settings.OverrideMinMaxLevel)
            // {
            //     __instance.CampaignForImportSaveMode.maxLevelImport = Level20Context.ModMaxLevel;
            // }
        }
    }

    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.OnEndHide))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginHide_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            Dropdown?.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.HandleInputControlSchemeChangedForShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleInputControlSchemeChangedForShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix()
        {
            Dropdown?.UpdateControls();
        }
    }
}
