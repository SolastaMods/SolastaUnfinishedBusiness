using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // add the class selection stage panel to the level up screen
    [HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterLevelUpScreen_LoadStagePanels
    {
        internal static void Postfix(
            CharacterEditionScreen __instance,
            ref Dictionary<string, CharacterStagePanel> ___stagePanelsByName)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return;
            }

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
