using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationScreenMapPatcher
{
    //PATCH: displays the location of campfires, entrances and exits on the game location screen map (level map)
    [HarmonyPatch(typeof(GameLocationScreenMap), nameof(GameLocationScreenMap.BindGadgets))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BindGadgets_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationScreenMap __instance)
        {
            if (!Main.Settings.EnableAdditionalIconsOnLevelMap || Gui.GameLocation.UserLocation == null)
            {
                return true;
            }

            // Add additional cases for camp and exit/entrance, and change behaviour to account for
            // 1) Exits have Enable and Param_Enabled states
            // 2) Teleporters have an Invisible state
            foreach (var gameSector in Gui.GameLocation.GameSectors)
            {
                foreach (var gameGadget in gameSector.GameGadgets)
                {
                    var isEnabled = gameGadget.CheckIsEnabled();
                    var isInvisible = gameGadget.IsInvisible();
                    var isRevealed = gameGadget.Revealed;

                    // ReSharper disable once InvocationIsSkipped
                    Main.Log(
                        $"{gameGadget.UniqueNameId}, Revealed={isRevealed}, Enabled={isEnabled}, Invisible={isInvisible}");

                    if (!isRevealed || !isEnabled)
                    {
                        continue;
                    }

                    var itemType = (MapGadgetItem.ItemType)int.MinValue;

                    if (gameGadget.IsCamp())
                    {
                        itemType = (MapGadgetItem.ItemType)(-1);
                    }
                    else if (gameGadget.IsExit())
                    {
                        itemType = (MapGadgetItem.ItemType)(-2);
                    }
                    else if (gameGadget.IsTeleport()
                             && (Main.Settings.MarkInvisibleTeleportersOnLevelMap || !isInvisible))
                    {
                        itemType = (MapGadgetItem.ItemType)(-3);
                    }
                    else if (gameGadget.CheckIsLocked())
                    {
                        itemType = MapGadgetItem.ItemType.Lock;
                    }
                    //TODO: check why triggered traps are shown on map
                    else if (gameGadget.CheckHasActiveDetectedTrap())
                    {
                        itemType = MapGadgetItem.ItemType.Trap;
                    }
                    else if (gameGadget.ItemContainer != null)
                    {
                        itemType = MapGadgetItem.ItemType.Container;
                    }

                    if ((int)itemType <= int.MinValue)
                    {
                        continue;
                    }

                    ++__instance.activeMapGadgetItems;

                    for (var index = __instance.mapGadgetItems.Count - 1;
                         index < __instance.activeMapGadgetItems;
                         ++index)
                    {
                        var gameObject = Object.Instantiate(__instance.mapGadgetItemPrefab,
                            __instance.mapItemsTransform);

                        if (!gameObject.TryGetComponent<MapGadgetItem>(out var mapGadgetItem))
                        {
                            continue;
                        }

                        mapGadgetItem.Unbind();
                        __instance.mapGadgetItems.Add(mapGadgetItem);
                    }

                    __instance.mapGadgetItems[__instance.activeMapGadgetItems - 1].Bind(gameGadget, itemType);
                }
            }

            for (var index = 0; index < __instance.activeMapGadgetItems; ++index)
            {
                __instance.sortedItems.Add(__instance.mapGadgetItems[index]);
            }

            return false;
        }
    }
}
