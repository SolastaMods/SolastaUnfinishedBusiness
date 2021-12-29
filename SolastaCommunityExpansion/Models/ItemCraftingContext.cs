using SolastaCommunityExpansion.ItemCrafting;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class ItemCraftingContext
    {
        public static Dictionary<string, List<ItemDefinition>> RecipeBooks { get; } = new Dictionary<string, List<ItemDefinition>>();

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
                    item.SetCosts(new int[] { 0, Main.Settings.RecipeCost, 0, 0, 0 });
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
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                var craftedItem = recipeBookDefinition.DocumentDescription.RecipeDefinition.CraftedItem;
                var factionRelicDescription = new FactionRelicDescription();

                craftedItem.SetFactionRelicDescription(factionRelicDescription);
                craftedItem.SetInDungeonEditor(true);
                craftedItem.GuiPresentation.SetHidden(!Main.Settings.CraftingItemsInDM.Contains(key));
            }
        }

        internal static void UpdateCraftingRecipesInDMState(string key)
        {
            foreach (ItemDefinition recipeBookDefinition in RecipeBooks[key])
            {
                var factionRelicDescription = new FactionRelicDescription();

                recipeBookDefinition.SetFactionRelicDescription(factionRelicDescription);
                recipeBookDefinition.SetInDungeonEditor(true);
                recipeBookDefinition.GuiPresentation.SetHidden(!Main.Settings.CraftingRecipesInDM.Contains(key));
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
            var outString = new StringBuilder("[heading]Craftable Items[/heading]");

            outString.Append("\n[list]");

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

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
