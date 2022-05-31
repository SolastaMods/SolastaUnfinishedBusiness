using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Monk;
using SolastaModApi.Extensions;
using static RuleDefinitions.ItemRarity;
using static SolastaCommunityExpansion.Models.ItemPropertyDescriptions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models;

public static class HandwrapWeaponContext
{
    public static ItemDefinition HandwrapsPlus1, HandwrapsPlus2, HandwrapsOfForce, HandwrapsOfPulling;

    public static void Load(List<(ItemDefinition, ShopItemType)> shopItems)
    {
        HandwrapsPlus1 = BuildHandwraps("Handwraps+1", 400, true, true, Uncommon, WeaponPlus1);
        HandwrapsPlus2 = BuildHandwraps("Handwraps+2", 1500, true, true, Rare, WeaponPlus2);

        shopItems.Add((HandwrapsPlus1, CustomWeapons.ShopMeleePlus1));
        shopItems.Add((HandwrapsPlus2, CustomWeapons.ShopMeleePlus2));


        HandwrapsOfForce = BuildHandwraps("HandwrapsOfForce", 2000, true, false, Rare, ForceImpactVFX, WeaponPlus1AttackOnly);
        HandwrapsOfForce.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D4,
                damageType: RuleDefinitions.DamageTypeForce)
            .Build());


        HandwrapsOfPulling = BuildHandwraps("HandwrapsOfPulling", 2000, true, false, Rare, WeaponPlus1AttackOnly);
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

        // HandwrapsPlus1.SetInDungeonEditor(true);
        MakeRecipes(shopItems);
    }

    private static ItemDefinition BuildHandwraps(string name, int goldCost, bool noDescription, bool needId,
        RuleDefinitions.ItemRarity rarity,
        params ItemPropertyDescription[] properties)
    {
        return CustomWeapons.BuildWeapon(
            name,
            ItemDefinitions.UnarmedStrikeBase,
            goldCost,
            noDescription, rarity, needId: needId,
            properties:properties
        );
    }

    private static void MakeRecipes(List<(ItemDefinition, ShopItemType)> shopItems)
    {
        shopItems.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsPlus1, 24, 10, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), Monk.GUID), CustomWeapons.ShopCrafting));

        shopItems.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsPlus2, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), Monk.GUID), CustomWeapons.ShopCrafting));

        shopItems.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsOfForce, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem), Monk.GUID), CustomWeapons.ShopCrafting));

        shopItems.Add((CustomWeapons.BuildManual(CustomWeapons.BuildRecipe(HandwrapsOfPulling, 48, 16, Monk.GUID,
            ItemDefinitions.Ingredient_Enchant_Slavestone), Monk.GUID), CustomWeapons.ShopCrafting));
    }
}
