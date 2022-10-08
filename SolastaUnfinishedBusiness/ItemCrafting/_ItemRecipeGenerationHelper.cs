using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ItemRecipeGenerationHelper
{
    internal static void AddRecipesForArmor(ItemCollection itemCollection)
    {
        foreach (var baseItem in itemCollection.BaseWeapons)
        {
            foreach (var itemData in itemCollection.MagicToCopy)
            {
                // Generate new items
                var newItem = ItemBuilder.BuildNewMagicArmor(
                    baseItem, itemData.Name, itemData.Item);
                var recipeName = "RecipeEnchanting" + newItem.Name;
                var builder = RecipeDefinitionBuilder.Create(recipeName);
                
                builder.AddIngredient(baseItem);
                
                foreach (var ingredient in itemData.Recipe.Ingredients.Where(ingredient =>
                             !itemCollection.PossiblePrimedItemsToReplace.Contains(ingredient.ItemDefinition)))
                {
                    builder.AddIngredient(ingredient);
                }

                builder.SetCraftedItem(newItem, itemCollection.NumProduced);
                builder.SetCraftingCheckData(itemData.Recipe.CraftingHours, itemData.Recipe.CraftingDC,
                    itemData.Recipe.ToolType);
                builder.SetGuiPresentation(newItem.GuiPresentation);
                
                var newRecipe = builder.AddToDB();
                var craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(
                    DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_EmpressGarb,
                    "CraftingManual_" + newRecipe.Name,
                    newRecipe, Main.Settings.RecipeCost,
                    DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_EmpressGarb.GuiPresentation);

                if (!RecipeBooks.ContainsKey(baseItem.Name))
                {
                    RecipeBooks.Add(baseItem.Name, new List<ItemDefinition>());
                }

                RecipeBooks[baseItem.Name].Add(craftingManual);

                if (!Main.Settings.CraftingInStore.Contains(baseItem.Name))
                {
                    continue;
                }

                foreach (var merchant in MerchantTypeContext.MerchantTypes
                             .Where(x => x.Item2.IsDocument))
                {
                    StockItem(merchant.Item1, craftingManual);
                }
            }
        }
    }

    internal static void AddRecipesForWeapons(ItemCollection itemCollection)
    {
        foreach (var baseItem in itemCollection.BaseWeapons)
        {
            foreach (var itemData in itemCollection.MagicToCopy)
            {
                // Generate new items
                var newItem = ItemBuilder.BuildNewMagicWeapon(baseItem, itemData.Name,
                    itemData.Item);
                var recipeName = "RecipeEnchanting" + newItem.Name;
                var builder = RecipeDefinitionBuilder.Create(recipeName);
                
                builder.AddIngredient(baseItem);
                
                foreach (var ingredient in itemData.Recipe.Ingredients.Where(ingredient =>
                             !itemCollection.PossiblePrimedItemsToReplace.Contains(ingredient.ItemDefinition)))
                {
                    builder.AddIngredient(ingredient);
                }

                builder.SetCraftedItem(newItem, itemCollection.NumProduced);
                builder.SetCraftingCheckData(itemData.Recipe.CraftingHours, itemData.Recipe.CraftingDC,
                    itemData.Recipe.ToolType);
                builder.SetGuiPresentation(newItem.GuiPresentation);
                
                var newRecipe = builder.AddToDB();
                var craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(
                    DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longbow_Of_Accuracy,
                    "CraftingManual_" + newRecipe.Name,
                    newRecipe, Main.Settings.RecipeCost,
                    DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation);

                if (!RecipeBooks.ContainsKey(baseItem.Name))
                {
                    RecipeBooks.Add(baseItem.Name, new List<ItemDefinition>());
                }

                RecipeBooks[baseItem.Name].Add(craftingManual);

                if (!Main.Settings.CraftingInStore.Contains(baseItem.Name))
                {
                    continue;
                }

                foreach (var merchant in MerchantTypeContext.MerchantTypes
                             .Where(x => x.Item2.IsDocument))
                {
                    StockItem(merchant.Item1, craftingManual);
                }
            }
        }
    }

    internal static void StockItem(MerchantDefinition merchant, ItemDefinition item)
    {
        var stockUnit = new StockUnitDescription
        {
            itemDefinition = item,
            initialAmount = 1,
            initialized = true,
            maxAmount = 2,
            minAmount = 1,
            stackCount = 1,
            reassortAmount = 1,
            reassortRateValue = 1,
            reassortRateType = RuleDefinitions.DurationType.Hour
        };
        
        merchant.StockUnitDescriptions.Add(stockUnit);
    }

    internal static void AddIngredientEnchanting()
    {
        var enchantedToIngredient = new Dictionary<ItemDefinition, ItemDefinition>
        {
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_MithralStone,
                DatabaseHelper.ItemDefinitions._300_GP_Opal
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Crystal_Of_Winter,
                DatabaseHelper.ItemDefinitions._100_GP_Pearl
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Gem, DatabaseHelper.ItemDefinitions._500_GP_Ruby
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Soul_Gem,
                DatabaseHelper.ItemDefinitions._1000_GP_Diamond
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Slavestone,
                DatabaseHelper.ItemDefinitions._100_GP_Emerald
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Cloud_Diamond,
                DatabaseHelper.ItemDefinitions._1000_GP_Diamond
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Stardust, DatabaseHelper.ItemDefinitions._100_GP_Pearl
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Doom_Gem,
                DatabaseHelper.ItemDefinitions._50_GP_Sapphire
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Fire,
                DatabaseHelper.ItemDefinitions._500_GP_Ruby
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Shard_Of_Ice,
                DatabaseHelper.ItemDefinitions._50_GP_Sapphire
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_LifeStone,
                DatabaseHelper.ItemDefinitions._1000_GP_Diamond
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Diamond_Of_Elai,
                DatabaseHelper.ItemDefinitions._100_GP_Emerald
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_PrimordialLavaStones,
                DatabaseHelper.ItemDefinitions._20_GP_Amethyst
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Blood_Of_Solasta,
                DatabaseHelper.ItemDefinitions.Ingredient_Acid
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Medusa_Coral,
                DatabaseHelper.ItemDefinitions._300_GP_Opal
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_PurpleAmber,
                DatabaseHelper.ItemDefinitions._50_GP_Sapphire
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_Heartstone,
                DatabaseHelper.ItemDefinitions._300_GP_Opal
            },
            {
                DatabaseHelper.ItemDefinitions.Ingredient_Enchant_SpiderQueen_Venom,
                DatabaseHelper.ItemDefinitions.Ingredient_BadlandsSpiderVenomGland
            }
        };
        var recipes = new List<RecipeDefinition>();

        foreach (var item in enchantedToIngredient.Keys)
        {
            var recipeName = "RecipeEnchanting" + item.Name;
            var builder = RecipeDefinitionBuilder.Create(recipeName);
            
            builder.AddIngredient(enchantedToIngredient[item]);
            builder.SetCraftedItem(item);
            builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
            builder.SetGuiPresentation(item.GuiPresentation);
            recipes.Add(builder.AddToDB());
        }

        const string GROUP_KEY = "EnchantingIngredients";
        
        RecipeBooks.Add(GROUP_KEY, new List<ItemDefinition>());

        foreach (var craftingManual in recipes.Select(recipe => ItemBuilder.BuilderCopyFromItemSetRecipe(
                     DatabaseHelper.ItemDefinitions.CraftingManualRemedy, "CraftingManual_" + recipe.Name,
                     recipe, Main.Settings.RecipeCost,
                     DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation)))
        {
            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            foreach (var merchant in MerchantTypeContext.MerchantTypes
                         .Where(x => x.Item2.IsDocument))
            {
                StockItem(merchant.Item1, craftingManual);
            }
        }
    }

    internal static void AddPrimingRecipes()
    {
        var primedToBase = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { DatabaseHelper.ItemDefinitions.Primed_Battleaxe, DatabaseHelper.ItemDefinitions.Battleaxe },
            { DatabaseHelper.ItemDefinitions.Primed_Breastplate, DatabaseHelper.ItemDefinitions.Breastplate },
            { DatabaseHelper.ItemDefinitions.Primed_ChainMail, DatabaseHelper.ItemDefinitions.ChainMail },
            { DatabaseHelper.ItemDefinitions.Primed_ChainShirt, DatabaseHelper.ItemDefinitions.ChainShirt },
            { DatabaseHelper.ItemDefinitions.Primed_Dagger, DatabaseHelper.ItemDefinitions.Dagger },
            { DatabaseHelper.ItemDefinitions.Primed_Greataxe, DatabaseHelper.ItemDefinitions.Greataxe },
            { DatabaseHelper.ItemDefinitions.Primed_Greatsword, DatabaseHelper.ItemDefinitions.Greatsword },
            { DatabaseHelper.ItemDefinitions.Primed_HalfPlate, DatabaseHelper.ItemDefinitions.HalfPlate },
            { DatabaseHelper.ItemDefinitions.Primed_HeavyCrossbow, DatabaseHelper.ItemDefinitions.HeavyCrossbow },
            { DatabaseHelper.ItemDefinitions.Primed_HideArmor, DatabaseHelper.ItemDefinitions.HideArmor },
            { DatabaseHelper.ItemDefinitions.Primed_LeatherDruid, DatabaseHelper.ItemDefinitions.LeatherDruid },
            { DatabaseHelper.ItemDefinitions.Primed_Leather_Armor, DatabaseHelper.ItemDefinitions.Leather },
            { DatabaseHelper.ItemDefinitions.Primed_LightCrossbow, DatabaseHelper.ItemDefinitions.LightCrossbow },
            { DatabaseHelper.ItemDefinitions.Primed_Longbow, DatabaseHelper.ItemDefinitions.Longbow },
            { DatabaseHelper.ItemDefinitions.Primed_Longsword, DatabaseHelper.ItemDefinitions.Longsword },
            { DatabaseHelper.ItemDefinitions.Primed_Mace, DatabaseHelper.ItemDefinitions.Mace },
            { DatabaseHelper.ItemDefinitions.Primed_Maul, DatabaseHelper.ItemDefinitions.Maul },
            { DatabaseHelper.ItemDefinitions.Primed_Morningstar, DatabaseHelper.ItemDefinitions.Morningstar },
            { DatabaseHelper.ItemDefinitions.Primed_Plate, DatabaseHelper.ItemDefinitions.Plate },
            { DatabaseHelper.ItemDefinitions.Primed_Rapier, DatabaseHelper.ItemDefinitions.Rapier },
            { DatabaseHelper.ItemDefinitions.Primed_ScaleMail, DatabaseHelper.ItemDefinitions.ScaleMail },
            { DatabaseHelper.ItemDefinitions.Primed_Scimitar, DatabaseHelper.ItemDefinitions.Scimitar },
            { DatabaseHelper.ItemDefinitions.Primed_Shortbow, DatabaseHelper.ItemDefinitions.Shortbow },
            { DatabaseHelper.ItemDefinitions.Primed_Shortsword, DatabaseHelper.ItemDefinitions.Shortsword },
            { DatabaseHelper.ItemDefinitions.Primed_Spear, DatabaseHelper.ItemDefinitions.Spear },
            { DatabaseHelper.ItemDefinitions.Primed_StuddedLeather, DatabaseHelper.ItemDefinitions.StuddedLeather },
            { DatabaseHelper.ItemDefinitions.Primed_Warhammer, DatabaseHelper.ItemDefinitions.Warhammer }
        };
        var recipes = primedToBase.Keys.Select(item => CreatePrimingRecipe(primedToBase[item], item));

        const string GROUP_KEY = "PrimedItems";
        
        RecipeBooks.Add(GROUP_KEY, new List<ItemDefinition>());

        foreach (var recipe in recipes)
        {
            var craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(
                DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longsword_Warden,
                "CraftingManual_" + recipe.Name,
                recipe, Main.Settings.RecipeCost,
                DatabaseHelper.ItemDefinitions.CraftingManual_Enchant_Longsword_Warden.GuiPresentation);

            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            foreach (var merchant in MerchantTypeContext.MerchantTypes
                         .Where(x => x.Item2.IsDocument))
            {
                StockItem(merchant.Item1, craftingManual);
            }
        }
    }

    internal static void AddFactionItems()
    {
        var forgeryToIngredient = new Dictionary<ItemDefinition, ItemDefinition>
        {
            {
                DatabaseHelper.ItemDefinitions.CAERLEM_TirmarianHolySymbol,
                DatabaseHelper.ItemDefinitions.Art_Item_50_GP_JadePendant
            },
            {
                DatabaseHelper.ItemDefinitions.BONEKEEP_MagicRune,
                DatabaseHelper.ItemDefinitions.Art_Item_25_GP_EngraveBoneDice
            },
            {
                DatabaseHelper.ItemDefinitions.CaerLem_Gate_Plaque,
                DatabaseHelper.ItemDefinitions.Art_Item_25_GP_SilverChalice
            }
        };
        var recipes = new List<RecipeDefinition>();

        foreach (var item in forgeryToIngredient.Keys)
        {
            var recipeName = "RecipeForgery" + item.Name;
            var builder = RecipeDefinitionBuilder.Create(recipeName);
            
            builder.AddIngredient(forgeryToIngredient[item]);
            builder.SetCraftedItem(item);
            builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType);
            builder.SetGuiPresentation(item.GuiPresentation);
            recipes.Add(builder.AddToDB());
        }

        var scrollForgeries = new Dictionary<ItemDefinition, ItemDefinition>
        {
            {
                DatabaseHelper.ItemDefinitions.BONEKEEP_AkshasJournal,
                DatabaseHelper.ItemDefinitions.Ingredient_AngryViolet
            },
            {
                DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_Manifest,
                DatabaseHelper.ItemDefinitions.Ingredient_ManacalonOrchid
            },
            {
                DatabaseHelper.ItemDefinitions.ABJURATION_MastersmithLoreDocument,
                DatabaseHelper.ItemDefinitions.Ingredient_RefinedOil
            },
            {
                DatabaseHelper.ItemDefinitions.CAERLEM_Inquisitor_Document,
                DatabaseHelper.ItemDefinitions.Ingredient_AbyssMoss
            },
            {
                DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_Poem,
                DatabaseHelper.ItemDefinitions.Ingredient_LilyOfTheBadlands
            },
            {
                DatabaseHelper.ItemDefinitions.ABJURATION_TOWER_ElvenWars,
                DatabaseHelper.ItemDefinitions.Ingredient_BloodDaffodil
            },
            { DatabaseHelper.ItemDefinitions.CAERLEM_Daliat_Document, DatabaseHelper.ItemDefinitions.Ingredient_Skarn }
        };

        foreach (var item in scrollForgeries.Keys)
        {
            var recipeName = "RecipeForgery" + item.Name;
            var builder = RecipeDefinitionBuilder.Create(recipeName);
            
            builder.AddIngredient(scrollForgeries[item]);
            builder.SetCraftedItem(item);
            builder.SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ScrollKitType);
            builder.SetGuiPresentation(item.GuiPresentation);
            recipes.Add(builder.AddToDB());
        }

        const string GROUP_KEY = "RelicForgeries";

        RecipeBooks.Add(GROUP_KEY, new List<ItemDefinition>());

        foreach (var craftingManual in recipes.Select(recipe => ItemBuilder.BuilderCopyFromItemSetRecipe(
                     DatabaseHelper.ItemDefinitions.CraftingManualRemedy, "CraftingManual_" + recipe.Name,
                     recipe, Main.Settings.RecipeCost,
                     DatabaseHelper.ItemDefinitions.CraftingManualRemedy.GuiPresentation)))
        {
            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            foreach (var merchant in MerchantTypeContext.MerchantTypes
                         .Where(x => x.Item2.IsDocument))
            {
                StockItem(merchant.Item1, craftingManual);
            }
        }
    }

    internal static RecipeDefinition CreatePrimingRecipe(ItemDefinition baseItem,
        ItemDefinition primed)
    {
        var recipeName = "RecipePriming" + baseItem.Name;
        var builder = RecipeDefinitionBuilder.Create(recipeName);
        
        builder.AddIngredient(baseItem);
        builder.SetCraftedItem(primed);
        builder.SetCraftingCheckData(8, 15, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
        builder.SetGuiPresentation(primed.GuiPresentation);
        
        return builder.AddToDB();
    }
}
