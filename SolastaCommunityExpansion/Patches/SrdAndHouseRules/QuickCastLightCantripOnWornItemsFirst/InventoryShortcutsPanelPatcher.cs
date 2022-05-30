using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.QuickCastLightCantripOnWornItemsFirst;
// TODO: modify "Screen/&CastLightButtonDescription"

[HarmonyPatch(typeof(InventoryShortcutsPanel), "OnCastLightCb")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InventoryShortcutsPanel_OnCastLightCb
{
    public static bool MyTryFindTargetWieldedItem(RulesetCharacterHero rulesetCharacterHero,
        out RulesetItem targetItem)
    {
        if (!Main.Settings.QuickCastLightCantripOnWornItemsFirst)
        {
            return rulesetCharacterHero.TryFindTargetWieldedItem(out targetItem);
        }

        var characterInventory = rulesetCharacterHero.CharacterInventory;

        // Head
        var rulesetInventorySlot1 = characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeHead];
        if (rulesetInventorySlot1.EquipedItem != null)
        {
            targetItem = rulesetInventorySlot1.EquipedItem;
            return true;
        }

        // Neck
        var rulesetInventorySlot2 = characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeNeck];
        if (rulesetInventorySlot1.EquipedItem != null)
        {
            targetItem = rulesetInventorySlot2.EquipedItem;
            return true;
        }

        // Torso
        var rulesetInventorySlote = characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso];
        if (rulesetInventorySlot2.EquipedItem != null)
        {
            targetItem = rulesetInventorySlote.EquipedItem;
            return true;
        }

        // Other...

        // Else default MainHand, OffHand
        return rulesetCharacterHero.TryFindTargetWieldedItem(out targetItem);
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var tryFindTargetWieldedItemMethod = typeof(RulesetCharacter).GetMethod("TryFindTargetWieldedItem");
        var myTryFindTargetWieldedItemMethod =
            typeof(InventoryShortcutsPanel_OnCastLightCb).GetMethod("MyTryFindTargetWieldedItem");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(tryFindTargetWieldedItemMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, myTryFindTargetWieldedItemMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
