using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    internal static class CharacterEditionScreenPatcher
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
                var stagePanelPrefabs =
                    characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
                var customFeatureSelection = CustomFeatureSelectionPanel.Get(stagePanelPrefabs, __instance);

                if (__instance is not CharacterLevelUpScreen)
                {
                    var targetPosition = ___stagePanelsByName.Count - 1;
                    var newDict = new Dictionary<string, CharacterStagePanel>();
                    var i = 0;
                    foreach (var e in ___stagePanelsByName)
                    {
                        if (i == targetPosition)
                        {
                            newDict.Add(customFeatureSelection.Name, customFeatureSelection);
                        }

                        newDict.Add(e.Key, e.Value);
                        i++;
                    }

                    __instance.SetField("stagePanelsByName", newDict);
                }
                else
                {
                    ___stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                }
            }
        }
    }
}
