using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

// set the inspection context for MC heroes
[HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_Bind
{
    internal static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
    {
        Global.InspectedHero = heroCharacter;

        var transform = __instance.toggleGroup.transform;

        // get more real state for the toggles on top (required for MC)
        transform.position = new Vector3(__instance.characterPlate.transform.position.x / 2f, transform.position.y, 0);
    }
}

// reset the inspection context for MC heroes
[HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_Unbind
{
    internal static void Prefix()
    {
        InventoryManagementContext.ResetControls();

        Global.InspectedHero = null;
    }
}

// reset the inspection context for MC heroes
[HarmonyPatch(typeof(CharacterInspectionScreen), "DoClose")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_DoClose
{
    internal static void Prefix()
    {
        Global.InspectedHero = null;
    }
}
