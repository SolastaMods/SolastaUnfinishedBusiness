using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterInspectionScreenPatcher
{
    //PATCH: set the inspection context for MC heroes and get more real state for the toggles on top (Multiclass)
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            Global.InspectedHero = heroCharacter;

            var transform = __instance.toggleGroup.transform;

            transform.position =
                new Vector3(__instance.characterPlate.transform.position.x / 2f, transform.position.y, 0);
        }
    }

    [HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Prefix()
        {
            //PATCH: resets the inspection context for MC heroes
            Global.InspectedHero = null;

            //PATCH: Enable Inventory Filtering and Sorting
            if (Main.Settings.EnableInventoryFilteringAndSorting)
            {
                InventoryManagementContext.ResetControls();
            }
        }
    }

    [HarmonyPatch(typeof(CharacterInspectionScreen), "DoClose")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class DoClose_Patch
    {
        internal static void Prefix()
        {
            //PATCH: reset the inspection context for MC heroes
            Global.InspectedHero = null;
        }
    }

    //PATCH: traps the input hotkey and start the char export process
    [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleInput_Patch
    {
        public static void Postfix(CharacterInspectionScreen __instance, InputCommands.Id command)
        {
            if (Main.Settings.EnableCharacterExport && command == Hotkeys.CtrlShiftE &&
                !CharacterExportContext.InputModalVisible)
            {
                CharacterExportContext.ExportInspectedCharacter(__instance.InspectedCharacter.RulesetCharacterHero);
            }
        }
    }
}
