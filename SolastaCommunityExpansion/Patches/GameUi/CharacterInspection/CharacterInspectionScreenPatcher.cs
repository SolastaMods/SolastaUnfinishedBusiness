using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

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
        internal static void Postfix(RulesetCharacterHero heroCharacter)
        {
            Global.InspectedHero = heroCharacter;
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
