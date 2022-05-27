using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Utils;
using UnityEngine;
using UnityEngine.UI;
using Resources = SolastaCommunityExpansion.Properties.Resources;

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
            Image ___backgroundImage,
            Image ___iconImage,
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
                    ___iconImage.sprite = CustomIcons.GetOrCreateSprite("Fire", Resources.Fire, 24);
                    ___guiTooltip.Content = "Camp";
                    break;
                case -2:
                    ___backgroundImage.sprite = ___backgroundSprites[2];
                    ___iconImage.sprite =
                        CustomIcons.GetOrCreateSprite("Entrance", Resources.Entry, 24);
                    ___guiTooltip.Content = "Exit";
                    break;
                case -3:
                    ___backgroundImage.sprite = ___backgroundSprites[2];
                    ___iconImage.sprite =
                        CustomIcons.GetOrCreateSprite("Teleport", Resources.Teleport, 24);
                    ___guiTooltip.Content = "Teleporter";
                    break;
                default:
                    return true;
            }

            ___gameGadget = gameGadget;
            __instance.gameObject.SetActive(true);
            return false;
        }
    }
}
