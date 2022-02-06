using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class CharacterInspectionScreenPatcher
    {
        // set the inspection context for MC heroes
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
        internal static class CharacterInspectionScreenBind
        {
            internal static void Postfix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.InspectionPanelContext.SelectedHero = heroCharacter;
                CharacterInspectionScreenHandleInput.CharacterTabActive = true;

                var characterPlate = __instance.GetField<CharacterInspectionScreen, CharacterPlateDetailed>("characterPlate");
                var toggleGroup = __instance.GetField<CharacterInspectionScreen, ToggleGroup>("toggleGroup");

                characterPlate.ClassLabel.TMP_Text.fontSize = Models.GameUiContext.GetFontSize(heroCharacter.ClassesAndLevels.Count);
                toggleGroup.transform.position = new UnityEngine.Vector3(characterPlate.transform.position.x / 2f, toggleGroup.transform.position.y, 0);
            }
        }

        // reset the inspection context for MC heroes
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
        internal static class CharacterInspectionScreenUnbind
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.InspectionPanelContext.SelectedHero = null;
            }
        }

        // trap the hotkeys on inspection panel
        [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
        internal static class CharacterInspectionScreenHandleInput
        {
            internal static bool CharacterTabActive { get; set; }

            internal static void Postfix(CharacterInspectionScreen __instance, InputCommands.Id command)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var characterInformationPanel = __instance.GetField<CharacterInspectionScreen, CharacterInformationPanel>("characterInformationPanel");
                var toggleGroup = __instance.GetField<CharacterInspectionScreen, ToggleGroup>("toggleGroup");

                switch (command)
                {
                    case Models.InspectionPanelContext.PLAIN_UP:
                        if (!characterInformationPanel.gameObject.activeSelf)
                        {
                            CharacterTabActive = !CharacterTabActive;
                            toggleGroup.transform.GetChild(1).gameObject.SetActive(CharacterTabActive);
                        }

                        break;

                    case Models.InspectionPanelContext.PLAIN_DOWN:
                        Models.InspectionPanelContext.PickNextHeroClass();
                        
                        if (characterInformationPanel.gameObject.activeSelf)
                        {
                            characterInformationPanel.RefreshNow();
                        }

                        break;
                }
            }
        }
    }
}
