using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Monk;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;
using static RuleDefinitions.ItemRarity;
using static SolastaCommunityExpansion.Models.ItemPropertyDescriptions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models;

public static class HandwrapWeaponContext
{
    public static ItemDefinition HandwrapsPlus1, HandwrapsPlus2, HandwrapsOfForce, HandwrapsOfPulling;
    private static readonly List<ItemDefinition> CraftingManuals = new();

    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    public static void Load()
    {
        HandwrapsPlus1 = BuildHandwraps("Handwraps+1", 400, true, Uncommon, WeaponPlus1);
        HandwrapsPlus2 = BuildHandwraps("Handwraps+2", 1500, true, Rare, WeaponPlus2);


        HandwrapsOfForce = BuildHandwraps("HandwrapsOfForce", 2000, true, Rare, ForceImpactVFX, WeaponPlus1);
        HandwrapsOfForce.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D4,
                damageType: RuleDefinitions.DamageTypeForce)
            .Build());


        HandwrapsOfPulling = BuildHandwraps("HandwrapsOfPulling", 2000, true, Rare, WeaponPlus1);
        HandwrapsOfPulling.SetIsUsableDevice(true);
        HandwrapsOfPulling.SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
            .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
            .SetSaveDC(-1)
            .AddFunctions(new DeviceFunctionDescriptionBuilder()
                .SetPower(FeatureDefinitionPowerBuilder
                    .Create("PowerFunctionHandwrapsOfPulling", Monk.GUID)
                    .SetGuiPresentation(Category.Power)
                    .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
                    .SetUsesFixed(1)
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetEffectDescription(new EffectDescriptionBuilder()
                        .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 3,
                            RuleDefinitions.TargetType.Individuals)
                        .ExcludeCaster()
                        .SetSavingThrowData(
                            true,
                            true,
                            AttributeDefinitions.Strength,
                            false,
                            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            AttributeDefinitions.Wisdom)
                        .SetParticleEffectParameters(FeatureDefinitionPowers.PowerShadowTamerRopeGrapple
                            .EffectDescription.EffectParticleParameters)
                        .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                        .SetEffectForms(new EffectFormBuilder()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .Build())
                        .Build())
                    .AddToDB())
                .Build())
            .Build());


        // Works, but you need to click attack. Auto attacking till forc to go near target
        //TODO: investigate patch to make auto-attack respect reach, at least if already in range
        // HandwrapsPlus1.WeaponDescription.SetReachRange(5);

        // HandwrapsPlus1.SetInDungeonEditor(true);
        MakeRecipes();
        AddToShops();
        AddToLootTables();
    }

    private static ItemPresentation BuildHandwrapsPresentation(string unIdentifiedName)
    {
        var presentation = new ItemPresentation(ItemDefinitions.UnarmedStrikeBase.ItemPresentation);
        presentation.ItemFlags.Clear();
        presentation.SetAssetReference(new AssetReference());
        presentation.SetUnidentifiedTitle(GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item));
        presentation.SetUnidentifiedDescription(
            GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item));
        return presentation;
    }

    private static ItemDefinition BuildHandwraps(string name, int goldCost, bool noDescription,
        RuleDefinitions.ItemRarity rarity,
        params ItemPropertyDescription[] properties)
    {
        var unarmedStrikeBase = ItemDefinitions.UnarmedStrikeBase;
        var builder = ItemDefinitionBuilder
            .Create(name, Monk.GUID)
            .SetGold(goldCost)
            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
            .SetStaticProperties(properties)
            .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.ContainerSlot)
            .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot)
            .SetWeaponDescription(new WeaponDescription(unarmedStrikeBase.WeaponDescription))
            .SetItemPresentation(BuildHandwrapsPresentation($"{name}Unidentified"))
            .SetRequiresIdentification(true)
            .SetItemRarity(rarity)
            .MakeMagical();

        if (noDescription)
        {
            builder.SetGuiPresentation(Category.Item, Gui.NoLocalization,
                unarmedStrikeBase.GuiPresentation.SpriteReference);
        }
        else
        {
            builder.SetGuiPresentation(Category.Item, unarmedStrikeBase.GuiPresentation.SpriteReference);
        }

        return builder
            .AddToDB();
    }

    public static void MakeRecipes()
    {
        CraftingManuals.Add(BuildManual(BuildRecipe(HandwrapsPlus1, 24, 10,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness)));

        CraftingManuals.Add(BuildManual(BuildRecipe(HandwrapsPlus2, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem)));

        CraftingManuals.Add(BuildManual(BuildRecipe(HandwrapsOfForce, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem)));

        CraftingManuals.Add(BuildManual(BuildRecipe(HandwrapsOfPulling, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Slavestone)));
    }

    private static RecipeDefinition BuildRecipe(ItemDefinition item, int hours, int DC,
        params ItemDefinition[] ingredients)
    {
        return RecipeDefinitionBuilder
            .Create($"RecipeEnchant{item.Name}", Monk.GUID)
            .SetGuiPresentation(item.GuiPresentation.Title, Gui.NoLocalization)
            .SetCraftedItem(item)
            .SetCraftingCheckData(hours, DC, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(ingredients)
            .AddToDB();
    }

    private static ItemDefinition BuildManual(RecipeDefinition recipe)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        return ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}", Monk.GUID)
            .SetGuiPresentation(Category.Item, reference.GuiPresentation.SpriteReference)
            .SetItemPresentation(reference.ItemPresentation)
            .SetMerchantCategory(MerchantCategoryDefinitions.Crafting)
            .SetSlotTypes(SlotTypeDefinitions.ContainerSlot)
            .SetItemTags(TagsDefinitions.ItemTagStandard, TagsDefinitions.ItemTagPaper)
            .SetDocumentInformation(recipe, reference.DocumentDescription.ContentFragments)
            .SetGold(Main.Settings.RecipeCost)
            .AddToDB();
    }

    public static void AddToShops()
    {
        //Generic +1/+2 weapons
        var merchants = new[]
        {
            MerchantDefinitions.Store_Merchant_CircleOfDanantar_Joriel_Foxeye //Caer Cyflen
            //TODO: find magic weapon merchants in Lost Valley
        };

        foreach (var merchant in merchants)
        {
            StockItem(merchant, HandwrapsPlus1, FactionStatusDefinitions.Alliance);
            StockItem(merchant, HandwrapsPlus2, FactionStatusDefinitions.Brotherhood);
        }

        //Crafting manuals
        merchants = new[]
        {
            MerchantDefinitions.Store_Merchant_Circe //Manaclon Ruins
            //TODO: find crafting manuals merchants in Lost Valley
        };

        foreach (var merchant in merchants)
        {
            foreach (var manual in CraftingManuals)
            {
                StockItem(merchant, manual, FactionStatusDefinitions.Alliance);
            }
        }
    }

    private static void StockItem(MerchantDefinition merchant, ItemDefinition item, FactionStatusDefinition status)
    {
        merchant.StockUnitDescriptions.Add(StockBuilder
            .SetItem(item)
            .SetFaction(merchant.FactionAffinity, status.Name)
            .Build()
        );
    }

    private static StockUnitDescriptionBuilder BuildStockBuilder()
    {
        return new StockUnitDescriptionBuilder()
            .SetStock(initialAmount: 1)
            .SetRestock(1);
    }

    public static void AddToLootTables()
    {
        //Generic +1, weight 2-3
        //TreasureTableDefinitions.RandomTreasureTableC_MagicWeapons_01

        //Generic +2, weight 2-3
        //TreasureTableDefinitions.RandomTreasureTableD1_Weapons_02
    }
}
