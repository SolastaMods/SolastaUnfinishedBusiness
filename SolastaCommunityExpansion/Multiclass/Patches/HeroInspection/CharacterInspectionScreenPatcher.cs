using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.HeroInspection
{
    internal static class CharacterInspectionScreenPatcher
    {
        // set the inspection context for MC heroes
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
        internal static class CharacterInspectionScreenBind
        {
            internal static void Postfix(RulesetCharacterHero heroCharacter, CharacterPlateDetailed ___characterPlate, ToggleGroup ___toggleGroup)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                InspectionPanelContext.SelectedHero = heroCharacter;
                CharacterInspectionScreenHandleInput.CharacterTabActive = true;

                // get more real state for the toggles on top
                ___toggleGroup.transform.position = new UnityEngine.Vector3(___characterPlate.transform.position.x / 2f, ___toggleGroup.transform.position.y, 0);
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

                InspectionPanelContext.SelectedHero = null;
            }
        }

        // trap the hotkeys on inspection panel
        [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
        internal static class CharacterInspectionScreenHandleInput
        {
            internal static bool CharacterTabActive { get; set; }

            internal static void Postfix(InputCommands.Id command, ToggleGroup ___toggleGroup, CharacterInformationPanel ___characterInformationPanel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

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
}
