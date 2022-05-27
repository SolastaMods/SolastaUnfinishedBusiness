using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    // set the inspection context for MC heroes
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_Bind
    {
        internal static void Prefix(
            RulesetCharacterHero heroCharacter,
            CharacterPlateDetailed ___characterPlate,
            ToggleGroup ___toggleGroup)
        {
            Global.InspectedHero = heroCharacter;

            // get more real state for the toggles on top (required for MC)
            ___toggleGroup.transform.position =
                new Vector3(___characterPlate.transform.position.x / 2f,
                    ___toggleGroup.transform.position.y, 0);
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
}
