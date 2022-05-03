using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    // add the class selection stage panel to the level up screen
    [HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
    internal static class CharacterLevelUpScreenLoadStagePanels
    {
        internal static void Postfix(
            CharacterEditionScreen __instance,
            ref Dictionary<string, CharacterStagePanel> ___stagePanelsByName)
        {
            var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
            var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
            var customFeatureSelection = CustomFeatureSelectionPanel.Get(stagePanelPrefabs, __instance);

            if (__instance is CharacterCreationScreen)
            {
                var last = ___stagePanelsByName.ElementAt(___stagePanelsByName.Count - 1);

                ___stagePanelsByName.Remove(last.Key);
                ___stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                ___stagePanelsByName.Add(last.Key, last.Value);
            }
            else if (__instance is CharacterLevelUpScreen)
            {
                ___stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
            }
        }
    }
}
