using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CircleOfTheAncientForest : AbstractSubclass
{
    private const string LifeSapName = "OnMagicalAttackDamageEffectAncientForestLifeSap";

    internal CircleOfTheAncientForest()
    {
        var bonusCantripAncientForest = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripAncientForest")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Shillelagh, ChillTouch)
            .AddToDB();

        var autoPreparedSpellsForestGuardian = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsAncientForest")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Goodberry, Entangle),
                BuildSpellGroup(3, ProtectionFromPoison, SpikeGrowth),
                BuildSpellGroup(5, Revivify, StinkingCloud),
                BuildSpellGroup(7, Blight, GiantInsect),
                BuildSpellGroup(9, Contagion, InsectPlague))
            .SetSpellcastingClass(DatabaseHelper.CharacterClassDefinitions.Druid)
            .AddToDB();

        var lifeSapFeature = FeatureDefinitionBuilder
            .Create(LifeSapName)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new OnMagicalAttackDamageEffectAncientForestLifeSap())
            .AddToDB();

        var powerAncientForestRegrowth = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinLayOnHands, "PowerAncientForestRegrowth")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealing)
            .AddToDB();

        var powerPoolAncientForestHerbalBrew = FeatureDefinitionPowerBuilder
            .Create("PowerPoolAncientForestHerbalBrew")
            .SetGuiPresentation(Category.Feature, PotionRemedy)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetBonusToAttack(true)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPoolAncientForestHerbalBrew, true,
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, "Toxifying", Poison_Basic),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, "Healing", PotionOfHealing),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityAcidResistance, PotionOfSpeed),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityLightningResistance, Ingredient_RefinedOil),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityNecroticResistance, PotionOfClimbing),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityPoisonResistance, PotionOfHeroism),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityRadiantResistance, PotionOfInvisibility)
        );

        var lightAffinityAncientForest = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityAncientForestPhotosynthesis")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Bright,
                condition = ConditionDefinitionBuilder
                    .Create("ConditionAncientForestPhotosynthesis")
                    .SetGuiPresentation(Category.Condition)
                    .SetSilent(Silent.None)
                    .AddFeatures(DatabaseHelper.FeatureDefinitionRegenerations.RegenerationRing)
                    .AddToDB()
            })
            .AddToDB();

        var powerAncientForestEntangleAtWill = FeatureDefinitionPowerBuilder
            .Create("PowerAncientForestEntangleAtWill")
            .SetGuiPresentation(Entangle.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(Entangle.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var conditionAncientForestRooted = ConditionDefinitionBuilder
            .Create("ConditionAncientForestRooted")
            .SetSilent(Silent.None)
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .AddFeatures(DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity)
            .AddFeatures(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging)
            .AddFeatures(powerAncientForestEntangleAtWill)
            .AddToDB();

        var powerAncientForestRooted = FeatureDefinitionPowerBuilder
            .Create("PowerAncientForestRooted")
            .SetGuiPresentation(Category.Feature, PowerRangerHideInPlainSight)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAncientForestRooted,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerPoolAncientForestWallOfThorns = FeatureDefinitionPowerBuilder
            .Create("PowerPoolAncientForestWallOfThorns")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetBonusToAttack(true)
            .AddToDB();

        var featureSetWallOfThorns = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWallOfThorns")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPoolAncientForestWallOfThorns)
            .AddToDB();

        var wallOfThornsSpells = new[] { WallOfThornsWallLine, WallOfThornsWallRing };

        foreach (var spell in wallOfThornsSpells)
        {
            FeatureDefinitionPower wallOfThorns = FeatureDefinitionPowerSharedPoolBuilder
                .Create("PowerSharedPoolAncientForest" + spell.name)
                .SetGuiPresentation(spell.GuiPresentation)
                .SetSharedPool(ActivationTime.Rest, powerPoolAncientForestWallOfThorns)
                .SetEffectDescription(spell.effectDescription)
                .AddToDB();

            featureSetWallOfThorns.FeatureSet.Add(wallOfThorns);
        }

        var attributeModifierAncientForestRegrowth = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierPaladinHealingPoolBase, "AttributeModifierAncientForestRegrowth")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var attributeModifierAncientForestRegrowthMultiplier = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierPaladinHealingPoolMultiplier, "AttributeModifierAncientForestRegrowthMultiplier")
            .AddToDB();

        var attributeModifierAncientForestBarkskin = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierBarkskin, "AttributeModifierAncientForestBarkskin")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("CircleOfTheAncientForest")
            .SetGuiPresentation(Category.Subclass, TraditionGreenmage)
            .AddFeaturesAtLevel(2,
                autoPreparedSpellsForestGuardian,
                attributeModifierAncientForestRegrowth,
                attributeModifierAncientForestRegrowthMultiplier,
                powerAncientForestRegrowth,
                bonusCantripAncientForest)
            .AddFeaturesAtLevel(6,
                powerPoolAncientForestHerbalBrew,
                lifeSapFeature)
            .AddFeaturesAtLevel(10,
                lightAffinityAncientForest,
                powerAncientForestRooted)
            .AddFeaturesAtLevel(14,
                attributeModifierAncientForestBarkskin,
                featureSetWallOfThorns)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => DatabaseHelper.FeatureDefinitionSubclassChoices
        .SubclassChoiceDruidCircle;

    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPower BuildHerbalBrew(
        FeatureDefinitionPower pool,
        string type,
        ItemDefinition baseItem)
    {
        var itemTitle = $"Equipment/&HerbalBrew{type}Title";
        var itemName = $"ItemAncientForestHerbalBrew{type}";
        var powerName = $"PowerAncientForestHerbalBrew{type}";

        var guiPresentation = new GuiPresentationBuilder(
                itemTitle,
                baseItem.GuiPresentation.Description,
                baseItem.GuiPresentation.SpriteReference)
            .Build();

        var foodDescription = new FoodDescription { nutritiveCapacity = 0, perishable = true };

        var brewItem = ItemDefinitionBuilder
            .Create(baseItem, itemName)
            .SetGuiPresentation(guiPresentation)
            .SetFoodDescription(foodDescription)
            .SetUsableDeviceDescription(baseItem.UsableDeviceDescription)
            .SetGold(0)
            .AddToDB();

        var brewForm = EffectFormBuilder
            .Create()
            .SetSummonItemForm(brewItem, 1)
            .SetBonusMode(AddBonusMode.DoubleProficiency)
            .Build();

        var brewEffect = EffectDescriptionBuilder
            .Create()
            .SetEffectForms(brewForm)
            .SetDurationData(DurationType.UntilLongRest)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerName)
            .SetGuiPresentation(guiPresentation)
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(brewEffect)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildHerbalBrew(
        FeatureDefinitionPower pool,
        FeatureDefinition featureDefinitionDamageAffinity,
        ItemDefinition baseItem)
    {
        var name = featureDefinitionDamageAffinity.Name;
        var itemName = $"ItemAncientForestHerbalBrew{name}";
        var powerName = $"PowerAncientForestHerbalBrew{name}";

        var guiPresentation = new GuiPresentationBuilder(
                $"Equipment/&HerbalBrew{name}Title",
                $"Equipment/&HerbalBrew{name}Description",
                baseItem.GuiPresentation.SpriteReference)
            .Build();

        var conditionAncientForestHerbalBrew = ConditionDefinitionBuilder
            .Create($"ConditionAncientForestHerbalBrew{name}")
            .SetSilent(Silent.None)
            .SetGuiPresentation(guiPresentation)
            .AddFeatures(featureDefinitionDamageAffinity)
            .AddToDB();

        var powerAncientForestPotion = FeatureDefinitionPowerBuilder
            .Create($"PowerAncientForestPotion{name}")
            .SetGuiPresentation(new GuiPresentationBuilder(guiPresentation)
                .SetTitle("Equipment/&FunctionPotionDrinkTitle")
                .Build())
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAncientForestHerbalBrew,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var foodDescription = new FoodDescription { nutritiveCapacity = 0, perishable = true };

        var brewItem = ItemDefinitionBuilder
            .Create(baseItem, itemName)
            .SetGold(0)
            .SetGuiPresentation(guiPresentation)
            .MakeMagical()
            .SetFoodDescription(foodDescription)
            .SetUsableDeviceDescription(powerAncientForestPotion)
            .SetItemRarity(ItemRarity.Common)
            .SetRequiresIdentification(false)
            .AddToDB();

        var brewForm = EffectFormBuilder
            .Create()
            .SetSummonItemForm(brewItem, 1)
            .SetBonusMode(AddBonusMode.DoubleProficiency)
            .Build();

        var brewEffect = EffectDescriptionBuilder
            .Create()
            .SetEffectForms(brewForm)
            .SetDurationData(DurationType.UntilLongRest)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerName)
            .SetGuiPresentation(guiPresentation)
            .SetSharedPool(ActivationTime.Action, pool)
            .SetEffectDescription(brewEffect)
            .AddToDB();
    }

    private sealed class OnMagicalAttackDamageEffectAncientForestLifeSap : IOnMagicalAttackDamageEffect
    {
        public void AfterOnMagicalAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var caster = attacker.RulesetCharacter;

            if (caster.MissingHitPoints <= 0 ||
                !rulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                return;
            }

            var belowHalfHealth = caster.MissingHitPoints > caster.CurrentHitPoints;

            attacker.UsedSpecialFeatures.TryGetValue(LifeSapName, out var used);

            if (!belowHalfHealth && used != 0)
            {
                return;
            }

            attacker.UsedSpecialFeatures[LifeSapName] = used + 1;

            var level = caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            var healing = used == 0 && belowHalfHealth ? level : Mathf.CeilToInt(level / 2f);
            var cap = used == 0 ? HealingCap.MaximumHitPoints : HealingCap.HalfMaximumHitPoints;
            var ability = GuiPresentationBuilder.CreateTitleKey(LifeSapName, Category.Feature);

            GameConsoleHelper.LogCharacterActivatesAbility(caster, ability);
            RulesetCharacter.Heal(healing, caster, caster, cap, caster.Guid);
        }
    }
}
