using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

// add the custom features selection stage panel to the creation and level up screen
[HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterEditionScreen_LoadStagePanels
{
    private static CustomFeatureSelectionPanel GetPanel(CharacterEditionScreen __instance)
    {
        var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
        var stagePanelPrefabs =
            characterCreationScreen.stagePanelPrefabs;
        var container = __instance.StagesPanelContainer;
        var gameObject = Gui.GetPrefabFromPool(stagePanelPrefabs[8], container);
        var characterStageSpellSelectionPanel = gameObject.GetComponent<CharacterStageSpellSelectionPanel>();
        var customFeatureSelection = gameObject.AddComponent<CustomFeatureSelectionPanel>();

        customFeatureSelection.Setup(characterStageSpellSelectionPanel);

        return customFeatureSelection;
    }

    internal static void Postfix(CharacterEditionScreen __instance)
    {
        switch (__instance)
        {
            case CharacterCreationScreen:
            {
                var customFeatureSelection = GetPanel(__instance);
                var last = __instance.stagePanelsByName.ElementAt(__instance.stagePanelsByName.Count - 1);

                __instance.stagePanelsByName.Remove(last.Key);
                __instance.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                __instance.stagePanelsByName.Add(last.Key, last.Value);
                break;
            }
            case CharacterLevelUpScreen:
            {
                var customFeatureSelection = GetPanel(__instance);

                __instance.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                break;
            }
        }

        //
        // MULTICLASS
        //

        if (Main.Settings.MaxAllowedClasses == 1 || __instance is not CharacterLevelUpScreen)
        {
            return;
        }

        var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
        var stagePanelPrefabs =
            characterCreationScreen.stagePanelPrefabs;
        var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer)
            .GetComponent<CharacterStagePanel>();
        var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer)
            .GetComponent<CharacterStagePanel>();
        var newLevelUpSequence =
            new Dictionary<string, CharacterStagePanel> {{"ClassSelection", classSelectionPanel}};

        foreach (var stagePanel in __instance.stagePanelsByName)
        {
            newLevelUpSequence.Add(stagePanel.Key, stagePanel.Value);

            if (stagePanel.Key == "LevelGains")
            {
                newLevelUpSequence.Add("DeitySelection", deitySelectionPanel);
            }
        }

        __instance.stagePanelsByName = newLevelUpSequence;
    }
}

[HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterEditionScreen_DoAbort
{
    internal static void Prefix(CharacterEditionScreen __instance)
    {
        LevelUpContext.UnregisterHero(__instance.currentHero);
    }
}
