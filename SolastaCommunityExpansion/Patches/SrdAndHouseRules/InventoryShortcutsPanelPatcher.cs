using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharactersPool
{
    // TODO: modify "Screen/&CastLightButtonDescription"

    [HarmonyPatch(typeof(InventoryShortcutsPanel), "OnCastLightCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryShortcutsPanel_OnCastLightCb
    {
        public static bool InOnCastLightCb { get; private set; }

        public static void Prefix()
        {
            InOnCastLightCb = true;
        }

        public static void Postfix()
        {
            InOnCastLightCb = false;
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "TryFindTargetWieldedItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TryFindTargetWieldedItem
    {
        public static bool Prefix(RulesetInventory ___characterInventory, out RulesetItem targetItem, out bool __result)
        {
            targetItem = null;
            __result = false;

            if (!Main.Settings.QuickCastLightCantripOnWornItemsFirst || !InventoryShortcutsPanel_OnCastLightCb.InOnCastLightCb)
            {
                return true;
            }

            // Head
            RulesetInventorySlot rulesetInventorySlot1 = ___characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeHead];
            if (rulesetInventorySlot1.EquipedItem != null)
            {
                targetItem = rulesetInventorySlot1.EquipedItem;
                __result = true;
                return false;
            }

            // Neck
            RulesetInventorySlot rulesetInventorySlot2 = ___characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeNeck];
            if (rulesetInventorySlot1.EquipedItem != null)
            {
                targetItem = rulesetInventorySlot2.EquipedItem;
                __result = true;
                return false;
            }

            // Torso
            RulesetInventorySlot rulesetInventorySlote = ___characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso];
            if (rulesetInventorySlot2.EquipedItem != null)
            {
                targetItem = rulesetInventorySlote.EquipedItem;
                __result = true;
                return false;
            }

            // Other...

            // Else default MainHand, OffHand
            return true;
        }
    }
}
