using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaCommunityExpansion.CustomUI;
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
            internal static void Postfix(CharacterEditionScreen __instance, Dictionary<string, CharacterStagePanel> ___stagePanelsByName)
            {
                var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
                var customFeatureSelection = CustomFeatureSelectionPanel.Get(stagePanelPrefabs, __instance.StagesPanelContainer);
                if (__instance is not CharacterLevelUpScreen characterLevelUpScreen)
                {
                    var newDict = new Dictionary<string, CharacterStagePanel>();
                    var i = 0;
                    var targetPosition = ___stagePanelsByName.Count - 1;
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
                    return;
                }

                // var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                // var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
                var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();

                characterLevelUpScreen.SetField("stagePanelsByName", new Dictionary<string, CharacterStagePanel>
                {
                    { "ClassSelection", classSelectionPanel },
                    { "LevelGains", ___stagePanelsByName["LevelGains"] },
                    { "DeitySelection", deitySelectionPanel },
                    { "SubclassSelection", ___stagePanelsByName["SubclassSelection"] },
                    { "AbilityScores", ___stagePanelsByName["AbilityScores"] },
                    { "FightingStyleSelection", ___stagePanelsByName["FightingStyleSelection"] },
                    { "ProficiencySelection", ___stagePanelsByName["ProficiencySelection"] },
                    { "", ___stagePanelsByName[""] },
                    {customFeatureSelection.Name, customFeatureSelection}
                });
            }
        }
    }
}
