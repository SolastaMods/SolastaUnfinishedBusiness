using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetInventoryPatcher
{
    [HarmonyPatch(typeof(RulesetInventory), nameof(RulesetInventory.SwitchToWieldItemsOfConfiguration))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SwitchToWieldItemsOfConfiguration_Patch
    {
        private const int LightSource = 2;

        [UsedImplicitly]
        public static void Prefix(RulesetInventory __instance, ref int rank)
        {
            if (rank < 100)
            {
                return;
            }

            rank -= 100;

            var itemsConfigurations = __instance.WieldedItemsConfigurations;
            var currentRank = __instance.CurrentConfiguration;

            if (rank == LightSource || currentRank == LightSource)
            {
                return;
            }

            if (itemsConfigurations[rank].OffHandSlot.Disabled || itemsConfigurations[currentRank].OffHandSlot.Disabled)
            {
                return;
            }

            var equippedItem0 = itemsConfigurations[rank].OffHandSlot.EquipedItem;
            var equippedItem1 = itemsConfigurations[currentRank].OffHandSlot.EquipedItem;

            itemsConfigurations[rank].OffHandSlot.UnequipItem(silent: true);
            itemsConfigurations[currentRank].OffHandSlot.UnequipItem(silent: true);
            itemsConfigurations[rank].OffHandSlot.EquipItem(equippedItem1, silent: true);
            itemsConfigurations[currentRank].OffHandSlot.EquipItem(equippedItem0, silent: true);
        }
    }
}
