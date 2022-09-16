using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.LevelUp;

//PATCH: adds the Multiclass class selection panel to the level up screen (MULTICLASS)
[HarmonyPatch(typeof(CharacterEditionScreen), "LoadStagePanels")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterEditionScreen_LoadStagePanels
{
    // [NotNull]
    // private static CustomFeatureSelectionPanel GetPanel([NotNull] CharacterEditionScreen __instance)
    // {
    //     var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
    //     var stagePanelPrefabs =
    //         characterCreationScreen.stagePanelPrefabs;
    //     var container = __instance.StagesPanelContainer;
    //     var gameObject = Gui.GetPrefabFromPool(stagePanelPrefabs[8], container);
    //     var characterStageSpellSelectionPanel = gameObject.GetComponent<CharacterStageSpellSelectionPanel>();
    //     var customFeatureSelection = gameObject.AddComponent<CustomFeatureSelectionPanel>();
    //
    //     customFeatureSelection.Setup(characterStageSpellSelectionPanel);
    //
    //     return customFeatureSelection;
    // }

    internal static void Postfix(CharacterEditionScreen __instance)
    {
        // switch (__instance)
        // {
        //     case CharacterCreationScreen:
        //     {
        //         var customFeatureSelection = GetPanel(__instance);
        //         var last = __instance.stagePanelsByName.ElementAt(__instance.stagePanelsByName.Count - 1);
        //
        //         __instance.stagePanelsByName.Remove(last.Key);
        //         __instance.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
        //         __instance.stagePanelsByName.Add(last.Key, last.Value);
        //         break;
        //     }
        //     case CharacterLevelUpScreen:
        //     {
        //         var customFeatureSelection = GetPanel(__instance);
        //
        //         __instance.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
        //         break;
        //     }
        // }

        //
        // MULTICLASS
        //

        if (Main.Settings.MaxAllowedClasses <= 1 || __instance is not CharacterLevelUpScreen)
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
            new Dictionary<string, CharacterStagePanel> { { "ClassSelection", classSelectionPanel } };

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

//PATCH: Unregisters hero from level up context (MULTICLASS)
[HarmonyPatch(typeof(CharacterEditionScreen), "DoAbort")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterEditionScreen_DoAbort
{
    internal static void Prefix([NotNull] CharacterEditionScreen __instance)
    {
        LevelUpContext.UnregisterHero(__instance.currentHero);
    }
}
