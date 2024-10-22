using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Displays;

internal static class CraftingAndItems
{
    private const int MaxColumns = 1;

    private static readonly (string, Func<ItemDefinition, bool>)[] ItemsFilters =
    [
        (Gui.Localize("MainMenu/&CharacterSourceToggleAllTitle"), _ => true),
        (Gui.Localize("Equipment/&ItemTypeAmmunitionTitle"), a => a.IsAmmunition),
        (Gui.Localize("MerchantCategory/&ArmorTitle"), a => a.IsArmor),
        (Gui.Localize("MerchantCategory/&DocumentTitle"), a => a.IsDocument),
        (Gui.Localize("Equipment/&ItemTypeSpellFocusTitle"), a => a.IsFocusItem),
        (Gui.Localize("Screen/&TravelFoodTitle"), a => a.IsFood),
        (Gui.Localize("Equipment/&ItemTypeLightSourceTitle"), a => a.IsLightSourceItem),
        (Gui.Localize("Equipment/&SpellbookTitle"), a => a.IsSpellbook),
        (Gui.Localize("Equipment/&ItemTypeStarterPackTitle"), a => a.IsStarterPack),
        (Gui.Localize("Screen/&ProficiencyToggleToolTitle"), a => a.IsTool),
        (Gui.Localize("Merchant/&DungeonMakerMagicalDevicesTitle"), a => a.IsUsableDevice),
        (Gui.Localize("MerchantCategory/&WeaponTitle"), a => a.IsWeapon),
        (Gui.Localize("Tooltip/&TagFactionRelicTitle"), a => a.IsFactionRelic)
    ];

    private static readonly string[] ItemsFiltersLabels = ItemsFilters.Select(x => x.Item1).ToArray();

    private static readonly (string, Func<ItemDefinition, bool>)[] ItemsItemTagsFilters =
    [
        .. TagsDefinitions.AllItemTags
            .Select<string, (string, Func<ItemDefinition, bool>)>(x =>
                (Gui.Localize($"Tooltip/&Tag{x}Title"), a => a.ItemTags.Contains(x)))
            .AddItem((Gui.Localize("MainMenu/&CharacterSourceToggleAllTitle"), _ => true))
            .OrderBy(x => x.Item1)
    ];

    private static readonly int WeaponIndexItemFilters =
        Array.FindIndex(ItemsFilters, x => x.Item1 == Gui.Localize("MerchantCategory/&WeaponTitle"));

    private static readonly string[] ItemsItemTagsFiltersLabels = ItemsItemTagsFilters.Select(x => x.Item1).ToArray();

    private static readonly (string, Func<ItemDefinition, bool>)[] ItemsWeaponTagsFilters =
    [
        .. TagsDefinitions.AllWeaponTags
            .Select<string, (string, Func<ItemDefinition, bool>)>(x =>
                (Gui.Localize($"Tooltip/&Tag{x}Title"),
                    a => a.IsWeapon && a.WeaponDescription.WeaponTags.Contains(x)))
            .AddItem((Gui.Localize("MainMenu/&CharacterSourceToggleAllTitle"), _ => true))
            .AddItem((Gui.Localize("Tooltip/&TagRangeTitle"),
                a => a.IsWeapon && a.WeaponDescription.WeaponTags.Contains("Range")))
            .OrderBy(x => x.Item1)
    ];

    private static readonly int AllTitleIndexWeaponTagsFilters =
        Array.FindIndex(ItemsWeaponTagsFilters,
            x => x.Item1 == Gui.Localize("MainMenu/&CharacterSourceToggleAllTitle"));

    private static readonly string[] ItemsWeaponTagsFiltersLabels =
        ItemsWeaponTagsFilters.Select(x => x.Item1).ToArray();

    private static Vector2 ItemPosition { get; set; } = Vector2.zero;

    private static int CurrentItemsFilterIndex { get; set; }

    private static int CurrentItemsItemTagsFilterIndex { get; set; }

    private static int CurrentItemsWeaponTagsFilterIndex { get; set; }

    internal static void DisplayCraftingAndItems()
    {
        UI.Label();
        UI.Label();

        var toggle = Main.Settings.AllowAnyClassToUseArcaneShieldstaff;
        if (UI.Toggle(Gui.Localize("ModUi/&ArcaneShieldstaffOptions"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToUseArcaneShieldstaff = toggle;
            ItemCraftingMerchantContext.SwitchAttuneArcaneShieldstaff();
        }

        toggle = Main.Settings.IdentifyAfterRest;
        if (UI.Toggle(Gui.Localize("ModUi/&IdentifyAfterRest"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IdentifyAfterRest = toggle;
        }

        toggle = Main.Settings.IncreaseMaxAttunedItems;
        if (UI.Toggle(Gui.Localize("ModUi/&IncreaseMaxAttunedItems"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IncreaseMaxAttunedItems = toggle;
        }

        toggle = Main.Settings.RemoveAttunementRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&RemoveAttunementRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.RemoveAttunementRequirements = toggle;
        }

        UI.Label();

        toggle = Main.Settings.AllowAnyClassToWearSylvanArmor;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowAnyClassToWearSylvanArmor"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowAnyClassToWearSylvanArmor = toggle;
            SrdAndHouseRulesContext.SwitchUniversalSylvanArmorAndLightbringer();
        }

        toggle = Main.Settings.AllowClubsToBeThrown;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowClubsToBeThrown"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AllowClubsToBeThrown = toggle;
            SrdAndHouseRulesContext.SwitchAllowClubsToBeThrown();
        }

        toggle = Main.Settings.IgnoreHandXbowFreeHandRequirements;
        if (UI.Toggle(Gui.Localize("ModUi/&IgnoreHandXbowFreeHandRequirements"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.IgnoreHandXbowFreeHandRequirements = toggle;
        }

        toggle = Main.Settings.MakeAllMagicStaveArcaneFoci;
        if (UI.Toggle(Gui.Localize("ModUi/&MakeAllMagicStaveArcaneFoci"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.MakeAllMagicStaveArcaneFoci = toggle;
            SrdAndHouseRulesContext.SwitchMagicStaffFoci();
        }

        toggle = Main.Settings.FixRingOfRegenerationHealRate;
        if (UI.Toggle(Gui.Localize("ModUi/&FixRingOfRegenerationHealRate"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.FixRingOfRegenerationHealRate = toggle;
            SrdAndHouseRulesContext.SwitchRingOfRegenerationHealRate();
        }

        #region Item

        UI.Label();

        toggle = Main.Settings.AddCustomIconsToOfficialItems;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddCustomIconsToOfficialItems")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddCustomIconsToOfficialItems = toggle;
        }

        toggle = Main.Settings.DisableAutoEquip;
        if (UI.Toggle(Gui.Localize("ModUi/&DisableAutoEquip"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.DisableAutoEquip = toggle;
        }

        toggle = Main.Settings.EnableInventoryFilteringAndSorting;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryFilteringAndSorting"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryFilteringAndSorting = toggle;
            InventoryManagementContext.RefreshControlsVisibility();
        }

        UI.Label();

        toggle = Main.Settings.EnableInventoryTaintNonProficientItemsRed;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryTaintNonProficientItemsRed"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryTaintNonProficientItemsRed = toggle;
        }

        toggle = Main.Settings.EnableInventoryTintKnownRecipesRed;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableInventoryTintKnownRecipesRed"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableInventoryTintKnownRecipesRed = toggle;
        }

        UI.Label();

        toggle = Main.Settings.ShowCraftingRecipeInDetailedTooltips;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowCraftingRecipeInDetailedTooltips"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowCraftingRecipeInDetailedTooltips = toggle;
        }

        toggle = Main.Settings.ShowCraftedItemOnRecipeIcon;
        if (UI.Toggle(Gui.Localize("ModUi/&ShowCraftedItemOnRecipeIcon"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.ShowCraftedItemOnRecipeIcon = toggle;
        }

        if (Main.Settings.ShowCraftedItemOnRecipeIcon)
        {
            toggle = Main.Settings.SwapCraftedItemAndRecipeIcons;
            if (UI.Toggle(Gui.Localize("ModUi/&SwapCraftedItemAndRecipeIcons"), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.SwapCraftedItemAndRecipeIcons = toggle;
            }
        }

        UI.Label();

        toggle = Main.Settings.AddPickPocketableLoot;
        if (UI.Toggle(Gui.Localize("ModUi/&AddPickPocketableLoot"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddPickPocketableLoot = toggle;
            if (toggle)
            {
                PickPocketContext.Load();
            }
        }

        UI.Label();

        var intValue = Main.Settings.SetBeltOfDwarvenKindBeardChances;
        if (UI.Slider(Gui.Localize("ModUi/&SetBeltOfDwarvenKindBeardChances"), ref intValue,
                0, 100, 50, "%", UI.Width(500f)))
        {
            Main.Settings.SetBeltOfDwarvenKindBeardChances = intValue;
            ItemCraftingMerchantContext.SwitchSetBeltOfDwarvenKindBeardChances();
        }

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&EmpressGarbAppearance"), UI.Width(325f));

            intValue = Main.Settings.EmpressGarbAppearanceIndex;
            // ReSharper disable once InvertIf
            if (UI.SelectionGrid(ref intValue, ItemCraftingMerchantContext.EmpressGarbAppearances,
                    ItemCraftingMerchantContext.EmpressGarbAppearances.Length, 2, UI.Width(440f)))
            {
                Main.Settings.EmpressGarbAppearanceIndex = intValue;
                GameUiContext.SwitchEmpressGarb();
            }
        }

        #endregion

        DisplayCrafting();
        DisplayItems();

        UI.Label();
    }

    private static void DisplayCrafting()
    {
        UI.Label();

        var toggle = Main.Settings.DisplayCraftingToggle;
        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Crafting"), ref toggle, 200))
        {
            Main.Settings.DisplayCraftingToggle = toggle;
        }

        if (!Main.Settings.DisplayCraftingToggle)
        {
            return;
        }

        UI.Label();

        toggle = Main.Settings.AddNewWeaponsAndRecipesToShops;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&AddNewWeaponsAndRecipesToShops")), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.AddNewWeaponsAndRecipesToShops = toggle;
        }

        UI.Label();

        var intValue = Main.Settings.RecipeCost;
        if (UI.Slider(Gui.Localize("ModUi/&RecipeCost"), ref intValue, 1, 500, 200, "G", UI.AutoWidth()))
        {
            Main.Settings.RecipeCost = intValue;
            CraftingContext.UpdateRecipeCost();
        }

        intValue = Main.Settings.TotalCraftingTimeModifier;
        if (UI.Slider(Gui.Localize("ModUi/&TotalCraftingTimeModifier"), ref intValue, 0, 100, 0, "%", UI.AutoWidth()))
        {
            Main.Settings.TotalCraftingTimeModifier = intValue;
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&CraftingHelp"));
        UI.Label();

        using (UI.HorizontalScope(UI.AutoWidth()))
        {
            UI.Space(204f);

            toggle = CraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingInStore.Count;
            if (UI.Toggle(Gui.Localize("ModUi/&AddAllToStore"), ref toggle, UI.Width(125f)))
            {
                Main.Settings.CraftingInStore.Clear();

                if (toggle)
                {
                    Main.Settings.CraftingInStore.AddRange(CraftingContext.RecipeBooks.Keys);
                }
            }

            toggle = CraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingRecipesInDm.Count;
            if (UI.Toggle(Gui.Localize("ModUi/&AllRecipesInDm"), ref toggle, UI.Width(125f)))
            {
                Main.Settings.CraftingRecipesInDm.Clear();

                if (toggle)
                {
                    Main.Settings.CraftingRecipesInDm.AddRange(CraftingContext.RecipeBooks.Keys);
                }
            }

            toggle = CraftingContext.RecipeBooks.Keys.Count == Main.Settings.CraftingItemsInDm.Count;
            if (UI.Toggle(Gui.Localize("ModUi/&AllItemInDm"), ref toggle, UI.Width(125f)))
            {
                Main.Settings.CraftingItemsInDm.Clear();

                if (toggle)
                {
                    Main.Settings.CraftingItemsInDm.AddRange(CraftingContext.RecipeBooks.Keys);
                }
            }
        }

        UI.Label();

        var keys = CraftingContext.RecipeBooks.Keys;
        var current = 0;
        var count = keys.Count;

        while (current < count)
        {
            var cols = 0;

            using (UI.HorizontalScope())
            {
                while (current < count && cols < MaxColumns)
                {
                    AddUIForWeaponKey(keys.ElementAt(current));

                    cols++;
                    current++;
                }
            }
        }
    }

    private static void DisplayItems()
    {
        var toggle = Main.Settings.DisplayItemsToggle;

        UI.Label();

        if (UI.DisclosureToggle(Gui.Localize("ModUi/&Items"), ref toggle))
        {
            Main.Settings.DisplayItemsToggle = toggle;
        }

        if (!Main.Settings.DisplayItemsToggle)
        {
            return;
        }

        UI.Label();

        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();

        if (!characterInspectionScreen.Visible || characterInspectionScreen.externalContainer == null)
        {
            UI.Label(Gui.Localize("ModUi/&ItemsHelp1"));

            return;
        }

        using (UI.HorizontalScope())
        {
            UI.Space(40f);
            UI.Label("Category".Bold(), UI.Width(100f));

            if (CurrentItemsFilterIndex == WeaponIndexItemFilters /* Weapons */)
            {
                UI.Space(40f);
                UI.Label("Weapon Tag".Bold(), UI.Width(100f));
            }

            UI.Space(40f);
            UI.Label("Item Tag".Bold(), UI.Width(100f));

            UI.Space(40f);
            UI.Label(Gui.Localize("ModUi/&ItemsHelp2"));
        }

        using (UI.HorizontalScope(UI.Width(800f), UI.Height(400)))
        {
            var intValue = CurrentItemsFilterIndex;
            if (UI.SelectionGrid(
                    ref intValue,
                    ItemsFiltersLabels,
                    ItemsFiltersLabels.Length,
                    1, UI.Width(140f)))
            {
                CurrentItemsFilterIndex = intValue;

                if (CurrentItemsFilterIndex != WeaponIndexItemFilters /* Weapons */)
                {
                    CurrentItemsWeaponTagsFilterIndex = AllTitleIndexWeaponTagsFilters;
                }
            }

            if (CurrentItemsFilterIndex == WeaponIndexItemFilters /* Weapons */)
            {
                intValue = CurrentItemsWeaponTagsFilterIndex;
                if (UI.SelectionGrid(
                        ref intValue,
                        ItemsWeaponTagsFiltersLabels,
                        ItemsWeaponTagsFiltersLabels.Length,
                        1, UI.Width(140f)))
                {
                    CurrentItemsWeaponTagsFilterIndex = intValue;
                }
            }

            intValue = CurrentItemsItemTagsFilterIndex;
            if (UI.SelectionGrid(
                    ref intValue,
                    ItemsItemTagsFiltersLabels,
                    ItemsItemTagsFiltersLabels.Length,
                    1, UI.Width(140f)))
            {
                CurrentItemsItemTagsFilterIndex = intValue;
            }

            DisplayItemsBox();
        }
    }

    private static void AddUIForWeaponKey([NotNull] string key)
    {
        using (UI.HorizontalScope(UI.AutoWidth()))
        {
            UI.ActionButton(CraftingContext.RecipeTitles[key], () => CraftingContext.LearnRecipes(key),
                UI.Width(180f));
            UI.Space(20f);

            var toggle = Main.Settings.CraftingInStore.Contains(key);
            if (UI.Toggle(Gui.Localize("ModUi/&AddToStore"), ref toggle, UI.Width(125f)))
            {
                if (toggle)
                {
                    Main.Settings.CraftingInStore.Add(key);
                }
                else
                {
                    Main.Settings.CraftingInStore.Remove(key);
                }

                CraftingContext.AddToStore(key);
            }

            toggle = Main.Settings.CraftingRecipesInDm.Contains(key);
            if (UI.Toggle(Gui.Localize("ModUi/&RecipesInDm"), ref toggle, UI.Width(125f)))
            {
                if (toggle)
                {
                    Main.Settings.CraftingRecipesInDm.Add(key);
                }
                else
                {
                    Main.Settings.CraftingRecipesInDm.Remove(key);
                }

                CraftingContext.UpdateCraftingRecipesInDmState(key);
            }

            if (!CraftingContext.BaseGameItemsCategories.Contains(key))
            {
                toggle = Main.Settings.CraftingItemsInDm.Contains(key);

                if (!UI.Toggle(Gui.Localize("ModUi/&ItemInDm"), ref toggle, UI.Width(125f)))
                {
                    return;
                }

                if (toggle)
                {
                    Main.Settings.CraftingItemsInDm.Add(key);
                }
                else
                {
                    Main.Settings.CraftingItemsInDm.Remove(key);
                }

                CraftingContext.UpdateCraftingItemsInDmState(key);
            }
            else
            {
                UI.Space(128f);
            }
        }
    }

    private static void DisplayItemsBox()
    {
        var service = ServiceRepository.GetService<IGamingPlatformService>();
        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var rulesetItemFactoryService = ServiceRepository.GetService<IRulesetItemFactoryService>();
        var characterName = characterInspectionScreen.InspectedCharacter.Name;

        var items = DatabaseRepository.GetDatabase<ItemDefinition>()
            .Where(x => !x.guiPresentation.Hidden)
            .Where(x => ItemsFilters[CurrentItemsFilterIndex].Item2(x))
            .Where(x => ItemsItemTagsFilters[CurrentItemsItemTagsFilterIndex].Item2(x))
            .Where(x => ItemsWeaponTagsFilters[CurrentItemsWeaponTagsFilterIndex].Item2(x))
            .Where(x => service.IsContentPackAvailable(x.ContentPack))
            .OrderBy(x => x.FormatTitle());

        using var scrollView =
            new GUILayout.ScrollViewScope(ItemPosition, UI.AutoWidth(), UI.AutoHeight());

        ItemPosition = scrollView.scrollPosition;

        foreach (var item in items)
        {
            using (UI.HorizontalScope())
            {
                UI.ActionButton("+".Bold().Red(), () =>
                    {
                        var rulesetItem = rulesetItemFactoryService.CreateStandardItem(item, true, characterName);

                        characterInspectionScreen.externalContainer.AddSubItem(rulesetItem);
                    },
                    UI.Width(30f));

                var label = item.GuiPresentation.Title.StartsWith("Equipment/&CraftingManual")
                    ? Gui.Format(item.GuiPresentation.Title,
                        item.DocumentDescription.RecipeDefinition.CraftedItem.FormatTitle())
                    : item.FormatTitle();

                UI.Label(label, UI.AutoWidth());
            }
        }
    }
}
