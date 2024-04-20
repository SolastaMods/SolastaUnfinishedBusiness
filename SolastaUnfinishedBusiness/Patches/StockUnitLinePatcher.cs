using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class StockUnitLinePatcher
{
    private static Transform GetRecipeItem(Transform factionGroup, bool justRetrieve)
    {
        const string NAME = "RECIPE_ITEM";

        var t = factionGroup.parent.Find(NAME);

        if (t || justRetrieve)
        {
            return t;
        }

        var item = Object.Instantiate(factionGroup.gameObject, factionGroup.parent);
        item.name = NAME;
        item.SetActive(true);
        t = item.GetComponent<RectTransform>();
        t.localPosition = new Vector3(-100, 0);
        t.localScale = new Vector3(1.5f, 1.5f, 1f);

        return t;
    }

    private static Image SetupCraftedItem(Transform t, BaseDefinition item)
    {
        if (!t)
        {
            return null;
        }

        var img = t.Find("IncompatibleImage").GetComponent<Image>();
        var tooltip = t.GetComponent<GuiTooltip>();
        if (item)
        {
            img.color = Color.white;
            img.sprite = Gui.LoadAssetSync<Sprite>(item.GuiPresentation.SpriteReference);
            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiItemDefinition(item.Name)
                .SetupTooltip(tooltip);
        }
        else if (img.sprite)
        {
            Gui.ReleaseAddressableAsset(img.sprite);
            img.sprite = null;
            tooltip.Clear();
        }

        return img;
    }

    [HarmonyPatch(typeof(StockUnitLine), nameof(StockUnitLine.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(StockUnitLine __instance)
        {
            var item = GetRecipeItem(__instance.factionIncompatibleGroup, false);
            var crafted = Main.Settings.ShowCraftedItemOnRecipeIcon
                ? RecipeHelper.GetCraftedItem(__instance.StockUnit.ItemDefinition)
                : null;

            if (!crafted)
            {
                item.gameObject.SetActive(false);
                __instance.itemImage.transform.localPosition = new Vector3(33, 0, 0);
            }
            else
            {
                item.gameObject.SetActive(true);
                __instance.itemImage.transform.localPosition = new Vector3(58, 0, 0);

                var img = SetupCraftedItem(item, crafted);

                if (Main.Settings.SwapCraftedItemAndRecipeIcons)
                {
                    (img.sprite, __instance.itemImage.sprite) = (__instance.itemImage.sprite, img.sprite);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StockUnitLine), nameof(StockUnitLine.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(StockUnitLine __instance)
        {
            SetupCraftedItem(GetRecipeItem(__instance.factionIncompatibleGroup, true), null);
        }
    }
}
