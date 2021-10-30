using SolastaContentExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaContentExpansion.Models
{
    internal static class ItemCraftingContext
    {
        public static Dictionary<string, List<ItemDefinition>> RecipeBooks = new Dictionary<string, List<ItemDefinition>>();

        internal static void Load()
        {
            ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, DatabaseHelper.ItemDefinitions.Maul);

            ItemRecipeGenerationHelper.AddPrimingRecipes();
            ItemRecipeGenerationHelper.AddIngredientEnchanting();

            ItemRecipeGenerationHelper.AddRecipesForWeapons(CrossbowData.CrossbowItems);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(HandaxeData.Items);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(ThrowingWeaponData.Items);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(BashingWeaponsData.Items);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(QuarterstaffData.Items);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(SpearData.Items);
            ItemRecipeGenerationHelper.AddRecipesForWeapons(ScimitarData.Items);

            ItemRecipeGenerationHelper.AddRecipesForArmor(ArmorAndShieldData.Items);
        }

        internal static void UpdateRecipeCost()
        {
            foreach (List<ItemDefinition> items in RecipeBooks.Values)
            {
                foreach (ItemDefinition item in items)
                {
                    item.SetCosts(new int[] { 0, Main.Settings.RecipeCost, 0, 0, 0 });
                }
            }
        }

        internal static void AddToStore(string key)
        {
            if (Main.Settings.InStore.Contains(key))
            {
                foreach (ItemDefinition item in RecipeBooks[key])
                {
                    ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, item);
                    ItemRecipeGenerationHelper.StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, item);
                }
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
    }
}
