using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Respec
{
    // use these patches to enable the Level Down button in the Heroes Pool
    internal static class CharactersPanelPatcher
    {
        // enables the level down button if hero above level 1
        [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
        internal static class CharactersPanelRefresh
        {
            internal static void Postfix(CharactersPanel __instance)
            {
                if (Main.Settings.EnableRespec)
                {
                    var characterPlates = __instance.GetField<CharactersPanel, List<CharacterPlateToggle>>("characterPlates");
                    var selectedPlate = __instance.GetField<CharactersPanel, int>("selectedPlate");
                    var exportPdfButton = __instance.GetField<CharactersPanel, Button>("exportPdfButton");
                    var characterLevel = (selectedPlate >= 0) ? characterPlates[selectedPlate].GuiCharacter.CharacterLevel : 1;

                    exportPdfButton.gameObject.SetActive(characterLevel > 1);
                }
            }
        }
    }
}
