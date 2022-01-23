using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.ScreenMap
{
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
            if (!Main.Settings.EnableAdditionalIconsOnLevelMap || Gui.GameLocation.UserLocation == null)
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
                    ___guiTooltip.Content = "Exit";
                    break;
                case -3:
                    ___backgroundImage.sprite = ___backgroundSprites[2];
                    ___iconImage.sprite = CreateSprite(Properties.Resources.Teleport, 24);
                    ___guiTooltip.Content = "Teleporter";
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
