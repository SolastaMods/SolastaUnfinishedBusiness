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
            private static bool HasInit { get; set; }
            private static int SelectedPlate { get; set; }

            internal static void Postfix(List<CharacterPlateToggle> ___characterPlates, int ___selectedPlate, Button ___exportPdfButton)
            {
                if (!Main.Settings.EnableRespec || !Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var characterLevel = (___selectedPlate >= 0) ? ___characterPlates[___selectedPlate].GuiCharacter.CharacterLevel : 1;

                SelectedPlate = ___selectedPlate;
                ___exportPdfButton.gameObject.SetActive(characterLevel > 1);

                if (HasInit)
                {
                    return;
                }

                ___exportPdfButton.onClick.RemoveAllListeners();
                ___exportPdfButton.onClick.AddListener(() =>
                {
                    LevelDownContext.ConfirmAndExecute(___characterPlates[SelectedPlate].Filename);
                });

                HasInit = true;
            }
        }
    }
}
