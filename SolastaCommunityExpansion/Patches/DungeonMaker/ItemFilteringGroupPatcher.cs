using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    [HarmonyPatch(typeof(ItemFilteringGroup), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemFilteringGroup_Refresh
    {
        public static bool Prefix(
            ItemFilteringGroup __instance, 
            List<ItemDefinition> ___itemsList,
            List<GuiItemDefinition> ___guiItemDefinitions, 
            string ___keyword, string ___currentCategory,
            ref bool ___refreshing, 
            bool ___showCustom,
            bool notify)
        {
            if (!Main.Settings.FixItemFiltering)
            {
                return true;
            }

            if (___itemsList == null)
            {
                return false;
            }

            ___refreshing = true;
            __instance.FilteredItemsList.Clear();
            
            for (int index = 0; index < ___itemsList.Count; ++index)
            {
                ItemDefinition items = ___itemsList[index];
                GuiItemDefinition guiItemDefinition = ___guiItemDefinitions[index];

                // The only change from original code is to use '>-1' not '>0'
                if (
                    (string.IsNullOrEmpty(___keyword) || 
                     guiItemDefinition.Title.IndexOf(___keyword, StringComparison.OrdinalIgnoreCase) > -1) 
                     && (___showCustom || !items.UserItem))
                {
                    if (___currentCategory == "All")
                        __instance.FilteredItemsList.Add(items);
                    else if (___currentCategory == "Custom" && items.UserItem)
                        __instance.FilteredItemsList.Add(items);
                    else if (items.MerchantCategory == ___currentCategory)
                        __instance.FilteredItemsList.Add(items);
                }
            }

            if (notify)
            {
                __instance.ItemsFiltered?.Invoke(__instance.FilteredItemsList);
            }

            ___refreshing = false;

            return false;
        }
    }
}
