using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    [HarmonyPatch(typeof(CharacterInspectionScreen), "OnBeginHide")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_OnBeginHide
    {
        internal static void Prefix()
        {
            if (Global.IsMultiplayer)
            {
                return;
            }

            InventoryManagementContext.ResetControls();
        }
    }
    
    // set the inspection context for MC heroes
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_Bind
    {
        internal static void Postfix(
            RulesetCharacterHero heroCharacter,
            CharacterPlateDetailed ___characterPlate,
            ToggleGroup ___toggleGroup)
        {
            Global.InspectedHero = heroCharacter;

            // get more real state for the toggles on top
            if (Main.Settings.EnableMulticlass)
            {
                ___toggleGroup.transform.position = 
                    new UnityEngine.Vector3(___characterPlate.transform.position.x / 2f, ___toggleGroup.transform.position.y, 0);
            }
        }
    }

    // reset the inspection context for MC heroes
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_Unbind
    {
        internal static void Postfix()
        {
            Global.InspectedHero = null;
        }
    }
}
