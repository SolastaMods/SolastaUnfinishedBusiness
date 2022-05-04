using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaMulticlass.Models;
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

            CharacterInspectionScreen_HandleInput.CharacterTabActive = true;

            if (Main.Settings.EnableMulticlass)
            {
                // get more real state for the toggles on top
                ___toggleGroup.transform.position = new UnityEngine.Vector3(___characterPlate.transform.position.x / 2f, ___toggleGroup.transform.position.y, 0);
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

    // trap the hotkeys on inspection panel
    [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        internal static bool CharacterTabActive { get; set; }

        internal static void Postfix(InputCommands.Id command, ToggleGroup ___toggleGroup, CharacterInformationPanel ___characterInformationPanel)
        {
            switch (command)
            {
                case InspectionPanelContext.PLAIN_UP:
                    if (!___characterInformationPanel.gameObject.activeSelf)
                    {
                        CharacterTabActive = !CharacterTabActive;
                        ___toggleGroup.transform.GetChild(1).gameObject.SetActive(CharacterTabActive);
                    }

                    break;

                case InspectionPanelContext.PLAIN_DOWN:
                    InspectionPanelContext.PickNextHeroClass();

                    if (___characterInformationPanel.gameObject.activeSelf)
                    {
                        ___characterInformationPanel.RefreshNow();
                    }

                    break;
            }
        }
    }
}
