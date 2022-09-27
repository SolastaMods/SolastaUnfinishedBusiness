#if false
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PatronAncientForest : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal PatronAncientForest()
    {
        Subclass = null;
    }

        public static CharacterSubclassDefinition Build()
    {
        var ancientForestSpelllist = SpellListDefinitionBuilder
            .Create(SpellListPaladin, "SpellListAncientForest")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Goodberry, Entangle)
            .SetSpellsAtLevel(2, ProtectionFromPoison, SpikeGrowth)
            .SetSpellsAtLevel(3, Revivify, StinkingCloud)
            .SetSpellsAtLevel(4, Blight, GiantInsect)
            .SetSpellsAtLevel(5, Contagion, InsectPlague)
            .FinalizeSpells()
            .AddToDB();

        // necrotic and healing
        var ancientForestExpandedSpellListAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("AncientForestExpandedSpelllistAfinity")
            .SetGuiPresentation(Category.Feature)
            .SetExtendedSpellList(ancientForestSpelllist)
            .AddToDB();

        const string lifeSapId = "AncientForestLifeSap";

        var lifeSapFeature = FeatureDefinitionOnMagicalAttackDamageEffectBuilder
            .Create(lifeSapId)
            .SetGuiPresentation(Category.Feature)
            .SetOnMagicalAttackDamageDelegates(null, (attacker, _, _, effect, _, _, _) =>
            {
                var caster = attacker.RulesetCharacter;

                if (caster.MissingHitPoints <= 0 ||
                    !effect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
                {
                    return;
                }

                var belowHalfHealth = caster.MissingHitPoints > caster.CurrentHitPoints;

                attacker.UsedSpecialFeatures.TryGetValue(lifeSapId, out var used);

                if (!belowHalfHealth && used != 0)
                {
                    return;
                }

                attacker.UsedSpecialFeatures[lifeSapId] = used + 1;

                var level = caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
                var healing = used == 0 && belowHalfHealth ? level : Mathf.CeilToInt(level / 2f);
                var cap = used == 0 ? HealingCap.MaximumHitPoints : HealingCap.HalfMaximumHitPoints;

                var ability = GuiPresentationBuilder.CreateTitleKey(lifeSapId, Category.Feature);
                GameConsoleHelper.LogCharacterActivatesAbility(caster, ability);
                RulesetCharacter.Heal(healing, caster, caster, cap, caster.Guid);
            })
            .AddToDB();

        var regrowth = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinLayOnHands, "AncientForestRegrowth")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealing.GuiPresentation.SpriteReference)
            .AddToDB();

        var AncientForestBonusCantrip = FeatureDefinitionBonusCantripsBuilder
            .Create("DHAncientForestBonusCantrip")
            .SetGuiPresentation(Category.Feature)
            .ClearBonusCantrips()
            .AddBonusCantrip(Shillelagh)
            .AddBonusCantrip(ChillTouch)
            .AddToDB();

        var herbalBrewPool = FeatureDefinitionPowerPoolBuilder
            .Create("DH_HerbalBrewPool")
            .SetGuiPresentation(new GuiPresentationBuilder(
                "Feature/&HerbalBrewFeatureSetTitle",
                "Feature/&HerbalBrewDescription",
                PotionRemedy.GuiPresentation.SpriteReference //TODO: find better icon
            ).Build())
            .SetUsesProficiency()
            .SetRechargeRate(RechargeRate.LongRest)
            .SetActivation(ActivationTime.Rest, 1)
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(herbalBrewPool, true,
            BuildHerbalBrew(herbalBrewPool, "Toxifying", Poison_Basic),
            BuildHerbalBrew(herbalBrewPool, "Healing", PotionOfHealing),
            BuildHerbalBrew(herbalBrewPool, DamageAffinityAcidResistance, PotionOfSpeed),
            BuildHerbalBrew(herbalBrewPool, DamageAffinityLightningResistance, Ingredient_RefinedOil),
            BuildHerbalBrew(herbalBrewPool, DamageAffinityNecroticResistance, PotionOfClimbing),
            BuildHerbalBrew(herbalBrewPool, DamageAffinityPoisonResistance, PotionOfHeroism),
            BuildHerbalBrew(herbalBrewPool, DamageAffinityRadiantResistance, PotionOfInvisibility)
        );

        RestActivityDefinitionBuilder
            .Create("AncientForestToxifyingRestActivity")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                PowersBundleContext.UseCustomRestPowerFunctorName,
                herbalBrewPool.name)
            .SetGuiPresentation(herbalBrewPool.GuiPresentation.Title, herbalBrewPool.GuiPresentation.Description)
            .AddToDB();

        var herbalBrewFeatureSet = FeatureDefinitionFeatureSetBuilder.Create(
                "HerbalBrewFeatureSet")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddFeatureSet(herbalBrewPool)
            .AddToDB();

        var photosynthesis = ConditionDefinitionBuilder
            .Create("AncientForestPhotosynthesis")
            .SetSilent(Silent.None)
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(DatabaseHelper.FeatureDefinitionRegenerations.RegenerationRing)
            .AddToDB();

        var ancientForestLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create("AncientForestLightAffinity")
            .SetGuiPresentation("AncientForestLightAffinity", Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Bright, condition = photosynthesis
            })
            .AddToDB();

        var atWillEntanglePower = FeatureDefinitionPowerBuilder
            .Create("AncientForestAtWillEntangle")
            .SetGuiPresentation(Entangle.GuiPresentation)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Charisma,
                Entangle.EffectDescription,
                true)
            .AddToDB();

        var rootedCondition = ConditionDefinitionBuilder
            .Create("AncientForestRootedCondition")
            .SetSilent(Silent.None)
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .AddFeatures(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
            .AddFeatures(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging)
            .AddFeatures(atWillEntanglePower)
            .AddToDB();

        var rootedPower = FeatureDefinitionPowerBuilder
            .Create("AncientForestRootedPower")
            .SetGuiPresentation(Category.Feature, PowerRangerHideInPlainSight.GuiPresentation.SpriteReference)
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
                    .AddEffectForm(
                        new EffectFormBuilder().SetConditionForm(
                            rootedCondition,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true,
                            new List<ConditionDefinition>()).Build())
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .Build()
                ,
                true)
            .AddToDB();

        var AncientForestWallofThornsPool = FeatureDefinitionPowerPoolBuilder
            .Create("DHAncientForestWallofThornsPool")
            .SetGuiPresentationNoContent()
            .SetUsesProficiency()
            .SetUsesAbility(1, AttributeDefinitions.Charisma)
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        var WallofThornsFeatureSet = FeatureDefinitionFeatureSetBuilder.Create(
                "WallofThornsFeatureSet")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .SetUniqueChoices(false)
            .AddToDB();

        var thornSpells = new List<SpellDefinition> {WallOfThornsWallLine, WallOfThornsWallRing};

        foreach (var spell in thornSpells)
        {
            FeatureDefinitionPower WallofThornsPower = new FeatureDefinitionPowerSharedPoolBuilder(
                "AncientForest" + spell.name,
                AncientForestWallofThornsPool,
                RechargeRate.LongRest,
                ActivationTime.Rest,
                1,
                false,
                false,
                AttributeDefinitions.Charisma,
                spell.EffectDescription,
                spell.GuiPresentation,
                false
            ).AddToDB();
            
            WallofThornsFeatureSet.FeatureSet.Add(WallofThornsPower);
        }
        // should Use features sets so character saves don't break

        var ancientForestAttributeModifierRegrowth = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierPaladinHealingPoolBase, "AncientForestAttributeModifierRegrowth")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var ancientForestAttributeModifierRegrowthMultiplier = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierPaladinHealingPoolMultiplier,
                "AncientForestAttributeModifierRegrowthMultiplier")
            .AddToDB();

        var AncientForestAttributeModifierBarkskin = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierBarkskin, "AncientForestAttributeModifierBarkskin")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        return CharacterSubclassDefinitionBuilder
            .Create("AncientForest")
            .SetGuiPresentation("WarlockAncientForest", Category.Subclass,
                TraditionGreenmage.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(ancientForestExpandedSpellListAffinity, 1)
            .AddFeatureAtLevel(ancientForestAttributeModifierRegrowth, 1)
            .AddFeatureAtLevel(ancientForestAttributeModifierRegrowthMultiplier, 1)
            .AddFeatureAtLevel(regrowth, 1)
            .AddFeatureAtLevel(AncientForestBonusCantrip, 1)
            .AddFeatureAtLevel(herbalBrewFeatureSet, 6)
            .AddFeatureAtLevel(lifeSapFeature, 6)
            .AddFeatureAtLevel(ancientForestLightAffinity, 10)
            .AddFeatureAtLevel(rootedPower, 10)
            .AddFeatureAtLevel(AncientForestAttributeModifierBarkskin, 14)
            .AddFeatureAtLevel(WallofThornsFeatureSet, 14)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildHerbalBrew(FeatureDefinitionPower pool,
        string type,
        ItemDefinition baseItem)
    {
        var itemTitle = $"Equipment/&HerbalBrew{type}Title";
        var itemName = $"AncientForestHerbalBrew{type}Item";
        var powerName = $"AncientForestHerbalBrew{type}Power";

        var guiPresentation = new GuiPresentationBuilder(
            itemTitle,
            baseItem.GuiPresentation.Description,
            baseItem.GuiPresentation.SpriteReference
        ).Build();

        var food = new FoodDescription {nutritiveCapacity = 0, perishable = true};

        var brewItem = ItemDefinitionBuilder.Create(baseItem, itemName)
            .SetGold(0)
            .AddToDB();
        brewItem.guiPresentation = guiPresentation;
        brewItem.isFood = true;
        brewItem.foodDescription = food;
        brewItem.IsUsableDevice = true;
        brewItem.usableDeviceDescription = baseItem.UsableDeviceDescription;

        var brewForm = new EffectFormBuilder()
            .SetSummonItemForm(brewItem, 1)
            .SetBonusMode(AddBonusMode.DoubleProficiency)
            .Build();

        var brewEffect = new EffectDescriptionBuilder()
            .AddEffectForm(brewForm)
            .SetDurationData(DurationType.UntilLongRest)
            .SetTargetingData(
                Side.Ally,
                RangeType.Self,
                1,
                TargetType.Self,
                1,
                1,
                ActionDefinitions.ItemSelectionType.Equiped)
            .Build();

        return new FeatureDefinitionPowerSharedPoolBuilder(
            powerName,
            pool,
            RechargeRate.ShortRest,
            ActivationTime.NoCost,
            1,
            false,
            false,
            AttributeDefinitions.Charisma,
            brewEffect,
            guiPresentation,
            false
        ).AddToDB();
    }

    private static FeatureDefinitionPower BuildHerbalBrew(FeatureDefinitionPower pool,
        FeatureDefinitionDamageAffinity resType,
        ItemDefinition baseItem)
    {
        var resTypeName = resType.Name;

        var itemName = $"AncientForestHerbalBrew{resTypeName}Item";
        var powerName = $"AncientForestHerbalBrew{resTypeName}Power";

        var guiPresentation = new GuiPresentationBuilder(
            $"Equipment/&HerbalBrew{resTypeName}Title",
            $"Equipment/&HerbalBrew{resTypeName}Description",
            baseItem.GuiPresentation.SpriteReference
        ).Build();

        var resistanceCondition = ConditionDefinitionBuilder.Create(
                $"AncientForestHerbalBrew{resTypeName}Condition")
            .SetDuration(DurationType.Hour, 1)
            .SetSilent(Silent.None)
            .SetGuiPresentation(guiPresentation)
            .AddFeatures(resType)
            .AddToDB();

        var potionFunction = FeatureDefinitionPowerBuilder
            .Create($"AncientForestPotion{resTypeName}Function")
            .SetGuiPresentation(new GuiPresentationBuilder(guiPresentation)
                .SetTitle("Equipment/&FunctionPotionDrinkTitle").Build())
            .Configure(
                1,
                UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                ActivationTime.Action,
                1,
                RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescriptionBuilder()
                    .AddEffectForm(
                        new EffectFormBuilder().SetConditionForm(
                            resistanceCondition,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true
                        ).Build())
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .Build()
                ,
                true)
            .AddToDB();

        var food = new FoodDescription {nutritiveCapacity = 0, perishable = true};

        var brewItem = ItemDefinitionBuilder.Create(baseItem, itemName)
            .SetGold(0)
            .SetGuiPresentation(guiPresentation)
            .MakeMagical()
            .SetUsableDeviceDescription(potionFunction)
            .AddToDB();
        var description = brewItem.UsableDeviceDescription;
        brewItem.guiPresentation = guiPresentation;
        brewItem.isFood = true;
        brewItem.itemRarity = ItemRarity.Common;
        brewItem.foodDescription = food;
        brewItem.IsUsableDevice = true;
        brewItem.requiresIdentification = false;
        brewItem.usableDeviceDescription = description;

        var brewForm = new EffectFormBuilder()
            .SetSummonItemForm(brewItem, 1)
            .SetBonusMode(AddBonusMode.DoubleProficiency)
            .Build();


        var brewEffect = new EffectDescriptionBuilder()
            .AddEffectForm(brewForm)
            .SetDurationData(DurationType.UntilLongRest)
            .SetTargetingData(
                Side.Ally,
                RangeType.Self,
                1,
                TargetType.Self,
                1,
                1,
                ActionDefinitions.ItemSelectionType.Equiped)
            .Build();


        return new FeatureDefinitionPowerSharedPoolBuilder(
            powerName,
            pool,
            RechargeRate.ShortRest,
            ActivationTime.NoCost,
            1,
            false,
            false,
            AttributeDefinitions.Charisma,
            brewEffect,
            guiPresentation,
            false
        ).AddToDB();
    }
    
    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
#endif
