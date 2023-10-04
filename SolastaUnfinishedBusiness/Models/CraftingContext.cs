using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.ItemCrafting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class CraftingContext
{
    internal static readonly List<string> BaseGameItemsCategories = new()
    {
        "PrimedItems", "EnchantingIngredients", "RelicForgeries"
    };

    internal static readonly Dictionary<string, string> RecipeTitles = new()
    {
        { "BarbarianClothes", Gui.Localize("Equipment/&Barbarian_Clothes_Title") },
        { "Battleaxe", Gui.Localize("Equipment/&BattleaxeTypeTitle") },
        { "Breastplate", Gui.Localize("Equipment/&Armor_BreastplateTitle") },
        { "CEHalberd", Gui.Localize("Item/&CEHalberdTitle") },
        { "CEHandXbow", Gui.Localize("Item/&CEHandXbowTitle") },
        { "CELongMace", Gui.Localize("Item/&CELongMaceTitle") },
        { "CEPike", Gui.Localize("Item/&CEPikeTitle") },
        { "ChainMail", Gui.Localize("Equipment/&Armor_ChainMailTitle") },
        { "ChainShirt", Gui.Localize("Equipment/&Armor_ChainShirtTitle") },
        { "ClothesWizard", Gui.Localize("Armor_Adventuring_Wizard_OutfitTitle") },
        { "Club", Gui.Localize("Equipment/&ClubTypeTitle") },
        { "Dart", Gui.Localize("Equipment/&DartTypeTitle") },
        { "EnchantingIngredients", Gui.Localize("Tooltip/&IngredientsHeaderTitle") },
        { "Greataxe", Gui.Localize("Equipment/&GreataxeTypeTitle") },
        { "Greatsword", Gui.Localize("Equipment/&GreatswordTypeTitle") },
        { "HalfPlate", Gui.Localize("Equipment/&Armor_HalfPlateTitle") },
        { "Handaxe", Gui.Localize("Equipment/&HandaxeTypeTitle") },
        { "HeavyCrossbow", Gui.Localize("Equipment/&HeavyCrossbowTypeTitle") },
        { "HideArmor", Gui.Localize("Equipment/&Armor_HideTitle") },
        { "Javelin", Gui.Localize("Equipment/&JavelinTypeTitle") },
        { "Leather", Gui.Localize("Equipment/&Armor_LeatherTitle") },
        { "LeatherDruid", Gui.Localize("Equipment/&Druid_Leather_Title") },
        { "LightCrossbow", Gui.Localize("Equipment/&LightCrossbowTypeTitle") },
        { "Longbow", Gui.Localize("Equipment/&LongbowTypeTitle") },
        { "Maul", Gui.Localize("Equipment/&MaulTypeTitle") },
        { "MonkArmor", Gui.Localize("Equipment/&Monk_Armor_Title") },
        { "PaddedLeather", Gui.Localize("Equipment/&Armor_PaddedTitle") },
        { "Plate", Gui.Localize("Equipment/&Armor_PlateTitle") },
        { "PrimedItems", Gui.Localize("ModUi/&PrimedItems") },
        { "Quarterstaff", Gui.Localize("Equipment/&QuarterstaffTypeTitle") },
        { "Rapier", Gui.Localize("Equipment/&RapierTypeTitle") },
        { "RelicForgeries", Gui.Localize("ModUi/&RelicForgeries") },
        { "Ringmail", Gui.Localize("Equipment/&Armor_RingMailTitle") },
        { "ScaleMail", Gui.Localize("Equipment/&Armor_ScaleMailTitle") },
        { "Scimitar", Gui.Localize("Equipment/&ScimitarTypeTitle") },
        { "Shield_Wooden", Gui.Localize("Equipment/&Shield_Wooden_Title") },
        { "Shield", Gui.Localize("Equipment/&ShieldCategoryTitle") },
        { "Shortbow", Gui.Localize("Equipment/&ShortbowTypeTitle") },
        { "SorcererArmor", Gui.Localize("Equipment/&Armor_Sorcerer_Outfit_Title") },
        { "Spear", Gui.Localize("Equipment/&SpearTypeTitle") },
        { "StuddedLeather", Gui.Localize("Equipment/&Armor_StuddedLeatherTitle") },
        { "Warhammer", Gui.Localize("Equipment/&WarhammerTypeTitle") },
        { "Warlock_Armor", Gui.Localize("Equipment/&Armor_Warlock_Title") }
    };

    private static readonly List<string> ItemCategories = new()
    {
        "All",
        "Ammunition",
        "Armor",
        "UsableDevices",
        "Weapons"
    };

    internal static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; } = new();

    private static GuiDropdown FilterGuiDropdown { get; set; }

    internal static void Load()
    {
        ItemRecipeGenerationHelper.AddPrimingRecipes();
        ItemRecipeGenerationHelper.AddIngredientEnchanting();
        ItemRecipeGenerationHelper.AddFactionItems();
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(BashingWeaponsData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(BattleAxeData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(CrossbowData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(GreatAxeData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(GreatSwordData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(HalberdData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(HandaxeData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(HandCrossbowData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(LongbowData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(LongMaceData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(PikeData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(QuarterstaffData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(RapierData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(ScimitarData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(ShortbowData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(SpearData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(ThrowingWeaponData.Items);
        ItemRecipeGenerationHelper.AddRecipesFromItemCollection(ArmorAndShieldData.Items, true);

        foreach (var key in RecipeBooks.Keys)
        {
            UpdateCraftingItemsInDmState(key);
            UpdateCraftingRecipesInDmState(key);
        }

        LoadFilteringAndSorting();
    }

    internal static void UpdateRecipeCost()
    {
        foreach (var item in RecipeBooks.Values.SelectMany(items => items))
        {
            item.costs = new[] { 0, Main.Settings.RecipeCost, 0, 0, 0 };
        }
    }

    internal static void AddToStore(string key)
    {
        if (!Main.Settings.CraftingInStore.Contains(key))
        {
            return;
        }

        foreach (var item in RecipeBooks[key])
        {
            MerchantContext.AddItem(item, ShopItemType.ShopCrafting);
        }
    }

    internal static void UpdateCraftingItemsInDmState(string key)
    {
        if (BaseGameItemsCategories.Contains(key))
        {
            // Don't touch the in dungeon state of base game items.
            return;
        }

        var available = Main.Settings.CraftingItemsInDm.Contains(key);

        foreach (var recipeBookDefinition in RecipeBooks[key])
        {
            recipeBookDefinition.DocumentDescription.RecipeDefinition.CraftedItem.inDungeonEditor = available;
        }
    }

    internal static void UpdateCraftingRecipesInDmState([NotNull] string key)
    {
        var available = Main.Settings.CraftingRecipesInDm.Contains(key);

        foreach (var recipeBookDefinition in RecipeBooks[key])
        {
            recipeBookDefinition.inDungeonEditor = available;
        }
    }

    internal static void LearnRecipes(string key)
    {
        var gameLoreService = ServiceRepository.GetService<IGameLoreService>();

        if (gameLoreService == null)
        {
            return;
        }

        foreach (var recipeBookDefinition in RecipeBooks[key])
        {
            gameLoreService.LearnRecipe(recipeBookDefinition.DocumentDescription.RecipeDefinition, false);
        }
    }

    private static void LoadFilteringAndSorting()
    {
        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var craftingPanel = characterInspectionScreen.craftingPanel;
        // ReSharper disable once Unity.UnknownResource
        var dropdownPrefab = Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown");
        var filter = UnityEngine.Object.Instantiate(dropdownPrefab, craftingPanel.transform);
        var filterRect = filter.GetComponent<RectTransform>();

        FilterGuiDropdown = filter.GetComponent<GuiDropdown>();

        // adds the filter dropdown

        var filterOptions = new List<TMP_Dropdown.OptionData>();

        filter.name = "FilterDropdown";
        filter.transform.localPosition = new Vector3(95f, 415f, 0f);

        filterRect.sizeDelta = new Vector2(150f, 28f);

        FilterGuiDropdown.ClearOptions();
        FilterGuiDropdown.onValueChanged.AddListener(delegate { craftingPanel.Refresh(); });

        ItemCategories.ForEach(x => filterOptions.Add(new GuiDropdown.OptionDataAdvanced { text = Gui.Localize(x) }));

        FilterGuiDropdown.AddOptions(filterOptions);
        FilterGuiDropdown.template.sizeDelta = new Vector2(1f, 208f);
    }

    internal static void FilterRecipes(ref List<RecipeDefinition> knownRecipes)
    {
        switch (FilterGuiDropdown.value)
        {
            case 0: // all
                break;

            case 1: // ammunition
                knownRecipes = knownRecipes
                    .Where(x => x.CraftedItem.IsAmmunition)
                    .ToList();
                break;

            case 2: // armor
                knownRecipes = knownRecipes
                    .Where(x => x.CraftedItem.IsArmor)
                    .ToList();
                break;

            case 3: // usable devices
                knownRecipes = knownRecipes
                    .Where(x => x.CraftedItem.IsUsableDevice)
                    .ToList();
                break;

            case 4: // weapons
                knownRecipes = knownRecipes
                    .Where(x => x.CraftedItem.IsWeapon)
                    .ToList();
                break;
        }

        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var craftingPanel = characterInspectionScreen.craftingPanel;

        LayoutRebuilder.ForceRebuildLayoutImmediate(craftingPanel.craftingOptionLinesTable);
    }

    internal sealed class ItemCollection
    {
        internal List<(ItemDefinition item, ItemDefinition presentation)> BaseItems;
        internal List<Object> CustomSubFeatures;
        internal List<MagicItemDataHolder> MagicToCopy;
        internal List<ItemDefinition> PossiblePrimedItemsToReplace;

        internal readonly struct MagicItemDataHolder
        {
            internal readonly string Name;
            internal readonly ItemDefinition Item;
            internal readonly RecipeDefinition Recipe;

            internal MagicItemDataHolder(string name, ItemDefinition item, RecipeDefinition recipe)
            {
                Name = name;
                Item = item;
                Recipe = recipe;
            }
        }
    }
}
