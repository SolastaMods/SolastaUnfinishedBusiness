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

    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    public static void Load()
    {
        HandwrapsPlus1 = BuildHandwraps("Handwraps+1", 400, true, Uncommon, WeaponPlus1);
        HandwrapsPlus2 = BuildHandwraps("Handwraps+2", 1500, true, Rare, WeaponPlus2);


        HandwrapsOfForce = BuildHandwraps("HandwrapsOfForce", 2000, true, Rare, WeaponPlus1);
        HandwrapsOfForce.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeForce)
            .Build());


        HandwrapsOfPulling = BuildHandwraps("HandwrapsOfPulling", 2000, true, Rare, WeaponPlus1);
        HandwrapsOfPulling.SetIsUsableDevice(true);
        HandwrapsOfPulling.SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
            .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
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

    public static void AddToShops()
    {
        //Generic +1/+2 weapons
        var merchants = new[]
        {
            MerchantDefinitions.Store_Merchant_CircleOfDanantar_Joriel_Foxeye, //Caer Cyflen
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
            MerchantDefinitions.Store_Merchant_Circe, //Manaclon Ruins
            //TODO: find crafting manuals merchants in Lost Valley
        };
        
        //TODO: add recipes to shops
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
            .SetStock()
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