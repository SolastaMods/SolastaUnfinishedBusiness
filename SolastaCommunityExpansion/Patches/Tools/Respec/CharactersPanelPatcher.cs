using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.Respec
{
    // enables the level down button on characters pool if hero above level 1

    [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class CharactersPanel_Refresh
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
