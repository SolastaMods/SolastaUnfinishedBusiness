using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class InventoryShortcutsPanelPatcher
{
    [HarmonyPatch(typeof(InventoryShortcutsPanel), "OnConfigurationSwitched")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnConfigurationSwitched_Patch
    {
        public static void Prefix(ref int rank)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Main.Settings.EnableCtrlClickOnlySwapsMainHand && isCtrlPressed)
            {
                rank += 100;
            }
        }

        public static void Postfix(InventoryShortcutsPanel __instance, int rank)
        {
            var gameLocationCharacter = __instance.GuiCharacter.gameLocationCharacter;

            if (gameLocationCharacter != null && gameLocationCharacter.RulesetCharacter
                    .HasSubFeatureOfType<IUnlimitedFreeAction>())
            {
                if (gameLocationCharacter.currentActionRankByType[ActionDefinitions.ActionType.FreeOnce] > 0)
                {
                    gameLocationCharacter.currentActionRankByType[ActionDefinitions.ActionType.FreeOnce]--;
                }
            }

            if (rank < 100)
            {
                return;
            }

            rank -= 100;

            var itemsConfigurations = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory
                .WieldedItemsConfigurations;

            for (var index = 0; index < itemsConfigurations.Count; ++index)
            {
                __instance.configurationsTable.GetChild(index).GetComponent<WieldedConfigurationSelector>().Selected =
                    index == rank;
            }
        }
    }
}
