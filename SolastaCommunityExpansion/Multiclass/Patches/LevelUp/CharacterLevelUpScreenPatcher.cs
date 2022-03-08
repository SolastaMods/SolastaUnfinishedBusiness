using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterLevelUpScreenPatcher
    {
        //
        // WILL FIX THIS LATER
        //

        //// add the class selection stage panel to the level up screen
        //[HarmonyPatch(typeof(CharacterLevelUpScreen), "LoadStagePanels")]
        //internal static class CharacterLevelUpScreenLoadStagePanels
        //{
        //    internal static void Postfix(CharacterLevelUpScreen __instance)
        //    {
        //        if (!Main.Settings.EnableMulticlass)
        //        {
        //            return;
        //        }

        //        var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
        //        var stagePanelPrefabs = characterCreationScreen.GetField<CharacterCreationScreen, GameObject[]>("stagePanelPrefabs");
        //        var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
        //        var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
        //        var stagePanelsByName = __instance.GetField<CharacterLevelUpScreen, Dictionary<string, CharacterStagePanel>>("stagePanelsByName");

        //        __instance.SetField("stagePanelsByName", new Dictionary<string, CharacterStagePanel>
        //        {
        //            { "ClassSelection", classSelectionPanel },
        //            { "LevelGains", stagePanelsByName["LevelGains"] },
        //            { "DeitySelection", deitySelectionPanel },
        //            { "SubclassSelection", stagePanelsByName["SubclassSelection"] },
        //            { "AbilityScores", stagePanelsByName["AbilityScores"] },
        //            { "FightingStyleSelection", stagePanelsByName["FightingStyleSelection"] },
        //            { "ProficiencySelection", stagePanelsByName["ProficiencySelection"] },
        //            { "", stagePanelsByName[""] }
        //        });
        //    }
        //}

        // binds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginShow")]
        internal static class CharacterLevelUpScreenOnBeginShow
        {
            internal static void Postfix(CharacterLevelUpScreen __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.LevelUpContext.SelectedHero = __instance.CharacterBuildingService.CurrentLocalHeroCharacter;
            }
        }

        // unbinds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginHide")]
        internal static class CharacterLevelUpScreenOnBeginHide
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.LevelUpContext.SelectedHero = null;
            }
        }
    }
}
