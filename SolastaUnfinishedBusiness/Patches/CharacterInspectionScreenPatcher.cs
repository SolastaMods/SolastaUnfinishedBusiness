using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterInspectionScreenPatcher
{
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            //PATCH: sets the inspection context for MC heroes
            Global.InspectedHero = heroCharacter;

            //PATCH: gets more real state for the toggles on top (MULTICLASS)
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
}
