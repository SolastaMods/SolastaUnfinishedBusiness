using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiScreenMap
{
    /// <summary>
    /// Patches to display the location of campfires, entrances and exits on the game location screen map (level map).
    /// </summary>
    [HarmonyPatch(typeof(GameLocationScreenMap), "BindGadgets")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationScreenMap_BindGadgets
    {
        internal static bool Prefix(List<MapGadgetItem> ___mapGadgetItems, 
            ref int ___activeMapGadgetItems, 
            GameObject ___mapGadgetItemPrefab, 
            Transform ___mapItemsTransform, 
            List<MapBaseItem> ___sortedItems)
        {
            if (!Main.Settings.EnableAdditionalIconsOnLevelMap || Gui.GameLocation.UserLocation == null)
            {
                return true;
            }

            // Copy entire method and add additional cases for camp and exit/entrance
            foreach (var gameSector in Gui.GameLocation.GameSectors)
            {
                foreach (var gameGadget in gameSector.GameGadgets)
                {
                    Main.Log($"{gameGadget.UniqueNameId}, {gameGadget.Revealed}");

                    if (gameGadget.Revealed && gameGadget.CheckIsEnabled())
                    {
                        MapGadgetItem.ItemType itemType;

                        if (gameGadget.UniqueNameId.StartsWith("Camp"))
                        {
                            itemType = (MapGadgetItem.ItemType)(-1);
                        }
                        else if (gameGadget.UniqueNameId.IndexOf("exit", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // exits are revealed by default, so all are shown
                            itemType = (MapGadgetItem.ItemType)(-2);
                        }
                        else if (gameGadget.CheckIsLocked())
                        {
                            itemType = MapGadgetItem.ItemType.Lock;
                        }
                        else if (gameGadget.CheckHasActiveDetectedTrap())
                        {
                            itemType = MapGadgetItem.ItemType.Trap;
                        }
                        else if (gameGadget.ItemContainer != null)
                        {
                            itemType = MapGadgetItem.ItemType.Container;
                        }
                        else
                        {
                            continue;
                        }

                        ++___activeMapGadgetItems;
                        for (var index = ___mapGadgetItems.Count - 1; index < ___activeMapGadgetItems; ++index)
                        {
                            var gameObject = Object.Instantiate(___mapGadgetItemPrefab, ___mapItemsTransform);

                            if (gameObject.TryGetComponent<MapGadgetItem>(out var mapGadgetItem))
                            {
                                mapGadgetItem.Unbind();
                                ___mapGadgetItems.Add(mapGadgetItem);
                            }
                        }
                        ___mapGadgetItems[___activeMapGadgetItems - 1].Bind(gameGadget, itemType);
                    }
                }
            }

            for (var index = 0; index < ___activeMapGadgetItems; ++index)
                ___sortedItems.Add(___mapGadgetItems[index]);

            return false;
        }
    }
}
