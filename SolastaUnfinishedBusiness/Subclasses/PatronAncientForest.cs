using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
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
    internal PatronAncientForest()
    {
        var spellListAncientForest = SpellListDefinitionBuilder
            .Create(SpellListPaladin, "SpellListAncientForest")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, Goodberry, Entangle)
            .SetSpellsAtLevel(2, ProtectionFromPoison, SpikeGrowth)
            .SetSpellsAtLevel(3, Revivify, StinkingCloud)
            .SetSpellsAtLevel(4, Blight, GiantInsect)
            .SetSpellsAtLevel(5, Contagion, InsectPlague)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityAncientForestExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityAncientForestExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListAncientForest)
            .AddToDB();

        const string LIFE_SAP_NAME = "OnMagicalAttackDamageEffectAncientForestLifeSap";

        var lifeSapFeature = FeatureDefinitionOnMagicalAttackDamageEffectBuilder
            .Create(LIFE_SAP_NAME)
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

                attacker.UsedSpecialFeatures.TryGetValue(LIFE_SAP_NAME, out var used);

                if (!belowHalfHealth && used != 0)
                {
                    return;
                }

                attacker.UsedSpecialFeatures[LIFE_SAP_NAME] = used + 1;

                var level = caster.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
                var healing = used == 0 && belowHalfHealth ? level : Mathf.CeilToInt(level / 2f);
                var cap = used == 0 ? HealingCap.MaximumHitPoints : HealingCap.HalfMaximumHitPoints;
                var ability = GuiPresentationBuilder.CreateTitleKey(LIFE_SAP_NAME, Category.Feature);

                GameConsoleHelper.LogCharacterActivatesAbility(caster, ability);
                RulesetCharacter.Heal(healing, caster, caster, cap, caster.Guid);
            })
            .AddToDB();

        var powerAncientForestRegrowth = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinLayOnHands, "PowerAncientForestRegrowth")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealing)
            .AddToDB();

        var bonusCantripAncientForest = FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripAncientForest")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Shillelagh, ChillTouch)
            .AddToDB();

        var powerPoolAncientForestHerbalBrew = FeatureDefinitionPowerBuilder
            .Create("PowerPoolAncientForestHerbalBrew")
            .SetGuiPresentation(Category.Feature, PotionRemedy)
            .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest)
            .SetIsPowerPool()
            .SetBonusToAttack(true)
            .AddToDB();

        PowersBundleContext.RegisterPowerBundle(powerPoolAncientForestHerbalBrew, true,
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, "Toxifying", Poison_Basic),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, "Healing", PotionOfHealing),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityAcidResistance, PotionOfSpeed),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityLightningResistance, Ingredient_RefinedOil),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityNecroticResistance, PotionOfClimbing),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityPoisonResistance, PotionOfHeroism),
            BuildHerbalBrew(powerPoolAncientForestHerbalBrew, DamageAffinityRadiantResistance, PotionOfInvisibility)
        );

        _ = RestActivityDefinitionBuilder
            .Create("RestActivityAncientForestToxifying")
            .SetGuiPresentation(powerPoolAncientForestHerbalBrew.GuiPresentation)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                PowersBundleContext.UseCustomRestPowerFunctorName,
                powerPoolAncientForestHerbalBrew.name)
            .AddToDB();

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
            .SetEffectDescription(Entangle.EffectDescription, true)
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
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerPoolAncientForestWallOfThorns = FeatureDefinitionPowerBuilder
            .Create("PowerPoolAncientForestWallOfThorns")
            .SetGuiPresentationNoContent()
            .SetUsesAbilityBonus(ActivationTime.Permanent, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetIsPowerPool()
            .SetBonusToAttack(true)
            .AddToDB();

        var featureSetWallOfThorns = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWallOfThorns")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var wallOfThornsSpells = new[] { WallOfThornsWallLine, WallOfThornsWallRing };

        foreach (var spell in wallOfThornsSpells)
        {
            FeatureDefinitionPower wallOfThorns = FeatureDefinitionPowerSharedPoolBuilder
                .Create("PowerSharedPoolAncientForest" + spell.name)
                .SetGuiPresentation(spell.GuiPresentation)
                .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest)
                .SetEffectDescription(spell.effectDescription, true)
                .SetSharedPool(powerPoolAncientForestWallOfThorns)
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
            .Create("PatronAncientForest")
            .SetGuiPresentation(Category.Subclass, TraditionGreenmage)
            .AddFeaturesAtLevel(1,
                magicAffinityAncientForestExpandedSpells,
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
        .SubclassChoiceWarlockOtherworldlyPatrons;

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
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerName)
            .SetGuiPresentation(guiPresentation)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(brewEffect)
            .SetSharedPool(pool)
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
            .SetDuration(DurationType.Hour, 1)
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
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
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
            .SetUsableDeviceDescription(new[] { powerAncientForestPotion })
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
            .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
            .Build();

        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerName)
            .SetGuiPresentation(guiPresentation)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(brewEffect)
            .SetSharedPool(pool)
            .AddToDB();
    }
}
