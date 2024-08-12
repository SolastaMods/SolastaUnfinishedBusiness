using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InventoryShortcutsPanelPatcher
{
    [HarmonyPatch(typeof(InventoryShortcutsPanel), nameof(InventoryShortcutsPanel.OnConfigurationSwitched))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnConfigurationSwitched_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref int rank)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Main.Settings.EnableCtrlClickOnlySwapsMainHand && isCtrlPressed)
            {
                rank += 100;
            }
        }

        [UsedImplicitly]
        public static void Postfix(InventoryShortcutsPanel __instance, int rank)
        {
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

    //PATCH: QuickCastLightCantripOnWornItemsFirst
    [HarmonyPatch(typeof(InventoryShortcutsPanel), nameof(InventoryShortcutsPanel.OnCastLightCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    internal static class OnCastLightCb_Patch
    {
        [UsedImplicitly]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var oldMethod = typeof(RulesetCharacter).GetMethod(nameof(RulesetCharacter.TryFindTargetWieldedItem));
            var newMethod = typeof(OnCastLightCb_Patch).GetMethod(nameof(MyTryFindTargetWieldedItem));

            return instructions.ReplaceCall(oldMethod,1, "InventoryShortcutsPanel.OnCastLightCb",
                new CodeInstruction(OpCodes.Call, newMethod));
        }
        
        public static bool MyTryFindTargetWieldedItem([NotNull] RulesetCharacter rulesetCharacter,
            out RulesetItem targetItem, bool fallbackOnTorsoArmor = false)
        {
            if (!Main.Settings.QuickCastLightCantripOnWornItemsFirst 
                || rulesetCharacter is not RulesetCharacterHero hero)
            {
                return rulesetCharacter.TryFindTargetWieldedItem(out targetItem, fallbackOnTorsoArmor);
            }

            var slots = hero.CharacterInventory.InventorySlotsByName;
            
            targetItem = slots[EquipmentDefinitions.SlotTypeHead].EquipedItem
                         ?? slots[EquipmentDefinitions.SlotTypeNeck].EquipedItem
                         ?? slots[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

            return targetItem != null || hero.TryFindTargetWieldedItem(out targetItem);
        }
    }
}
