using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Extensions;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.ItemCrafting
{
    internal static class ItemRecipeGenerationHelper
    {
        public static void AddRecipesForArmor(ItemCollection itemCollection)
        {
            foreach (ItemDefinition baseItem in itemCollection.BaseWeapons)
            {
                foreach (ItemCollection.MagicItemDataHolder itemData in itemCollection.MagicToCopy)
                {
                    // Generate new items
                    ItemDefinition newItem = ItemBuilder.BuildNewMagicArmor(itemCollection.BaseGuid, baseItem, itemData.Item, itemData.Name);
                    // Generate recipes for items
                    string recipeName = "RecipeEnchanting" + newItem.Name;
                    RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(itemCollection.BaseGuid, recipeName).ToString());
                    builder.AddIngredient(baseItem);
                    foreach (IngredientOccurenceDescription ingredient in itemData.Recipe.Ingredients)
                    {
                        if (itemCollection.PossiblePrimedItemsToReplace.Contains(ingredient.ItemDefinition))
                        {
                            continue;
                        }
                        builder.AddIngredient(ingredient);
                    }
                    builder.SetCraftedItem(newItem, itemCollection.NumProduced);
                    builder.SetCraftingCheckData(itemData.Recipe.CraftingHours, itemData.Recipe.CraftingDC, itemData.Recipe.ToolType);
                    builder.SetGuiPresentation(newItem.GuiPresentation);
                    RecipeDefinition newRecipe = builder.AddToDB();
                    // Stock item Recipes
                    ItemDefinition craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(itemCollection.BaseGuid, newRecipe, DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_EmpressGarb,
                    "CraftingManual_" + newRecipe.Name, DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_EmpressGarb.GuiPresentation, Main.Settings.RecipeCost);

                    if (!Models.ItemCraftingContext.RecipeBooks.ContainsKey(baseItem.Name))
                    {
                        Models.ItemCraftingContext.RecipeBooks.Add(baseItem.Name, new List<ItemDefinition>());
                    }
                    Models.ItemCraftingContext.RecipeBooks[baseItem.Name].Add(craftingManual);

                    if (Main.Settings.InStore.Contains(baseItem.Name))
                    {
                        StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, craftingManual);
                        StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftingManual);
                    }
                }

            }
        }

        public static void AddRecipesForWeapons(ItemCollection itemCollection)
        {
            foreach (ItemDefinition baseItem in itemCollection.BaseWeapons)
            {
                foreach (ItemCollection.MagicItemDataHolder itemData in itemCollection.MagicToCopy)
                {
                    // Generate new items
                    ItemDefinition newItem = ItemBuilder.BuildNewMagicWeapon(itemCollection.BaseGuid, baseItem, itemData.Item, itemData.Name);
                    // Generate recipes for items
                    string recipeName = "RecipeEnchanting" + newItem.Name;
                    RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(itemCollection.BaseGuid, recipeName).ToString());
                    builder.AddIngredient(baseItem);
                    foreach (IngredientOccurenceDescription ingredient in itemData.Recipe.Ingredients)
                    {
                        if (itemCollection.PossiblePrimedItemsToReplace.Contains(ingredient.ItemDefinition))
                        {
                            continue;
                        }
                        builder.AddIngredient(ingredient);
                    }
                    builder.SetCraftedItem(newItem, itemCollection.NumProduced);
                    builder.SetCraftingCheckData(itemData.Recipe.CraftingHours, itemData.Recipe.CraftingDC, itemData.Recipe.ToolType);
                    builder.SetGuiPresentation(newItem.GuiPresentation);
                    RecipeDefinition newRecipe = builder.AddToDB();
                    // Stock item Recipes
                    ItemDefinition craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(itemCollection.BaseGuid, newRecipe, DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longbow_Of_Accuracy,
                    "CraftingManual_" + newRecipe.Name, DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation, Main.Settings.RecipeCost);

                    if (!Models.ItemCraftingContext.RecipeBooks.ContainsKey(baseItem.Name))
                    {
                        Models.ItemCraftingContext.RecipeBooks.Add(baseItem.Name, new List<ItemDefinition>());
                    }
                    Models.ItemCraftingContext.RecipeBooks[baseItem.Name].Add(craftingManual);

                    if (Main.Settings.InStore.Contains(baseItem.Name))
                    {
                        StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, craftingManual);
                        StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftingManual);
                    }
                }
            }
        }

        public static void StockItem(MerchantDefinition merchant, ItemDefinition item)
        {
            StockUnitDescription stockUnit = new StockUnitDescription();
            stockUnit.SetItemDefinition(item);
            stockUnit.SetInitialAmount(1);
            stockUnit.SetInitialized(true);
            stockUnit.SetMaxAmount(2);
            stockUnit.SetMinAmount(1);
            stockUnit.SetStackCount(1);
            stockUnit.SetReassortAmount(1);
            stockUnit.SetReassortRateValue(1);
            stockUnit.SetReassortRateType(RuleDefinitions.DurationType.Hour);
            merchant.StockUnitDescriptions.Add(stockUnit);
        }

        public static void AddIngredientEnchanting()
        {
            Guid baseGuid = new Guid("80a5106e-5cb7-4fdd-8f96-b94f3aafd4dd");

            Dictionary<ItemDefinition, ItemDefinition> EnchantedToIngredient = new Dictionary<ItemDefinition, ItemDefinition>()
            {
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_MithralStone, DatabaseHelper.ItemDefinitions._300_GP_Opal },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Crystal_Of_Winter, DatabaseHelper.ItemDefinitions._100_GP_Pearl },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Gem, DatabaseHelper.ItemDefinitions._500_GP_Ruby },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Soul_Gem, DatabaseHelper.ItemDefinitions._1000_GP_Diamond},
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Slavestone, DatabaseHelper.ItemDefinitions._100_GP_Emerald },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Cloud_Diamond, DatabaseHelper.ItemDefinitions._1000_GP_Diamond },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Stardust, DatabaseHelper.ItemDefinitions._100_GP_Pearl },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Doom_Gem, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Fire, DatabaseHelper.ItemDefinitions._500_GP_Ruby },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Ice, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_LifeStone, DatabaseHelper.ItemDefinitions._1000_GP_Diamond },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Diamond_Of_Elai, DatabaseHelper.ItemDefinitions._100_GP_Emerald },
                {DatabaseHelper.ItemDefinitions.Ingredient_PrimordialLavaStones, DatabaseHelper.ItemDefinitions._20_GP_Amethyst },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Of_Solasta, DatabaseHelper.ItemDefinitions.Ingredient_Acid },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Medusa_Coral, DatabaseHelper.ItemDefinitions._300_GP_Opal },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_PurpleAmber, DatabaseHelper.ItemDefinitions._50_GP_Sapphire },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Heartstone, DatabaseHelper.ItemDefinitions._300_GP_Opal },
                {DatabaseHelper.ItemDefinitions.Ingredient_Enchant_SpiderQueen_Venom, DatabaseHelper.ItemDefinitions.Ingredient_BadlandsSpiderVenomGland },
            };
            List<RecipeDefinition> recipes = new List<RecipeDefinition>();
            foreach (ItemDefinition item in EnchantedToIngredient.Keys)
            {
                string recipeName = "RecipeEnchanting" + item.Name;
                RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(baseGuid, recipeName).ToString());
                builder.AddIngredient(EnchantedToIngredient[item]);
                builder.SetCraftedItem(item);
                builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
                builder.SetGuiPresentation(item.GuiPresentation);
                recipes.Add(builder.AddToDB());
            }

            string groupKey = "EnchantingIngredients";
            Models.ItemCraftingContext.RecipeBooks.Add(groupKey, new List<ItemDefinition>());

            foreach (RecipeDefinition recipe in recipes)
            {
                ItemDefinition craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(baseGuid, recipe, DatabaseHelper.ItemDefinitions.CraftingManualRemedy,
                    "CraftingManual_" + recipe.Name, DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation, Main.Settings.RecipeCost);

                Models.ItemCraftingContext.RecipeBooks[groupKey].Add(craftingManual);

                if (Main.Settings.InStore.Contains(groupKey))
                {
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, craftingManual);
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftingManual);
                }
            }
        }

        public static void AddPrimingRecipes()
        {
            ItemDefinition[] allItems = DatabaseRepository.GetDatabase<ItemDefinition>().GetAllElements();
            List<RecipeDefinition> recipes = new List<RecipeDefinition>();
            Guid baseGuid = new Guid("cad103e4-6226-4ba0-a9ed-2fee8886f6b9");
            foreach (ItemDefinition item in allItems)
            {
                if (item.ItemPresentation != null && item.ItemPresentation.ItemFlags != null &&
                    item.ItemPresentation.ItemFlags.Contains(DatabaseHelper.ItemFlagDefinitions.ItemFlagPrimed))
                {
                    recipes.Add(CreatePrimingRecipe(baseGuid, FindMatchingBase(allItems, item), item));
                }
            }

            string groupKey = "PrimedItems";
            Models.ItemCraftingContext.RecipeBooks.Add(groupKey, new List<ItemDefinition>());

            foreach (RecipeDefinition recipe in recipes)
            {
                ItemDefinition craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(baseGuid, recipe, DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longsword_Warden,
                    "CraftingManual_" + recipe.Name, DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longsword_Warden.GuiPresentation, Main.Settings.RecipeCost);

                Models.ItemCraftingContext.RecipeBooks[groupKey].Add(craftingManual);

                if (Main.Settings.InStore.Contains(groupKey))
                {
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, craftingManual);
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftingManual);
                }
            }
        }

        public static void AddFactionItems()
        {
            Guid baseGuid = new Guid("80a5106e-5cb7-4fdd-8f96-b94f3aafd4dd");

            Dictionary<ItemDefinition, ItemDefinition> ForgeryToIngredient = new Dictionary<ItemDefinition, ItemDefinition>()
            {
                {DatabaseHelper.ItemDefinitions.CAERLEM_TirmarianHolySymbol, DatabaseHelper.ItemDefinitions.Art_Item_50_GP_JadePendant},
                {DatabaseHelper.ItemDefinitions.BONEKEEP_MagicRune, DatabaseHelper.ItemDefinitions.Art_Item_25_GP_EngraveBoneDice},
                {DatabaseHelper.ItemDefinitions.CaerLem_Gate_Plaque, DatabaseHelper.ItemDefinitions.Art_Item_25_GP_SilverChalice},

            };
            List<RecipeDefinition> recipes = new List<RecipeDefinition>();
            foreach (ItemDefinition item in ForgeryToIngredient.Keys)
            {
                string recipeName = "RecipeForgery" + item.Name;
                RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(baseGuid, recipeName).ToString());
                builder.AddIngredient(ForgeryToIngredient[item]);
                builder.SetCraftedItem(item);
                builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType);
                builder.SetGuiPresentation(item.GuiPresentation);
                recipes.Add(builder.AddToDB());
            }

            Dictionary<ItemDefinition, ItemDefinition> ScrollForgeries = new Dictionary<ItemDefinition, ItemDefinition>()
            {
                {DatabaseHelper.ItemDefinitions.BONEKEEP_AkshasJournal, DatabaseHelper.ItemDefinitions.Ingredient_AngryViolet},
                {DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_Manifest, DatabaseHelper.ItemDefinitions.Ingredient_ManacalonOrchid},
                {DatabaseHelper.ItemDefinitions.ABJURATION_MastersmithLoreDocument, DatabaseHelper.ItemDefinitions.Ingredient_RefinedOil},
                {DatabaseHelper.ItemDefinitions.CAERLEM_Inquisitor_Document, DatabaseHelper.ItemDefinitions.Ingredient_AbyssMoss},
                {DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_Poem, DatabaseHelper.ItemDefinitions.Ingredient_LilyOfTheBadlands},
                {DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_ElvenWars, DatabaseHelper.ItemDefinitions.Ingredient_BloodDaffodil},
                {DatabaseHelper.ItemDefinitions.CAERLEM_Daliat_Document, DatabaseHelper.ItemDefinitions.Ingredient_Skarn},
            };
            foreach (ItemDefinition item in ScrollForgeries.Keys)
            {
                string recipeName = "RecipeForgery" + item.Name;
                RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(baseGuid, recipeName).ToString());
                builder.AddIngredient(ScrollForgeries[item]);
                builder.SetCraftedItem(item);
                builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ScrollKitType);
                builder.SetGuiPresentation(item.GuiPresentation);
                recipes.Add(builder.AddToDB());
            }

            string groupKey = "RelicForgeries";
            Models.ItemCraftingContext.RecipeBooks.Add(groupKey, new List<ItemDefinition>());

            foreach (RecipeDefinition recipe in recipes)
            {
                ItemDefinition craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(baseGuid, recipe, DatabaseHelper.ItemDefinitions.CraftingManualRemedy,
                    "CraftingManual_" + recipe.Name, DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation, Main.Settings.RecipeCost);

                Models.ItemCraftingContext.RecipeBooks[groupKey].Add(craftingManual);

                if (Main.Settings.InStore.Contains(groupKey))
                {
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Circe, craftingManual);
                    StockItem(DatabaseHelper.MerchantDefinitions.Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore, craftingManual);
                }
            }
        }

        private static ItemDefinition FindMatchingBase(ItemDefinition[] allItems, ItemDefinition primed)
        {
            ItemDefinition match = null;
            foreach (ItemDefinition item in allItems)
            {
                if (item.Magical ||
                    (item.ItemPresentation != null && item.ItemPresentation.ItemFlags != null && item.ItemPresentation.ItemFlags.Count > 0) ||
                    (item.ItemTags != null && (item.ItemTags.Contains("Quest") || !item.ItemTags.Contains("Standard"))) ||
                    item == DatabaseHelper.ItemDefinitions.ScaleMailCleric || item == DatabaseHelper.ItemDefinitions.Shortsword_Duel_Autoequip ||
                    item == DatabaseHelper.ItemDefinitions.ScaleMail_VigdisOnly || item == DatabaseHelper.ItemDefinitions.LeatherDruid)
                {
                    continue;
                }
                if (primed.IsWeapon && 
                    item.IsWeapon && 
                    primed.WeaponDescription.WeaponType == item.WeaponDescription.WeaponType)
                {
                    match = item;
                }
                if (primed.IsArmor && 
                    item.IsArmor && 
                    primed.ArmorDescription.ArmorType == item.ArmorDescription.ArmorType)
                {
                    match = item;
                }
            }
            return match;
        }

        private static RecipeDefinition CreatePrimingRecipe(Guid baseGuid, ItemDefinition baseItem, ItemDefinition primed)
        {
            string recipeName = "RecipePriming" + baseItem.Name;
            RecipeBuilder builder = new RecipeBuilder(recipeName, GuidHelper.Create(baseGuid, recipeName).ToString());
            builder.AddIngredient(baseItem);
            builder.SetCraftedItem(primed);
            builder.SetCraftingCheckData(8, 15, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
            builder.SetGuiPresentation(primed.GuiPresentation);
            return builder.AddToDB();
        }
    }
}
