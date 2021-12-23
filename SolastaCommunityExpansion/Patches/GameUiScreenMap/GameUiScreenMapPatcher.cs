using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
        internal static bool Prefix(List<MapGadgetItem> ___mapGadgetItems, ref int ___activeMapGadgetItems, GameObject ___mapGadgetItemPrefab, Transform ___mapItemsTransform, List<MapBaseItem> ___sortedItems)
        {
            if (!Main.Settings.AdditionalIconsOnLevelMap)
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
                        else if (gameGadget.UniqueNameId.IndexOf("entrance", System.StringComparison.OrdinalIgnoreCase) >= 0
                            || gameGadget.UniqueNameId.IndexOf("exit", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // seems like exits and entrances are revealed by default
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

    [HarmonyPatch(typeof(MapGadgetItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class MapGadgetItem_Bind
    {
        internal static bool Prefix(
            MapGadgetItem __instance,
            GameGadget gameGadget, MapGadgetItem.ItemType itemType,
            ref GameGadget ___gameGadget,
            GuiTooltip ___guiTooltip,
            UnityEngine.UI.Image ___backgroundImage,
            UnityEngine.UI.Image ___iconImage,
            Sprite[] ___backgroundSprites)
        {
            if (!Main.Settings.AdditionalIconsOnLevelMap)
            {
                return true;
            }

            // Handle standard item types in game code
            if (itemType >= 0)
            {
                return true;
            }

            switch ((int)itemType)
            {
                case -1:
                    ___backgroundImage.sprite = ___backgroundSprites[2];
                    ___iconImage.sprite = CreateSprite(Properties.Resources.Fire, 24);
                    ___guiTooltip.Content = "Camp";
                    break;
                case -2:
                    ___backgroundImage.sprite = ___backgroundSprites[2];
                    ___iconImage.sprite = CreateSprite(Properties.Resources.Entry, 24);
                    ___guiTooltip.Content = "Entrance or exit";
                    break;
                default:
                    return true;
            }

            ___gameGadget = gameGadget;
            __instance.gameObject.SetActive(true);
            return false;

            Sprite CreateSprite(Bitmap bitmap, int size)
            {
                if (!_spritesCache.TryGetValue(bitmap, out var sprite))
                {
                    var texture = new Texture2D(size, size, TextureFormat.DXT5, false);
                    texture.LoadImage((byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])));
                    sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0, 0));
                    _spritesCache[bitmap] = sprite;
                }

                return sprite;
            }
        }

        internal static readonly Dictionary<Bitmap, Sprite> _spritesCache = new Dictionary<Bitmap, Sprite>();
    }
}
