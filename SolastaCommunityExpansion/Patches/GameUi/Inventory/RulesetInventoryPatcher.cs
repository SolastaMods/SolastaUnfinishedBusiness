using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Inventory
{
    [HarmonyPatch(typeof(RulesetInventory), "BuildSlots")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetInventory_BuildSlots
    {
        internal static void Postfix(RulesetInventory __instance)
        {
            if (!Main.Settings.EnableInventoryTertiaryEquipmentRow)
            {
                return;
            }

            var tertiaryWieldedConfiguration = __instance.WieldedItemsConfigurations[EquipmentDefinitions.MaxWieldedItemsConfigurations - 1];

            tertiaryWieldedConfiguration.MainHandSlot.Disabled = false;
            tertiaryWieldedConfiguration.MainHandSlot.DisabledReason = RulesetInventorySlot.DisableReason.None;
            tertiaryWieldedConfiguration.OffHandSlot.RestrictedItemTags.Clear();
        }
    }

    [HarmonyPatch(typeof(RulesetInventory), "RefreshWieldedItemsConfigurations")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetInventory_RefreshWieldedItemsConfigurations
    {
        internal static void Postfix(RulesetInventory __instance)
        {
            if (!Main.Settings.EnableInventoryTertiaryEquipmentRow)
            {
                return;
            }

            var tertiaryWieldedConfiguration = __instance.WieldedItemsConfigurations[EquipmentDefinitions.MaxWieldedItemsConfigurations - 1];

            tertiaryWieldedConfiguration.MainHandSlot.Disabled = false;
            tertiaryWieldedConfiguration.MainHandSlot.ShadowedSlot = null;
        }
    }

    [HarmonyPatch(typeof(RulesetInventory), "UpdateLightSourceConfigurations")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetInventory_UpdateLightSourceConfigurations
    {
        internal static void Postfix(RulesetInventory __instance)
        {
            if (!Main.Settings.EnableInventoryTertiaryEquipmentRow)
            {
                return;
            }

            var tertiaryWieldedConfiguration = __instance.WieldedItemsConfigurations[EquipmentDefinitions.MaxWieldedItemsConfigurations - 1];

            tertiaryWieldedConfiguration.MainHandSlot.Disabled = false;
            tertiaryWieldedConfiguration.MainHandSlot.ShadowedSlot = null;
        }
    }
}
