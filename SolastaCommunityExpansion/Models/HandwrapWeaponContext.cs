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

    public static void Load(List<(ItemDefinition, FactionStatusDefinition)> generic, List<(ItemDefinition, FactionStatusDefinition)> magic, List<(ItemDefinition, FactionStatusDefinition)> manuals)
    {
        HandwrapsPlus1 = BuildHandwraps("Handwraps+1", 400, true, Uncommon, WeaponPlus1);
        HandwrapsPlus2 = BuildHandwraps("Handwraps+2", 1500, true, Rare, WeaponPlus2);
        
        magic.Add((HandwrapsPlus1, FactionStatusDefinitions.Alliance));
        magic.Add((HandwrapsPlus2, FactionStatusDefinitions.Brotherhood));


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
        MakeRecipes(manuals);
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

    private static void MakeRecipes(List<(ItemDefinition, FactionStatusDefinition)> CraftingManuals)
    {
        CraftingManuals.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsPlus1, 24, 10, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), Monk.GUID), FactionStatusDefinitions.Alliance));

        CraftingManuals.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsPlus2, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), Monk.GUID), FactionStatusDefinitions.Alliance));

        CraftingManuals.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsOfForce, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem), Monk.GUID), FactionStatusDefinitions.Alliance));

        CraftingManuals.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsOfPulling, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Slavestone), Monk.GUID), FactionStatusDefinitions.Alliance));
    }
}
