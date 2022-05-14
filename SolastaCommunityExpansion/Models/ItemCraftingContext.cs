using System.Collections.Generic;
using SolastaCommunityExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
#if DEBUG
using System.Linq;
using System.Text;
#endif

namespace SolastaCommunityExpansion.Models
{
    internal static class ItemCraftingContext
    {
        public static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; } = new Dictionary<string, List<ItemDefinition>>();

        public static readonly List<string> BASE_GAME_ITEMS_CATEGORIES = new()
        {
            "PrimedItems",
            "EnchantingIngredients",
            "RelicForgeries",
        };

        public static readonly Dictionary<string, string> RecipeTitles = new()
        {
            { "PrimedItems", "Primed Items" },
            { "EnchantingIngredients", "Enchanting Ingredients" },
            { "RelicForgeries", "Relic Forgeries" },
            { "LightCrossbow", "Light Crossbow" },
            { "HeavyCrossbow", "Heavy Crossbow" },
            { "Handaxe", "Handaxe" },
            { "Javelin", "Javelin" },
            { "Dart", "Dart" },
            { "Club", "Club" },
            { "Maul", "Maul" },
            { "Warhammer", "Warhammer" },
            { "Quarterstaff", "Quarterstaff" },
            { "Rapier", "Rapier" },
            { "Spear", "Spear" },
            { "Scimitar", "Scimitar" },
            { "Shield_Wooden", "Shield [Wooden]" },
            { "Shield", "Shield" },
            { "HideArmor", "Hide Armor" },
            { "StuddedLeather", "Studded Leather" },
        };

        internal static void Load()
        {
            ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, DatabaseHelper.ItemDefinitions.Maul);

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

            foreach (string key in RecipeBooks.Keys)
            {
                UpdateCraftingItemsInDMState(key);
                UpdateCraftingRecipesInDMState(key);
            }
        }

        internal static void UpdateRecipeCost()
        {
            foreach (List<ItemDefinition> items in RecipeBooks.Values)
            {
                foreach (ItemDefinition item in items)
                {
                    item.SetCosts(new[] { 0, Main.Settings.RecipeCost, 0, 0, 0 });
                }
            }
        }

        internal static void AddToStore(string key)
        {
            if (Main.Settings.CraftingInStore.Contains(key))
            {
                foreach (ItemDefinition item in RecipeBooks[key])
                {
                    ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, item);
                    ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, item);
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
            bool available = Main.Settings.CraftingItemsInDM.Contains(key);
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                recipeBookDefinition.DocumentDescription.RecipeDefinition.CraftedItem.SetInDungeonEditor(available);
            }
        }

        internal static void UpdateCraftingRecipesInDMState(string key)
        {
            bool available = Main.Settings.CraftingRecipesInDM.Contains(key);
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                recipeBookDefinition.SetInDungeonEditor(available);
            }
        }

        internal static void LearnRecipes(string key)
        {
            IGameLoreService gameLoreService = ServiceRepository.GetService<IGameLoreService>();
            if (gameLoreService == null)
            {
                return;
            }
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                gameLoreService.LearnRecipe(recipeBookDefinition.DocumentDescription.RecipeDefinition, false);
            }
        }

#if DEBUG
        public static string GenerateItemsDescription()
        {
            var outString = new StringBuilder();

            foreach (string key in RecipeBooks.Keys)
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
}
