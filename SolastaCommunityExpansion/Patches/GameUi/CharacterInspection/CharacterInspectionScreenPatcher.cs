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

            Models.InventoryManagementContext.ResetControls();
        }
    }
    
    // set the inspection context for MC heroes
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
    internal static class CharacterInspectionScreenBind
    {
        internal static void Postfix(RulesetCharacterHero heroCharacter)
        {
            Global.InspectedHero = heroCharacter;
        }
    }

    // reset the inspection context for MC heroes
    [HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
    internal static class CharacterInspectionScreenUnbind
    {
        internal static void Postfix()
        {
            Global.InspectedHero = null;
        }
    }
}
