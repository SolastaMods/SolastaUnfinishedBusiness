using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Models.CraftingContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ItemRecipeGenerationHelper
{
    internal static void AddRecipesFromItemCollection(ItemCollection itemCollection, bool isArmor = false)
    {
        var baseToPrimed = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { Battleaxe, Primed_Battleaxe },
            { Breastplate, Primed_Breastplate },
            { ChainMail, Primed_ChainMail },
            { ChainShirt, Primed_ChainShirt },
            { Dagger, Primed_Dagger },
            { Greataxe, Primed_Greataxe },
            { Greatsword, Primed_Greatsword },
            { HalfPlate, Primed_HalfPlate },
            { Handaxe, Primed_Handaxe },
            { HeavyCrossbow, Primed_HeavyCrossbow },
            { HideArmor, Primed_HideArmor },
            { Javelin, Primed_Javelin },
            { Leather, Primed_Leather_Armor },
            { LeatherDruid, Primed_LeatherDruid },
            { LightCrossbow, Primed_LightCrossbow },
            { Longbow, Primed_Longbow },
            { Longsword, Primed_Longsword },
            { Mace, Primed_Mace },
            { Maul, Primed_Maul },
            { Morningstar, Primed_Morningstar },
            { Plate, Primed_Plate },
            { Quarterstaff, Primed_Quarterstaff },
            { Rapier, Primed_Rapier },
            { ScaleMail, Primed_ScaleMail },
            { Scimitar, Primed_Scimitar },
            { Shield, Primed_Shield },
            { Shortbow, Primed_Shortbow },
            { Shortsword, Primed_Shortsword },
            { Spear, Primed_Spear },
            { StuddedLeather, Primed_StuddedLeather },
            { Warhammer, Primed_Warhammer },
            { CustomWeaponsContext.Halberd, CustomWeaponsContext.HalberdPrimed },
            { CustomWeaponsContext.Katana, CustomWeaponsContext.KatanaPrimed },
            { CustomWeaponsContext.Pike, CustomWeaponsContext.PikePrimed },
            { CustomWeaponsContext.LongMace, CustomWeaponsContext.LongMacePrimed },
            { CustomWeaponsContext.HandXbow, CustomWeaponsContext.HandXbowPrimed }
        };

        foreach (var baseConfig in itemCollection.BaseItems)
        {
            var baseItem = baseConfig.item;
            var presentation = baseConfig.presentation;

#pragma warning disable IDE0270
            if (!presentation)
#pragma warning restore IDE0270
            {
                presentation = baseItem;
            }

            foreach (var itemData in itemCollection.MagicToCopy)
            {
                // Generate new items
                var newItem = isArmor
                    ? ItemBuilder.BuildNewMagicArmor(baseItem, presentation, itemData.Name, itemData.Item)
                    : ItemBuilder.BuildNewMagicWeapon(baseItem, presentation, itemData.Name, itemData.Item);
                newItem.GuiPresentation.spriteReference = presentation.GuiPresentation.SpriteReference;
                var primedItem = baseToPrimed.TryGetValue(baseItem, out var value) ? value : baseItem;
                var ingredients = new List<ItemDefinition> { primedItem };

                if (itemCollection.CustomSubFeatures != null)
                {
                    newItem.AddCustomSubFeatures([.. itemCollection.CustomSubFeatures]);
                }

                ingredients.AddRange(itemData.Recipe.Ingredients
                    .Where(ingredient => !ingredient.ItemDefinition.IsArmor && !ingredient.ItemDefinition.IsWeapon)
                    .Select(x => x.ItemDefinition));

                var craftingManual = RecipeHelper.BuildRecipeManual(newItem, itemData.Recipe.CraftingHours,
                    itemData.Recipe.CraftingDC, [.. ingredients]);

                if (!RecipeBooks.TryGetValue(baseItem.Name, out var value1))
                {
                    value1 = [];
                    RecipeBooks.Add(baseItem.Name, value1);
                }

                value1.Add(craftingManual);

                if (!Main.Settings.CraftingInStore.Contains(baseItem.Name))
                {
                    continue;
                }

                MerchantContext.AddItem(craftingManual, ShopItemType.ShopCrafting);
            }
        }
    }

    internal static void AddIngredientEnchanting()
    {
        var enchantedToIngredient = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { Ingredient_Enchant_MithralStone, _300_GP_Opal },
            { Ingredient_Enchant_Crystal_Of_Winter, _100_GP_Pearl },
            { Ingredient_Enchant_Blood_Gem, _500_GP_Ruby },
            { Ingredient_Enchant_Soul_Gem, _1000_GP_Diamond },
            { Ingredient_Enchant_Slavestone, _100_GP_Emerald },
            { Ingredient_Enchant_Cloud_Diamond, _1000_GP_Diamond },
            { Ingredient_Enchant_Stardust, _100_GP_Pearl },
            { Ingredient_Enchant_Doom_Gem, _50_GP_Sapphire },
            { Ingredient_Enchant_Shard_Of_Fire, _500_GP_Ruby },
            { Ingredient_Enchant_Shard_Of_Ice, _50_GP_Sapphire },
            { Ingredient_Enchant_LifeStone, _1000_GP_Diamond },
            { Ingredient_Enchant_Diamond_Of_Elai, _100_GP_Emerald },
            { Ingredient_PrimordialLavaStones, _20_GP_Amethyst },
            { Ingredient_Enchant_Blood_Of_Solasta, Ingredient_Acid },
            { Ingredient_Enchant_Medusa_Coral, _300_GP_Opal },
            { Ingredient_Enchant_PurpleAmber, _50_GP_Sapphire },
            { Ingredient_Enchant_Heartstone, _300_GP_Opal },
            { Ingredient_Enchant_SpiderQueen_Venom, Ingredient_BadlandsSpiderVenomGland }
        };

        var recipes = enchantedToIngredient.Keys
            .Select(
                item => RecipeDefinitionBuilder
                    .Create("RecipeEnchant" + item.Name)
                    .SetGuiPresentation(item.GuiPresentation)
                    .AddIngredients(enchantedToIngredient[item])
                    .SetCraftedItem(item)
                    .SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.EnchantingToolType)
                    .AddToDB())
            .ToArray();

        const string GROUP_KEY = "EnchantingIngredients";

        RecipeBooks.Add(GROUP_KEY, []);

        foreach (var craftingManual in recipes
                     .Select(recipe => ItemBuilder.BuilderCopyFromItemSetRecipe(
                         CraftingManualRemedy,
                         "CraftingManual" + recipe.Name,
                         recipe,
                         Main.Settings.RecipeCost,
                         CraftingManualRemedy.GuiPresentation)))
        {
            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            MerchantContext.AddItem(craftingManual, ShopItemType.ShopCrafting);
        }
    }

    internal static void AddPrimingRecipes()
    {
        var primedToBase = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { Primed_Battleaxe, Battleaxe },
            { Primed_Breastplate, Breastplate },
            { Primed_ChainMail, ChainMail },
            { Primed_ChainShirt, ChainShirt },
            { Primed_Dagger, Dagger },
            { Primed_Greataxe, Greataxe },
            { Primed_Greatsword, Greatsword },
            { Primed_HalfPlate, HalfPlate },
            { Primed_HeavyCrossbow, HeavyCrossbow },
            { Primed_HideArmor, HideArmor },
            { Primed_LeatherDruid, LeatherDruid },
            { Primed_Leather_Armor, Leather },
            { Primed_LightCrossbow, LightCrossbow },
            { Primed_Longbow, Longbow },
            { Primed_Longsword, Longsword },
            { Primed_Mace, Mace },
            { Primed_Maul, Maul },
            { Primed_Morningstar, Morningstar },
            { Primed_Plate, Plate },
            { Primed_Rapier, Rapier },
            { Primed_ScaleMail, ScaleMail },
            { Primed_Scimitar, Scimitar },
            { Primed_Shortbow, Shortbow },
            { Primed_Shortsword, Shortsword },
            { Primed_Spear, Spear },
            { Primed_StuddedLeather, StuddedLeather },
            { Primed_Warhammer, Warhammer }
        };
        var recipes = primedToBase.Keys.Select(item => RecipeHelper.BuildPrimeRecipe(primedToBase[item], item));

        const string GROUP_KEY = "PrimedItems";

        RecipeBooks.Add(GROUP_KEY, []);

        foreach (var recipe in recipes)
        {
            var craftingManual = ItemBuilder.BuilderCopyFromItemSetRecipe(
                CraftingManual_Enchant_Longsword_Warden,
                "CraftingManual" + recipe.Name,
                recipe, Main.Settings.RecipeCost,
                CraftingManual_Enchant_Longsword_Warden.GuiPresentation);

            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            MerchantContext.AddItem(craftingManual, ShopItemType.ShopCrafting);
        }
    }

    internal static void AddFactionItems()
    {
        var forgeryToIngredient = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { CAERLEM_TirmarianHolySymbol, Art_Item_50_GP_JadePendant },
            { BONEKEEP_MagicRune, Art_Item_25_GP_EngraveBoneDice },
            { CaerLem_Gate_Plaque, Art_Item_25_GP_SilverChalice }
        };

        var scrollForgeries = new Dictionary<ItemDefinition, ItemDefinition>
        {
            { BONEKEEP_AkshasJournal, Ingredient_AngryViolet },
            { ABJURATION_TOWER_Manifest, Ingredient_ManacalonOrchid },
            { ABJURATION_MastersmithLoreDocument, Ingredient_RefinedOil },
            { CAERLEM_Inquisitor_Document, Ingredient_AbyssMoss },
            { ABJURATION_TOWER_Poem, Ingredient_LilyOfTheBadlands },
            { ABJURATION_TOWER_ElvenWars, Ingredient_BloodDaffodil },
            { CAERLEM_Daliat_Document, Ingredient_Skarn }
        };

        var recipes = forgeryToIngredient.Keys
            .Select(
                item => RecipeDefinitionBuilder
                    .Create("RecipeForgery" + item.Name)
                    .AddIngredients(forgeryToIngredient[item])
                    .SetCraftedItem(item)
                    .SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType)
                    .SetGuiPresentation(item.GuiPresentation)
                    .AddToDB())
            .Union(scrollForgeries.Keys
                .Select(
                    item => RecipeDefinitionBuilder
                        .Create("RecipeForgery" + item.Name)
                        .AddIngredients(scrollForgeries[item])
                        .SetCraftedItem(item)
                        .SetCraftingCheckData(16, 16, DatabaseHelper.ToolTypeDefinitions.ScrollKitType)
                        .SetGuiPresentation(item.GuiPresentation)
                        .AddToDB()))
            .ToArray();

        const string GROUP_KEY = "RelicForgeries";

        RecipeBooks.Add(GROUP_KEY, []);

        foreach (var craftingManual in recipes.Select(recipe => ItemBuilder.BuilderCopyFromItemSetRecipe(
                     CraftingManualRemedy, "CraftingManual" + recipe.Name,
                     recipe, Main.Settings.RecipeCost,
                     CraftingManualRemedy.GuiPresentation)))
        {
            RecipeBooks[GROUP_KEY].Add(craftingManual);

            if (!Main.Settings.CraftingInStore.Contains(GROUP_KEY))
            {
                continue;
            }

            MerchantContext.AddItem(craftingManual, ShopItemType.ShopCrafting);
        }
    }
}
