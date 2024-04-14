using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InventorySlotBoxPatcher
{
    [HarmonyPatch(typeof(InventorySlotBox), nameof(InventorySlotBox.RefreshState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshState_Patch
    {
        [UsedImplicitly]
        public static void Postfix(InventorySlotBox __instance, bool overrideItem, RulesetItem itemOverride)
        {
            var equipmentItem = overrideItem ? itemOverride : __instance.InventorySlot?.EquipedItem;
            if (equipmentItem == null)
            {
                return;
            }

            var itemDefinition = equipmentItem.ItemDefinition;

            //PATCH: Enable inventory taint non proficient items in red (paint them red)
            TintNonProficientItems(__instance, itemDefinition);
            ModifyCustomItemFlags(__instance, itemDefinition);
        }

        private static void ModifyCustomItemFlags(InventorySlotBox box, ItemDefinition definition)
        {
            if (!box.itemFlagsTableInstance)
            {
                return;
            }

            var flagsTransform = box.itemFlagsTableInstance.transform;
            var n = flagsTransform.childCount;

            for (var index = 0; index < n; index++)
            {
                var child = flagsTransform.GetChild(index);
                var img = child.GetComponent<Image>();
                var flag = definition.ItemPresentation.ItemFlags[index];
                child.TryGetComponent<GuiTooltip>(out var tooltip);
                var custom = flag.GetFirstSubFeatureOfType<RecipeHelper.TooltipModifier<ItemDefinition>>();
                custom?.Invoke(tooltip, img, child, definition, null);
            }
        }

        private static void TintNonProficientItems(InventorySlotBox box, ItemDefinition item)
        {
            var hero = box.GuiCharacter?.RulesetCharacterHero ?? Global.InspectedHero;

            if (!item || !box.equipedItemImage)
            {
                return;
            }

            var color = Color.white;
            if ((Main.Settings.EnableInventoryTaintNonProficientItemsRed && !(hero?.IsProficientWithItem(item) ?? true))
                || (Main.Settings.EnableInventoryTintKnownRecipesRed && RecipeHelper.RecipeIsKnown(item)))
            {
                color = Color.red;
            }

            box.equipedItemImage.color = color;
        }
    }


    [HarmonyPatch(typeof(InventorySlotBox), nameof(InventorySlotBox.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        // this should not have any protection to keep the house clean
        [UsedImplicitly]
        public static void Prefix(InventorySlotBox __instance)
        {
            //PATCH: Enable inventory taint non proficient items in red (paint them back white)
            if (!__instance.equipedItemImage)
            {
                return;
            }

            __instance.equipedItemImage.color = new Color(1, 1, 1);
        }
    }

    [HarmonyPatch(typeof(InventorySlotBox), nameof(InventorySlotBox.UnbindItemFlags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnbindItemFlags_Patch
    {
        [UsedImplicitly]
        public static void Prefix(InventorySlotBox __instance)
        {
            if (!__instance.itemFlagsTableInstance)
            {
                return;
            }

            var flagsTransform = __instance.itemFlagsTableInstance.transform;
            var n = flagsTransform.childCount;

            for (var index = 0; index < n; index++)
            {
                var child = flagsTransform.GetChild(index);
                child.localScale = Vector3.one;
                if (child.TryGetComponent<GuiTooltip>(out var tooltip))
                {
                    tooltip.Clear();
                }
            }
        }
    }
}
