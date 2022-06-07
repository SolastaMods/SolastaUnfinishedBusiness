using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
#if DEBUG
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
        { "PrimedItems", Gui.Localize("ModUi/&PrimedItems") },
        { "EnchantingIngredients", Gui.Localize("Tooltip/&IngredientsHeaderTitle") },
        { "RelicForgeries", Gui.Localize("ModUi/&RelicForgeries") },
        { "LightCrossbow", Gui.Localize("Equipment/&LightCrossbowTypeTitle") },
        { "HeavyCrossbow", Gui.Localize("Equipment/&HeavyCrossbowTypeTitle") },
        { "Handaxe", Gui.Localize("Equipment/&HandaxeTypeTitle") },
        { "Javelin", Gui.Localize("Equipment/&JavelinTypeTitle") },
        { "Dart", Gui.Localize("Equipment/&DartTypeTitle") },
        { "Club", Gui.Localize("Equipment/&ClubTypeTitle") },
        { "Maul", Gui.Localize("Equipment/&MaulTypeTitle") },
        { "Warhammer", Gui.Localize("Equipment/&WarhammerTypeTitle") },
        { "Quarterstaff", Gui.Localize("Equipment/&QuarterstaffTypeTitle") },
        { "Rapier", Gui.Localize("Equipment/&RapierTypeTitle") },
        { "Spear", Gui.Localize("Equipment/&SpearTypeTitle") },
        { "Scimitar", Gui.Localize("Equipment/&ScimitarTypeTitle") },
        { "Shield_Wooden", Gui.Localize("Equipment/&Shield_Wooden_Title") },
        { "Shield", Gui.Localize("Equipment/&ShieldCategoryTitle") },
        { "HideArmor", Gui.Localize("ModUi/&HiderArmor") },
        { "LeatherDruid", Gui.Localize("Equipment/&Druid_Leather_Title") },
        { "StuddedLeather", Gui.Localize("Equipment/&Armor_StuddedLeatherTitle") }
    };

    public static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; } = new();

    internal static void Load()
    {
        foreach (var merchant in MerchantTypeContext.MerchantTypes
                     .Where(x => x.Item2.IsDocument))
        {
            ItemRecipeGenerationHelper.StockItem(merchant.Item1, DatabaseHelper.ItemDefinitions.Maul);
        }

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
                item.SetCosts(new[] { 0, Main.Settings.RecipeCost, 0, 0, 0 });
            }
        }
    }

    internal static void AddToStore(string key)
    {
        if (Main.Settings.CraftingInStore.Contains(key))
        {
            foreach (var item in RecipeBooks[key])
            {
                foreach (var merchant in MerchantTypeContext.MerchantTypes
                             .Where(x => x.Item2.IsDocument))
                {
                    ItemRecipeGenerationHelper.StockItem(merchant.Item1, item);
                }
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
