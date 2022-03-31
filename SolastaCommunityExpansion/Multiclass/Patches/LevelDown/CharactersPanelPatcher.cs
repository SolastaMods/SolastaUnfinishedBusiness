using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.LevelDown
{
    // enables the level down button on characters pool if hero above level 1
    internal static class CharactersPanelPatcher
    {
        [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
        internal static class CharactersPanelRefresh
        {
            private static bool FirstRun { get; set; } = true;

            internal static void Postfix(List<CharacterPlateToggle> ___characterPlates, int ___selectedPlate, Button ___exportPdfButton)
            {
                if (!Main.Settings.EnableLevelDown)
                {
                    return;
                }

                var characterLevel = (___selectedPlate >= 0) ? ___characterPlates[___selectedPlate].GuiCharacter.CharacterLevel : 1;

                ___exportPdfButton.gameObject.SetActive(characterLevel > 1);


                if (FirstRun)
                {
                    ___exportPdfButton.onClick.RemoveAllListeners();
                    ___exportPdfButton.onClick.AddListener(() =>
                    {
                        LevelDownContext.ConfirmAndExecute(___characterPlates[___selectedPlate].Filename);
                    });
                }
            }
        }
    }
}
