using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

//PATCH: set the inspection context for MC heroes and get more real state for the toggles on top (Multiclass)
[HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_Bind
{
    internal static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
    {
        Global.InspectedHero = heroCharacter;

        var transform = __instance.toggleGroup.transform;

        transform.position = new Vector3(__instance.characterPlate.transform.position.x / 2f, transform.position.y, 0);
    }
}

//PATCH: resets the inspection context for MC heroes
//PATCH: Enable Inventory Filtering and Sorting
[HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_Unbind
{
    internal static void Prefix()
    {
        Global.InspectedHero = null;

        if (Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer)
        {
            InventoryManagementContext.ResetControls();
        }
    }
}

//PATCH: reset the inspection context for MC heroes
[HarmonyPatch(typeof(CharacterInspectionScreen), "DoClose")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_DoClose
{
    internal static void Prefix()
    {
        Global.InspectedHero = null;
    }
}
