using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaMulticlass.Patches.LevelUp
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
                if (__instance is not CharacterLevelUpScreen)
                {
                    return;
                }

                var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
                var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var newLevelUpSequence = new Dictionary<string, CharacterStagePanel>
                {
                    { "ClassSelection", classSelectionPanel }
                };

                foreach (var stagePanel in ___stagePanelsByName)
                {
                    newLevelUpSequence.Add(stagePanel.Key, stagePanel.Value);

                    if (stagePanel.Key == "LevelGains")
                    {
                        newLevelUpSequence.Add("DeitySelection", deitySelectionPanel);
                    }
                }

                ___stagePanelsByName = newLevelUpSequence;
            }
        }
    }
}
