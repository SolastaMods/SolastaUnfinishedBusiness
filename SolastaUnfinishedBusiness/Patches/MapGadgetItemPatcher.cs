using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Patches;

public static class MapGadgetItemPatcher
{
    [HarmonyPatch(typeof(MapGadgetItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static bool Prefix(
            MapGadgetItem __instance,
            GameGadget gameGadget,
            MapGadgetItem.ItemType itemType)
        {
            //PATCH: EnableAdditionalIconsOnLevelMap
            if (!Main.Settings.EnableAdditionalIconsOnLevelMap || Gui.GameLocation.UserLocation == null)
            {
                return true;
            }

            // Handle standard item types in game code
            if (itemType >= 0)
            {
                return true;
            }

            __instance.iconImage.color = Color.white;

            switch ((int)itemType)
            {
                case -1:
                    __instance.backgroundImage.sprite = __instance.backgroundSprites[2];
                    __instance.iconImage.sprite = Sprites.GetOrCreateSprite("Fire", Resources.Fire, 24);
                    __instance.guiTooltip.Content = Gui.Localize("Tooltip/&CustomMapMarkerCamp");

                    break;
                case -2:
                    __instance.backgroundImage.sprite = __instance.backgroundSprites[2];
                    __instance.iconImage.sprite =
                        Sprites.GetOrCreateSprite("Entrance", Resources.Entry, 24);
                    __instance.guiTooltip.Content = GetGadgetDestinationLocation(gameGadget)
                                                    ?? Gui.Localize("Tooltip/&CustomMapMarkerExit");
                    break;
                case -3:
                    __instance.backgroundImage.sprite = __instance.backgroundSprites[2];
                    __instance.iconImage.sprite =
                        Sprites.GetOrCreateSprite("Teleport", Resources.Teleport, 24);
                    __instance.guiTooltip.Content = Gui.Localize("Tooltip/&CustomMapMarkerTeleport");

                    break;
                default:
                    return true;
            }

            __instance.gameGadget = gameGadget;
            __instance.gameObject.SetActive(true);

            return false;
        }

        private static string GetGadgetDestinationLocation(GameGadget gameGadget)
        {
            return gameGadget.ActiveListeners
                .SelectMany(x => x.FunctorParams)
                .OfType<FunctorParametersDescription>()
                .Where(x => x.LocationDefinition != null && !string.IsNullOrEmpty(x.StringParameter2))
                .Select(p => p.StringParameter2)
                .FirstOrDefault();
        }
    }
}
