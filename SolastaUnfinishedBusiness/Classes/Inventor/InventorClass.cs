using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class InventorClass
{
    public const string ClassName = "Inventor";

    private static readonly AssetReferenceSprite Sprite =
        CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference;

    private static readonly AssetReferenceSprite Pictogram =
        CharacterClassDefinitions.Wizard.ClassPictogramReference;

    private static SpellListDefinition _spellList;
    private static readonly LimitedEffectInstances InfusionLimiter = new("Infusion", _ => 2);

    private static CharacterClassDefinition Class { get; set; }

    public static FeatureDefinitionPower InfusionPool { get; private set; }
    public static SpellListDefinition SpellList => _spellList ??= BuildSpellList();

    public static FeatureDefinitionCastSpell SpellCasting { get; private set; }

    public static CharacterClassDefinition Build()
    {
        if (Class != null)
        {
            throw new ArgumentException("Trying to build Inventor class additional time.");
        }

        InfusionPool = BuildInfusionPool();
        SpellCasting = BuildSpellCasting();

        Class = CharacterClassDefinitionBuilder
            .Create(ClassName)

            #region Presentation

            .SetGuiPresentation(Category.Class, Sprite)
            .SetPictogram(Pictogram)
            //.AddPersonality() //TODO: Add personality flags
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter)

            #endregion

            #region Priorities

            .SetAbilityScorePriorities(
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma
            )
            .AddSkillPreferences(
                SkillDefinitions.Athletics,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Stealth,
                SkillDefinitions.Religion,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival
            )
            .AddToolPreferences(
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType
            )
            //TODO: add dynamic preferred feats that can include ones that are disabled, but offered if enabled
            .AddFeatPreferences() //TODO: Add preferred feats

            #endregion

            .SetBattleAI(DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetIngredientGatheringOdds(CharacterClassDefinitions.Fighter.IngredientGatheringOdds)
            .SetHitDice(DieType.D8)

            #region Equipment

            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Club,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Shield,
                        EquipmentDefinitions.OptionArmor, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Club,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
                }
            )
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow,
                    EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt,
                    EquipmentDefinitions.OptionAmmoPack, 1)
            })
            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch,
                        EquipmentDefinitions.OptionFocus, 1)
                }
            )
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.StuddedLeather,
                        EquipmentDefinitions.OptionArmor, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ScaleMail,
                        EquipmentDefinitions.OptionArmor, 1)
                }
            )
            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack,
                        EquipmentDefinitions.OptionStarterPack, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ExplorerPack,
                        EquipmentDefinitions.OptionStarterPack, 1)
                }
            )

            #endregion

            #region Proficiencies

            // Weapons
            .AddFeaturesAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyInventorWeapon")
                .SetGuiPresentation(Category.Feature, "Feature/&WeaponTrainingShortDescription")
                .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.SimpleWeaponCategory)
                .AddToDB())

            // Armor
            .AddFeaturesAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyInventorArmor")
                .SetGuiPresentation(Category.Feature, "Feature/&ArmorTrainingShortDescription")
                .SetProficiencies(ProficiencyType.Armor,
                    EquipmentDefinitions.LightArmorCategory,
                    EquipmentDefinitions.MediumArmorCategory,
                    EquipmentDefinitions.ShieldCategory)
                .AddToDB())

            // Saves
            .AddFeaturesAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyInventorSavingThrow")
                .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
                .SetProficiencies(ProficiencyType.SavingThrow,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Dexterity)
                .AddToDB())

            // Skill points
            .AddFeaturesAtLevel(1, FeatureDefinitionPointPoolBuilder
                .Create("PointPoolInventorSkills")
                .SetGuiPresentation(Category.Feature, "Feature/&SkillGainChoicesPluralDescription")
                .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
                .OnlyUniqueChoices()
                .RestrictChoices( //TODO: decide on final skill list
                    SkillDefinitions.Arcana,
                    SkillDefinitions.Medecine,
                    SkillDefinitions.History,
                    SkillDefinitions.Insight,
                    SkillDefinitions.Religion,
                    SkillDefinitions.Survival,
                    SkillDefinitions.Nature
                )
                .AddToDB())

            #endregion

            #region Subclasses

            .AddFeaturesAtLevel(1, FeatureDefinitionSubclassChoiceBuilder
                .Create("SubclassChoiceInventor")
                .SetGuiPresentation("InventorInnovation", Category.Subclass)
                .SetSubclassSuffix("InventorInnovation")
                .SetFilterByDeity(false)
                .SetSubclasses(
                    InnovationAlchemy.Build(),
                    // InnovationNecromancy.Build(),
                    InnovationWeapon.Build()
                )
                .AddToDB())

            #endregion

            #region Level 01

            .AddFeaturesAtLevel(1, SpellCasting, BuildInfusions())
            .AddFeaturesAtLevel(2, TestInvocations())
            .AddFeaturesAtLevel(3, TestInvocations2())

            #endregion

            .AddToDB();


        return Class;
    }

    private static FeatureDefinition TestInvocations()
    {
        var poolType = InvocationPoolTypes.Infusion;
        
        CustomInvocationDefinitionBuilder
            .Create("TestLeapInvocation")
            .SetGuiPresentation(Category.Feature,SpellDefinitions.JumpOtherworldlyLeap)
            .SetPoolType(poolType)
            .SetRequiredSpell(SpellDefinitions.Aid)
            .SetGrantedSpell(SpellDefinitions.JumpOtherworldlyLeap)
            .AddToDB();
        
        CustomInvocationDefinitionBuilder
            .Create("TestArmorInvocation")
            .SetGuiPresentation(Category.Feature,SpellDefinitions.MageArmorInvocationArmorShadows)
            .SetPoolType(poolType)
            .SetRequiredLevel(2)
            .SetGrantedSpell(SpellDefinitions.MageArmorInvocationArmorShadows)
            .AddToDB();
        
        CustomInvocationDefinitionBuilder
            .Create("TestBlurInvocation")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Blur)
            .SetPoolType(poolType)
            .SetRequiredLevel(3)
            .SetRequiredPact(FeatureDefinitionAttributeModifiers.AttributeModifierBarbarianExtraAttack)
            .SetGrantedSpell(SpellDefinitions.Blur)
            .AddToDB();
        
        CustomInvocationDefinitionBuilder
            .Create("TestTHirstInvocation")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MagicWeapon)
            .SetPoolType(poolType)
            .SetGrantedFeature(FeatureDefinitionAttributeModifiers.AttributeModifierThirstingBladeExtraAttack)
            .AddToDB();


        return CustomInvocationPoolDefinitionBuilder
            .Create("TestInvocationPool")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Fly)
            .Setup(poolType, 1)
            .AddToDB();
    }

    private static FeatureDefinition TestInvocations2()
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("TestInvocationPoolReplace")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Fly)
            .Setup(InvocationPoolTypes.Infusion, 1, true)
            .AddToDB();
    }

    private static SpellListDefinition BuildSpellList()
    {
        return SpellListDefinitionBuilder
            .Create("SpellListInventor")
            .SetGuiPresentationNoContent(true) // spell lists don't need Gui presentation
            .ClearSpells()
            .SetSpellsAtLevel(0,
                SpellDefinitions.AcidSplash,
                SpellDefinitions.FireBolt,
                SpellDefinitions.RayOfFrost,
                SpellDefinitions.Light,
                SpellDefinitions.PoisonSpray,
                SpellDefinitions.Resistance,
                SpellDefinitions.ShockingGrasp,
                SpellDefinitions.Shine,
                SpellDefinitions.Sparkle
            )
            .FinalizeSpells()
            .AddToDB();
    }

    private static FeatureDefinitionCastSpell BuildSpellCasting()
    {
        return FeatureDefinitionCastSpellBuilder
            .Create("CastSpellsInventor")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6)
            .SetKnownSpells()
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellList)
            .AddToDB();
    }


    private static FeatureDefinitionPower BuildInfusionPool()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerInfusionPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(20)
            .SetRechargeRate(RechargeRate.ShortRest)
            .AddToDB();
    }

    public static FeatureDefinition BuildInfusions()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorInfusions")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                InfusionPool,
                BuildTestInfusion()
            )
            .AddToDB();
    }

    private static FeatureDefinition BuildTestInfusion()
    {
        var testSummonItem = FeatureDefinitionPowerBuilder.Create("TMPPowerTestSummonItem")
            .SetGuiPresentation(Category.Feature, ItemDefinitions.Dagger)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            // .SetUniqueInstance()
            // .SetSharedPool(InventorClass.InfusionPool)
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker, InfusionLimiter)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetAnimation(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Self)
                .SetSavingThrowData(
                    false,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(SpellDefinitions.Bless)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetSummonItemForm(ItemDefinitions.Dagger, 1, true)
                    .Build())
                .Build())
            .AddToDB();

        var testInfuseItem = FeatureDefinitionPowerBuilder.Create("TMPPowerTestInfuseItem")
            .SetGuiPresentation(Category.Feature, ItemDefinitions.BONEKEEP_MagicRune)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetUniqueInstance()
            // .SetSharedPool(InventorClass.InfusionPool)
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker,
                InfusionLimiter,
                new CustomItemFilter((_, item) => item.ItemDefinition.IsWeapon))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetAnimation(AnimationDefinitions.AnimationMagicEffect.Animation1)
                // .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalSpellLevelPerIncrement: 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetSavingThrowData(
                    false,
                    true,
                    AttributeDefinitions.Dexterity,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerOathOfJugementWeightOfJustice)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1,
                        new FeatureUnlockByLevel
                        {
                            level = 0,
                            featureDefinition = FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3
                        })
                    .Build())
                .Build())
            .AddToDB();

        //TODO: make some builder for these fake items
        var testItem = ItemDefinitionBuilder
            .Create("TMPTestItem")
            .SetGuiPresentation(Category.Feature, ItemDefinitions.ArwinMertonSword)
            .SetRequiresIdentification(true)
            .SetWeight(0)
            .SetItemPresentation(CustomWeaponsContext.BuildPresentation("TMPTestItemUnid",
                ItemDefinitions.ScrollFly.itemPresentation))
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetRecharge(RechargeRate.ShortRest)
                .SetSaveDc(-1) //Set to -1 so that it will calculate based on actual powers
                .AddFunctions(
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(testSummonItem, true)
                        .Build(),
                    new DeviceFunctionDescriptionBuilder()
                        .SetUsage(useAmount: 2, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                        .SetPower(testInfuseItem, true)
                        .Build()
                )
                .Build())
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create("TMPTestInfusion")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new PowerPoolDevice(testItem, InfusionPool))
            .AddToDB();
    }
}
