using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    // add the custom features selection stage panel to the creation and level up screen
    [HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterEditionScreen_LoadStagePanels
    {
        internal static CustomFeatureSelectionPanel GetPanel(CharacterEditionScreen __instance)
        {
            var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
            var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
            var container = __instance.StagesPanelContainer;
            var gameObject = Gui.GetPrefabFromPool(stagePanelPrefabs[8], container);
            var characterStageSpellSelectionPanel = gameObject.GetComponent<CharacterStageSpellSelectionPanel>();
            var customFeatureSelection = gameObject.AddComponent<CustomFeatureSelectionPanel>();

            customFeatureSelection.Setup(characterStageSpellSelectionPanel);

            return customFeatureSelection;
        }

        internal static void Postfix(
            CharacterEditionScreen __instance,
            ref Dictionary<string, CharacterStagePanel> ___stagePanelsByName)
        {
            if (__instance is CharacterCreationScreen)
            {
                var customFeatureSelection = GetPanel(__instance);
                var last = ___stagePanelsByName.ElementAt(___stagePanelsByName.Count - 1);

                ___stagePanelsByName.Remove(last.Key);
                ___stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                ___stagePanelsByName.Add(last.Key, last.Value);
            }
            else if (__instance is CharacterLevelUpScreen)
            {
                var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
                var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var customFeatureSelectionPanel = GetPanel(__instance);
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

                ___stagePanelsByName.Add(customFeatureSelectionPanel.Name, customFeatureSelectionPanel);
                ___stagePanelsByName = newLevelUpSequence;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterEditionScreen_DoAbort
    {
        internal static void Prefix(RulesetCharacterHero ___currentHero)
        {
            LevelUpContext.UnregisterHero(___currentHero);
        }
    }
}
