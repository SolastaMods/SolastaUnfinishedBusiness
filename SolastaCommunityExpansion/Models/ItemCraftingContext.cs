using SolastaCommunityExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
    internal static class ItemCraftingContext
    {
        public static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; private set; } = new Dictionary<string, List<ItemDefinition>>();

        public static readonly Dictionary<string, string> RecipeTitles = new Dictionary<string, string>
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

            ItemRecipeGenerationHelper.AddRecipesForArmor(ArmorAndShieldData.Items);

            foreach (string key in RecipeBooks.Keys)
            {
                UpdateItemsInDMState(key);
                UpdateRecipesInDMState(key);
            }
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

        internal static void UpdateItemsInDMState(string key)
        {
            bool available = Main.Settings.ItemsInDM.Contains(key);
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                recipeBookDefinition.DocumentDescription.RecipeDefinition.CraftedItem.SetInDungeonEditor(available);
            }
        }

        internal static void UpdateRecipesInDMState(string key)
        {
            bool available = Main.Settings.RecipesInDM.Contains(key);
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

        public static string GenerateItemsDescription()
        {
            string outString = "[heading]Craftable Items[/heading]";
            outString += "\n[list]";
            foreach (string key in RecipeBooks.Keys)
            {
                outString += "\n[*][b]" + RecipeTitles[key] + "[/b]: ";
                bool first = true;
                List<string> uniqueEntries = new List<string>();
                foreach (ItemDefinition item in RecipeBooks[key])
                {
                    string name = item.DocumentDescription.RecipeDefinition.GuiPresentation.Title;
                    if (!uniqueEntries.Contains(name))
                    {
                        uniqueEntries.Add(name);
                    }
                }
                foreach (string name in uniqueEntries)
                {
                    if (!first)
                    {
                        outString += ", ";
                    }
                    first = false;
                    outString += Gui.Format(name);
                }
            }
            outString += "\n[/list]";
            return outString;
        }
    }
}
