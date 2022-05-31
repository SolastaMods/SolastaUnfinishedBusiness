using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
#if DEBUG
using System.Linq;
using System.Text;
#endif

namespace SolastaCommunityExpansion.Models;

internal static class ItemCraftingContext
{
    public static readonly List<string> BASE_GAME_ITEMS_CATEGORIES = new()
    {
        "PrimedItems", "EnchantingIngredients", "RelicForgeries"
    };

    public static readonly Dictionary<string, string> RecipeTitles = new()
    {
        {"PrimedItems", Gui.Format("ModUi/&PrimedItems")},
        {"EnchantingIngredients", Gui.Format("Tooltip/&IngredientsHeaderTitle")},
        {"RelicForgeries", Gui.Format("ModUi/&RelicForgeries")},
        {"LightCrossbow", Gui.Format("Equipment/&LightCrossbowTypeTitle")},
        {"HeavyCrossbow", Gui.Format("Equipment/&HeavyCrossbowTypeTitle")},
        {"Handaxe", Gui.Format("Equipment/&HandaxeTypeTitle")},
        {"Javelin", Gui.Format("Equipment/&JavelinTitle")},
        {"Dart", Gui.Format("Equipment/&DartTitle")},
        {"Club", Gui.Format("Equipment/&ClubTitle")},
        {"Maul", Gui.Format("Equipment/&MaulTypeTitle")},
        {"Warhammer", Gui.Format("Equipment/&WarhammerTypeTitle")},
        {"Quarterstaff",Gui.Format("Equipment/&MaulQuarterstaffTitle")},
        {"Rapier", Gui.Format("Equipment/&RapierTypeTitle")},
        {"Spear",Gui.Format("Equipment/&SpearTypeTitle")},
        {"Scimitar", Gui.Format("Equipment/&ScimitarTypeTitle")},
        {"Shield_Wooden", Gui.Format("Equipment/&Shield_Wooden_Title")},
        {"Shield",  Gui.Format("Equipment/&ShieldCategoryTitle")},
        {"HideArmor", Gui.Format("ModUi/&HiderArmor")},
        {"LeatherDruid", Gui.Format("Equipment/&Druid_Leather_Title")},
        {"StuddedLeather", Gui.Format("Equipment/&Armor_StuddedLeatherTitle")}
    };

    public static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; } = new();

    internal static void Load()
    {
        ItemRecipeGenerationHelper.StockItem(
            DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore,
            DatabaseHelper.ItemDefinitions.Maul);

        ItemRecipeGenerationHelper.AddPrimingRecipes();
        ItemRecipeGenerationHelper.AddIngredientEnchanting();
        ItemRecipeGenerationHelper.AddFactionItems();

        ItemRecipeGenerationHelper.AddRecipesForWeapons(CrossbowData.CrossbowItems);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(HandaxeData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(ThrowingWeaponData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(BashingWeaponsData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(QuarterstaffData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(SpearData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(ScimitarData.Items);
        ItemRecipeGenerationHelper.AddRecipesForWeapons(RapierData.Items);

        ItemRecipeGenerationHelper.AddRecipesForArmor(ArmorAndShieldData.Items);

        foreach (var key in RecipeBooks.Keys)
        {
            UpdateCraftingItemsInDMState(key);
            UpdateCraftingRecipesInDMState(key);
        }
    }

    internal static void UpdateRecipeCost()
    {
        foreach (var items in RecipeBooks.Values)
        {
            foreach (var item in items)
            {
                item.SetCosts(new[] {0, Main.Settings.RecipeCost, 0, 0, 0});
            }
        }
    }

    internal static void AddToStore(string key)
    {
        if (Main.Settings.CraftingInStore.Contains(key))
        {
            foreach (var item in RecipeBooks[key])
            {
                ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, item);
                ItemRecipeGenerationHelper.StockItem(
                    DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, item);
            }
        }
    }

    internal static void UpdateCraftingItemsInDMState(string key)
    {
        if (BASE_GAME_ITEMS_CATEGORIES.Contains(key))
        {
            // Don't touch the in dungeon state of base game items.
            return;
        }

        var available = Main.Settings.CraftingItemsInDM.Contains(key);
        foreach (var recipeBookDefinition in RecipeBooks[key])
        {
            recipeBookDefinition.DocumentDescription.RecipeDefinition.CraftedItem.SetInDungeonEditor(available);
        }
    }

    internal static void UpdateCraftingRecipesInDMState(string key)
    {
        var available = Main.Settings.CraftingRecipesInDM.Contains(key);
        foreach (var recipeBookDefinition in RecipeBooks[key])
        {
            recipeBookDefinition.SetInDungeonEditor(available);
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

#if DEBUG
    public static string GenerateItemsDescription()
    {
        var outString = new StringBuilder();

        foreach (var key in RecipeBooks.Keys)
        {
            outString.Append("\n[*][b]");
            outString.Append(RecipeTitles[key]);
            outString.Append("[/b]: ");

            var uniqueEntries = RecipeBooks[key]
                .Select(rb => rb.DocumentDescription.RecipeDefinition.FormatTitle())
                .Distinct();

            outString.Append(string.Join(", ", uniqueEntries));
        }

        return outString.ToString();
    }
#endif
}
