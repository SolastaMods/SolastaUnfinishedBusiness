using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Classes;

internal static class InventorClass
{
    internal const string ClassName = "Inventor";
    internal const string InventorConstructFamily = "InventorConstruct";
    private const string InfusionsName = "FeatureInventorInfusionPool";
    private const string LimiterName = "Infusion";

    internal static readonly LimitEffectInstances InfusionLimiter = new(LimiterName, GetInfusionLimit);

    private static SpellListDefinition _spellList;
    private static FeatureDefinitionCastSpell _spellCasting;
    private static int _infusionPoolIncreases;
    private static readonly List<FeatureDefinitionPowerSharedPool> SpellStoringItemPowers1 = [];
    private static readonly List<FeatureDefinitionPowerSharedPool> SpellStoringItemPowers2 = [];

    private static readonly FeatureDefinitionPower PowerInventorSpellStoringItem1 = FeatureDefinitionPowerBuilder
        .Create("PowerInventorSpellStoringItem")
        .SetGuiPresentation(Category.Feature, ItemDefinitions.WandMagicMissile)
        .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerInventorSpellStoringItem2 =
        FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInventorSpellStoringItem2")
            .SetGuiPresentation("PowerInventorSpellStoringItem", Category.Feature, ItemDefinitions.WandMagicMissile)
            .SetSharedPool(ActivationTime.Action, PowerInventorSpellStoringItem1)
            .AddToDB();

    private static readonly int[] Costs = [0, 0, 0, 0, 0];

    internal static FeatureDefinitionCastSpell SpellCasting => _spellCasting ??= BuildSpellCasting();

    internal static CharacterClassDefinition Class { get; private set; }

    internal static FeatureDefinitionSubclassChoice SubclassChoice { get; } = FeatureDefinitionSubclassChoiceBuilder
        .Create("SubclassChoiceInventor")
        .SetGuiPresentation("InventorInnovation", Category.Subclass)
        .SetSubclassSuffix("InventorInnovation")
        .SetFilterByDeity(false)
        .AddToDB();

    internal static FeatureDefinitionPower InfusionPool { get; } = BuildInfusionPool();
    internal static SpellListDefinition SpellList => _spellList ??= BuildSpellList();

    public static void Build()
    {
        // Inventor Constructor Family

        _ = CharacterFamilyDefinitionBuilder
            .Create(InventorConstructFamily)
            .SetGuiPresentation(Category.MonsterFamily)
            .IsExtraPlanar()
            .AddToDB();

        var powerUseModifierInventorInfusionPool2 = FeatureDefinitionPowerUseModifierBuilder
            .Create("PowerUseModifierInventorInfusionPool2")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(InfusionPool, 2)
            .AddToDB();

        var learn2Infusion = BuildLearn(2);
        var featureSetInventorInfusions = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorInfusions")
            .SetGuiPresentation(InfusionsName, Category.Feature)
            .AddFeatureSet(InfusionPool, BuildLearn(4), powerUseModifierInventorInfusionPool2)
            .AddToDB();

        var unlearn = BuildUnlearn();

        InventorInfusions.Build();

        var builder = CharacterClassDefinitionBuilder
            .Create(ClassName)

            #region Presentation

            .SetGuiPresentation(
                Category.Class,
                Sprites.GetSprite(ClassName, Resources.Inventor, 1024, 576),
                hidden: true)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter)
            .SetPictogram(Sprites.GetSprite("InventorPictogram", Resources.InventorPictogram, 128));

        Wizard.personalityFlagOccurences
            .ForEach(fo => builder.AddPersonality(fo.personalityFlag, fo.weight));

        #endregion

        var powerInventorSoulOfArtifice = FeatureDefinitionPowerBuilder
            .Create("PowerInventorSoulOfArtifice")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        powerInventorSoulOfArtifice.AddCustomSubFeatures(
            new CustomBehaviorInitiatedSoulOfArtifice(powerInventorSoulOfArtifice));

        #region Priorities

        builder
            .SetAbilityScorePriorities(
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddSkillPreferences(
                SkillDefinitions.Athletics,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Stealth,
                SkillDefinitions.Religion,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival)
            .AddToolPreferences(
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.ScrollKitType)
            //TODO: Add more preferred feats
            .AddFeatPreferences(
                OtherFeats.FeatWarCaster,
                "PowerfulCantrip",
                "FlawlessConcentration")

            #endregion

            .SetBattleAI(DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetIngredientGatheringOdds(Wizard.IngredientGatheringOdds)
            .SetHitDice(DieType.D8)

            #region Equipment

            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Mace,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Shield,
                        EquipmentDefinitions.OptionArmor, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
                })
            .AddEquipmentRow(
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow,
                    EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt,
                    EquipmentDefinitions.OptionAmmoPack, 1))
            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch,
                        EquipmentDefinitions.OptionFocus, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch_Belt,
                        EquipmentDefinitions.OptionFocus, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch_Bracers,
                        EquipmentDefinitions.OptionFocus, 1)
                })
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.StuddedLeather,
                        EquipmentDefinitions.OptionArmor, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ScaleMail,
                        EquipmentDefinitions.OptionArmor, 1)
                })
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
                })

            #endregion

            #region Proficiencies

            // Weapons
            .AddFeaturesAtLevel(
                1, FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyInventorWeapon")
                    .SetGuiPresentation(Category.Feature, "Feature/&WeaponTrainingShortDescription")
                    .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.SimpleWeaponCategory)
                    .AddToDB())

            // Armor
            .AddFeaturesAtLevel(
                1, FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyInventorArmor")
                    .SetGuiPresentation(Category.Feature, "Feature/&ArmorTrainingShortDescription")
                    .SetProficiencies(ProficiencyType.Armor,
                        EquipmentDefinitions.LightArmorCategory,
                        EquipmentDefinitions.MediumArmorCategory,
                        EquipmentDefinitions.ShieldCategory)
                    .AddToDB())

            // Saves
            .AddFeaturesAtLevel(
                1, FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyInventorSavingThrow")
                    .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
                    .SetProficiencies(ProficiencyType.SavingThrow,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Constitution)
                    .AddToDB())

            // Tools Proficiency
            .AddFeaturesAtLevel(
                1, FeatureDefinitionProficiencyBuilder
                    .Create("ProficiencyInventorTools")
                    .SetGuiPresentation(Category.Feature, "Feature/&ToolProficiencyPluralDescription")
                    .SetProficiencies(ProficiencyType.Tool,
                        ToolTypeDefinitions.ThievesToolsType.Name,
                        ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
                    .AddToDB())

            // Tool Selection
            .AddFeaturesAtLevel(
                1, FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolInventorTools")
                    .SetGuiPresentation(Category.Feature, "Feature/&ToolGainChoicesSingleDescription")
                    .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
                    .OnlyUniqueChoices()
                    .RestrictChoices(
                        ToolTypeDefinitions.DisguiseKitType.Name,
                        ToolTypeDefinitions.EnchantingToolType.Name,
                        ToolTypeDefinitions.HerbalismKitType.Name,
                        ToolTypeDefinitions.PoisonersKitType.Name,
                        ToolTypeDefinitions.ScrollKitType.Name)
                    .AddToDB())

            // Skill points
            .AddFeaturesAtLevel(
                1, FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolInventorSkills")
                    .SetGuiPresentation(Category.Feature, "Feature/&SkillGainChoicesPluralDescription")
                    .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
                    .OnlyUniqueChoices()
                    .RestrictChoices(
                        SkillDefinitions.Arcana,
                        SkillDefinitions.History,
                        SkillDefinitions.Investigation,
                        SkillDefinitions.Medecine,
                        SkillDefinitions.Nature,
                        SkillDefinitions.Perception,
                        SkillDefinitions.SleightOfHand)
                    .AddToDB())

            #endregion

            #region Level 01

            .AddFeaturesAtLevel(1, SpellCasting, BuildBonusCantrips(), BuildRitualCasting())

            #endregion

            #region Level 02

            .AddFeaturesAtLevel(2, featureSetInventorInfusions)

            #endregion

            #region Level 03

            .AddFeaturesAtLevel(3, BuildRightToolForTheJob())

            #endregion

            #region Level 04

            .AddFeaturesAtLevel(4,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)

            #endregion

            #region Level 06

            .AddFeaturesAtLevel(6, learn2Infusion, BuildInfusionPoolIncrease(), BuildToolExpertise())

            #endregion

            #region Level 07

            .AddFeaturesAtLevel(7, BuildFlashOfGenius())

            #endregion

            #region Level 08

            .AddFeaturesAtLevel(8,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)

            #endregion

            #region Level 10

            .AddFeaturesAtLevel(10, learn2Infusion, BuildMagicAdept(),
                BuildInfusionPoolIncrease())

            #endregion

            #region Level 11

            // this is now handled on late load
            //.AddFeaturesAtLevel(11, BuildSpellStoringItem())

            #endregion

            #region Level 12

            .AddFeaturesAtLevel(12,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)

            #endregion

            #region Level 14

            .AddFeaturesAtLevel(14, learn2Infusion, BuildInfusionPoolIncrease(), BuildMagicItemSavant())

            #endregion

            #region Level 16

            .AddFeaturesAtLevel(16,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice)

            #endregion

            #region Level 18

            .AddFeaturesAtLevel(18, learn2Infusion, BuildInfusionPoolIncrease())

            #endregion

            #region Level 19

            .AddFeaturesAtLevel(19,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 20

            .AddFeaturesAtLevel(20,
                powerInventorSoulOfArtifice);

        #endregion

        for (var i = 3; i <= 20; i++)
        {
            builder.AddFeaturesAtLevel(i, unlearn);
        }

        builder.AddFeaturesAtLevel(3, SubclassChoice);

        BuildCancelAllInfusionsRestActivity();

        RegisterPoILoot();

        builder.SetVocalSpellSemeClass(VocalSpellSemeClass.Arcana);

        Class = builder.AddToDB();
    }

    /**Adds starting chest loot for PoI for Inventor class*/
    private static void RegisterPoILoot()
    {
        var loot = LootPackDefinitionBuilder
            .Create("UB_DLC3_Class_Lootpack_BasicChest_Inventor")
            .SetGuiPresentationNoContent(true)
            .AddExplicitItem(ItemDefinitions.SpearPlus2)
            .AddExplicitItem(ItemDefinitions.ShieldPlus1)
            .AddExplicitItem(CustomWeaponsContext.HandXbowAcid)
            .AddExplicitItem(ItemDefinitions.Bolt, 40)
            .AddExplicitItem(ItemDefinitions.BreastplatePlus1)
            .AddExplicitItem(ItemDefinitions.StuddedLeather_plus_one)
            .AddExplicitItem(ItemDefinitions.Backpack_Handy_Haversack)
            .AddExplicitItem(ItemDefinitions.WandOfWarMagePlus1)
            .AddExplicitItem(ItemDefinitions.RingDetectInvisible)
            .AddToDB();

        if (TryGetDefinition<CharacterToLootPackMapDefinition>("DLC3_CharacterToLootPackMap", out var map))
        {
            map.characterClassToLootPackMappings.Add(
                new CharacterToLootPackMapDefinition.CharacterClassToLootPackMapping
                {
                    className = ClassName, lootPack = loot
                });
        }
    }

    private static FeatureDefinitionProficiency BuildToolExpertise()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInventorToolExpertise")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.ToolOrExpertise,
                // don't use ToolDefinitions.ArtisanToolType as that constant has an incorrect name
                "ArtisanToolSmithToolsType",
                ToolTypeDefinitions.EnchantingToolType.Name,
                ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name,
                ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();
    }

    private static FeatureDefinitionPointPool BuildRightToolForTheJob()
    {
        return FeatureDefinitionPointPoolBuilder
            .Create("PointPoolInventorRightToolForTheJob")
            .SetGuiPresentation(Category.Feature, "Feature/&ToolGainChoicesSingleDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
            .OnlyUniqueChoices()
            .RestrictChoices(
                ToolTypeDefinitions.DisguiseKitType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name,
                ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name,
                ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();
    }

    internal static FeatureDefinitionCustomInvocationPool BuildLearn(int points, string label = null)
    {
        label ??= points.ToString();

        return CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPoolInfusionLearn{label}")
            .SetGuiPresentation($"InvocationPoolInfusionLearn{points}", Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Infusion, points)
            .AddToDB();
    }

    private static FeatureDefinitionCustomInvocationPool BuildUnlearn()
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolInventorUnlearnInfusion")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.Infusion, 1, true)
            .AddToDB();
    }

    internal static FeatureDefinition BuildInfusionPoolIncrease()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierInventorInfusionPool{_infusionPoolIncreases++:D2}")
            .SetGuiPresentation("PowerUseModifierInventorInfusionPool", Category.Feature)
            .SetFixedValue(InfusionPool, 1)
            .AddToDB();
    }

    private static int GetInfusionLimit(RulesetCharacter character)
    {
        return character.GetMaxUsesForPool(InfusionPool);
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
                SpellDefinitions.Guidance,
                SpellDefinitions.RayOfFrost,
                SpellDefinitions.PoisonSpray,
                SpellDefinitions.Resistance,
                SpellDefinitions.ShockingGrasp,
                SpellDefinitions.SpareTheDying)
            // absorb elements, snare, catapult, tasha's caustic brew
            .SetSpellsAtLevel(1,
                SpellDefinitions.CureWounds,
                SpellDefinitions.DetectMagic,
                SpellDefinitions.ExpeditiousRetreat,
                SpellDefinitions.FaerieFire,
                SpellDefinitions.FalseLife,
                SpellDefinitions.FeatherFall,
                SpellDefinitions.Grease,
                SpellDefinitions.Identify,
                SpellDefinitions.Jump,
                SpellDefinitions.Longstrider)
            // web, pyrotechnics, enlarge/reduce
            .SetSpellsAtLevel(2,
                SpellDefinitions.Aid,
                SpellDefinitions.Blur,
                SpellDefinitions.Darkvision,
                SpellDefinitions.EnhanceAbility,
                SpellDefinitions.HeatMetal,
                SpellDefinitions.Invisibility,
                SpellDefinitions.LesserRestoration,
                SpellDefinitions.Levitate,
                SpellDefinitions.MagicWeapon,
                SpellDefinitions.ProtectionFromPoison,
                SpellDefinitions.SeeInvisibility,
                SpellDefinitions.SpiderClimb)
            // blink, elemental weapon, flame arrows
            .SetSpellsAtLevel(3,
                SpellDefinitions.CreateFood,
                SpellDefinitions.DispelMagic,
                SpellDefinitions.Fly,
                SpellDefinitions.Haste,
                SpellDefinitions.ProtectionFromEnergy,
                SpellDefinitions.Revivify)
            // everything
            .SetSpellsAtLevel(4,
                SpellDefinitions.FreedomOfMovement,
                SpellDefinitions.Stoneskin)
            // everything
            .SetSpellsAtLevel(5,
                SpellDefinitions.GreaterRestoration)
            .FinalizeSpells(maxLevel: 5)
            .AddToDB();
    }

    private static FeatureDefinitionCastSpell BuildSpellCasting()
    {
        var castSpellsInventor = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellsInventor")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetFocusType(EquipmentDefinitions.FocusType.Universal) //should we add custom focus type?
            .SetReplacedSpells(1, 0)
            .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp)
            .SetSpellKnowledge(SpellKnowledge.WholeList)
            .SetSpellReadyness(SpellReadyness.Prepared)
            .SetSpellPreparationCount(SpellPreparationCount.AbilityBonusPlusHalfLevel)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellList)
            .AddToDB();

        return castSpellsInventor;
    }

    private static FeatureDefinitionFeatureSet BuildRitualCasting()
    {
        return FeatureDefinitionFeatureSetBuilder.Create("FeatureSetInventorRituals")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(
                FeatureDefinitionMagicAffinityBuilder
                    .Create("MagicAffinityInventorRituals")
                    .SetGuiPresentationNoContent(true)
                    .SetRitualCasting(RitualCasting.Prepared)
                    .AddToDB(),
                FeatureDefinitionActionAffinityBuilder
                    .Create("ActionAffinityInventorRituals")
                    .SetGuiPresentationNoContent(true)
                    .SetAuthorizedActions(ActionDefinitions.Id.CastRitual)
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinitionBonusCantrips BuildBonusCantrips()
    {
        return FeatureDefinitionBonusCantripsBuilder
            .Create("BonusCantripsInventorMagicalTinkering")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitions.Dazzle,
                SpellDefinitions.Light,
                SpellDefinitions.Shine,
                SpellDefinitions.Sparkle)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildInfusionPool()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerInfusionPool")
            .SetGuiPresentation(InfusionsName, Category.Feature)
            .AddCustomSubFeatures(HasModifiedUses.Marker, ModifyPowerVisibility.Hidden)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 0)
            .AddToDB();
    }

    private static void BuildCancelAllInfusionsRestActivity()
    {
        const string POWER_NAME = "PowerAfterRestStopInfusions";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestStopInfusions")
            .SetGuiPresentation(POWER_NAME, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                POWER_NAME)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("RestActivityLongRestStopInfusions")
            .SetGuiPresentation(POWER_NAME, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                POWER_NAME)
            .AddToDB();

        FeatureDefinitionPowerBuilder
            .Create(POWER_NAME)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                new HasActiveInfusions(),
                new LimitEffectInstances(LimiterName, _ => 1))
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .Build())
            .AddToDB();
    }

    private static FeatureDefinitionCraftingAffinity BuildMagicAdept()
    {
        return FeatureDefinitionCraftingAffinityBuilder
            .Create("CraftingAffinityInventorMagicItemAdept")
            .SetGuiPresentation(Category.Feature)
            //increases attunement limit by 1
            .AddCustomSubFeatures(new ModifyAttunementLimit(1))
            .SetAffinityGroups(0.25f, false,
                ToolTypeDefinitions.ThievesToolsType,
                ToolTypeDefinitions.ScrollKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType)
            .AddToDB();
    }

    private static FeatureDefinitionMagicAffinity BuildMagicItemSavant()
    {
        return FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityInventorMagicItemSavant")
            .SetGuiPresentation(Category.Feature)
            //increases attunement limit by 1
            .AddCustomSubFeatures(new ModifyAttunementLimit(1))
            .IgnoreClassRestrictionsOnMagicalItems()
            .AddToDB();
    }

    internal static void SwitchSpellStoringItemSubPower(SpellDefinition spell, bool active)
    {
        if (spell.ActivationTime != ActivationTime.Action || (spell.SpellLevel != 1 && spell.SpellLevel != 2))
        {
            return;
        }

        switch (spell.SpellLevel)
        {
            case 1:
            {
                var power = SpellStoringItemPowers1.FirstOrDefault(x => x.SourceDefinition == spell);

                // Main.Enabled as during initialization the powers weren't registered yet
                if (Main.Enabled && !power)
                {
                    Main.Error("found a null power when trying to switch a spell storing item");
                }

                Switch(PowerInventorSpellStoringItem1, active);
                break;
            }
            case 2:
            {
                var power = SpellStoringItemPowers2.FirstOrDefault(x => x.SourceDefinition == spell);

                // Main.Enabled as during initialization the powers weren't registered yet
                if (Main.Enabled && !power)
                {
                    Main.Error("found a null power when trying to switch a spell storing item");
                }

                Switch(PowerInventorSpellStoringItem2, active);
                break;
            }
        }

        return;

        static void Switch(FeatureDefinitionPower power, bool active)
        {
            var subPowers = power.GetBundle()?.SubPowers;

            if (active)
            {
                subPowers?.TryAdd(power);
            }
            else
            {
                subPowers?.Remove(power);
            }
        }
    }

    internal static void LateLoadSpellStoringItem()
    {
        var featureSet = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetSpellStoringItem")
            .SetGuiPresentation("PowerInventorSpellStoringItem", Category.Feature)
            .AddFeatureSet(
                BuildSpellStoringItem(1, PowerInventorSpellStoringItem1),
                BuildSpellStoringItem(2, PowerInventorSpellStoringItem2))
            .AddToDB();

        Class.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureSet, 11));
        Class.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static FeatureDefinitionPower BuildSpellStoringItem(int level, FeatureDefinitionPower power)
    {
        var spells = SpellsContext.Spells
            .Where(x => x.SpellLevel == level && x.castingTime == ActivationTime.Action);

        var spellStoringItems = level == 1 ? SpellStoringItemPowers1 : SpellStoringItemPowers2;

        // build powers for all level 1 and 2 spells to allow better integration with custom spells selection
        spellStoringItems.AddRange(spells
            .Select(spell =>
                BuildCreateSpellStoringItemPower(BuildWandOfSpell(spell), spell, power)));

        // only register the ones indeed in the inventor spell list
        var inventorPowers = spellStoringItems
            .Where(x => SpellList
                .ContainsSpell(x.SourceDefinition as SpellDefinition))
            .Cast<FeatureDefinitionPower>()
            .ToArray();

        PowerBundle.RegisterPowerBundle(power, true, inventorPowers);
        ForceGlobalUniqueEffects.AddToGroup(
            ForceGlobalUniqueEffects.Group.InventorSpellStoringItem, [.. inventorPowers]);

        return power;
    }

    private static FeatureDefinitionPowerSharedPool BuildCreateSpellStoringItemPower(
        ItemDefinition item, SpellDefinition spell, FeatureDefinitionPower pool)
    {
        var description = Gui.Format("Item/&CreateSpellStoringWandFormatDescription", spell.FormatTitle(),
            Gui.ToRoman(spell.spellLevel));

        var powerName = $"PowerCreate{item.name}";
        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerName)
            .SetGuiPresentation(spell.FormatTitle(), description, spell)
            .SetSharedPool(ActivationTime.Action, pool)
            .SetUniqueInstance()
            .AddCustomSubFeatures(
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                TrackItemsCarefully.Marker,
                SkipEffectRemovalOnLocationChange.Always)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(SpellDefinitions.Bless)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.None)
                            .SetSummonItemForm(item, 1, true)
                            .Build())
                    .Build())
            .AddToDB();

        // need this reference to the spell to be able to later on switch powers on them
        power.SourceDefinition = spell;

        return power;
    }

    private static ItemDefinition BuildWandOfSpell(SpellDefinition spell)
    {
        var spellName = spell.FormatTitle();
        var title = Gui.Format("Item/&SpellStoringWandTitle", spellName);
        var description = Gui.Format("Item/&SpellStoringWandDescription", spellName);

        return ItemDefinitionBuilder
            .Create(ItemDefinitions.WandMagicMissile, $"SpellStoringWandOf{spell.Name}")
            .SetOrUpdateGuiPresentation(title, description)
            .SetRequiresIdentification(false)
            .HideFromDungeonEditor()
            .AddCustomSubFeatures(ModifyAdditionalDamageClassLevelInventor.Instance)
            .SetCosts(Costs)
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetChargesCapitalNumber(6) //TODO: try to make this based off Inventor's INT bonus x2
                .SetOutOfChargesConsequence(EquipmentDefinitions.ItemOutOfCharges.Destroy)
                .SetRecharge(RechargeRate.None)
                .SetSaveDc(EffectHelpers.BasedOnItemSummoner)
                .SetMagicAttackBonus(EffectHelpers.BasedOnItemSummoner)
                .AddFunctions(new DeviceFunctionDescriptionBuilder()
                    .SetUsage(useAmount: 1, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                    .SetSpell(spell)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildFlashOfGenius()
    {
        var sprite = Sprites.GetSprite("InventorQuickWit", Resources.InventorQuickWit, 256, 128);

        var bonusPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusBonus")
            .SetGuiPresentation("PowerInventorFlashOfGenius", Category.Feature, sprite)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var auraPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusAura")
            .SetGuiPresentation("PowerInventorFlashOfGenius", Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(
                        ConditionDefinitionBuilder
                            .Create("ConditionInventorFlashOfGeniusAura")
                            .SetGuiPresentationNoContent(true)
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .AddCustomSubFeatures(new TryAlterOutcomeSavingThrowFlashOfGenius(bonusPower))
                            .AddToDB()))
                    .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorFlashOfGenius")
            .SetGuiPresentation("PowerInventorFlashOfGenius", Category.Feature)
            .AddFeatureSet(auraPower, bonusPower)
            .AddToDB();
    }

    private sealed class CustomBehaviorInitiatedSoulOfArtifice(FeatureDefinitionPower powerSoulOfArtifice)
        : IRollSavingThrowInitiated, IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (ServiceRepository.GetService<IGameLocationBattleService>() is not GameLocationBattleManager
                {
                    IsBattleInProgress: true
                } battleManager)
            {
                yield break;
            }

            var rulesetCharacter = defender.RulesetCharacter;

            if (rulesetCharacter.GetRemainingPowerUses(powerSoulOfArtifice) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerSoulOfArtifice, rulesetCharacter);
            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "SoulOfArtifice",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var hitPoints = rulesetCharacter.GetClassLevel(Class);

            rulesetCharacter.StabilizeAndGainHitPoints(hitPoints);

            EffectHelpers.StartVisualEffect(
                defender, defender, FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift,
                EffectHelpers.EffectType.Caster);
            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(new CharacterActionParams(defender, ActionDefinitions.Id.StandUp), null, true);
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            var attunedItems = 
                defender.CharacterInventory?.items?.Count(x => x.AttunedToCharacter == defender.Name) ?? 0;

            rollModifier += attunedItems;
            modifierTrends.Add(
                new TrendInfo(attunedItems, FeatureSourceType.CharacterFeature,
                    powerSoulOfArtifice.Name, powerSoulOfArtifice));
        }
    }

    private class HasActiveInfusions : IValidatePowerUse
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            return character.PowersUsedByMe.FirstOrDefault(p =>
                p.PowerDefinition.GetFirstSubFeatureOfType<LimitEffectInstances>()?.Name == LimiterName) != null;
        }
    }
}

internal class TryAlterOutcomeSavingThrowFlashOfGenius(FeatureDefinitionPower power)
    : ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
{
    public IEnumerator OnTryAlterAttributeCheck(
        GameLocationBattleManager battleManager,
        AbilityCheckData abilityCheckData,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier abilityCheckModifier)
    {
        var rulesetDefender = defender.RulesetActor;

        if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
            !rulesetDefender.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect,
                "ConditionInventorFlashOfGeniusAura",
                out var activeCondition) ||
            activeCondition.SourceGuid != helper.Guid)
        {
            yield break;
        }

        var rulesetHelper = helper.RulesetCharacter;
        var intelligence = rulesetHelper.TryGetAttributeValue(AttributeDefinitions.Intelligence);
        var bonus = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);

        if (abilityCheckData.AbilityCheckRoll == 0 ||
            abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
            !helper.CanReact() ||
            !helper.CanPerceiveTarget(defender) ||
            rulesetHelper.GetRemainingPowerUses(power) == 0 ||
            abilityCheckData.AbilityCheckSuccessDelta + bonus < 0)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(power, rulesetHelper);
        var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
        {
            StringParameter = "InventorFlashOfGeniusCheck",
            StringParameter2 = "SpendPowerInventorFlashOfGeniusCheckDescription".Formatted(
                Category.Reaction, defender.Name, helper.Name),
            RulesetEffect = implementationManager
                .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
            UsablePower = usablePower
        };
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToSpendPower(reactionParams);

        yield return battleManager.WaitForReactions(defender, actionService, count);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        abilityCheckData.AbilityCheckRoll += bonus;
        abilityCheckData.AbilityCheckSuccessDelta += bonus;

        if (abilityCheckData.AbilityCheckSuccessDelta >= 0)
        {
            abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
        }

        var extra = abilityCheckData.AbilityCheckSuccessDelta >= 0
            ? (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle")
            : (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");

        helper.RulesetCharacter.LogCharacterUsedPower(
            power,
            "Feedback/&FlashOfGeniusCheckToHitRoll",
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, bonus.ToString()),
                extra
            ]);
    }

    public IEnumerator OnTryAlterOutcomeSavingThrow(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck)
    {
        var rulesetDefender = defender.RulesetActor;

        if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
            !rulesetDefender.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect,
                "ConditionInventorFlashOfGeniusAura",
                out var activeCondition) ||
            activeCondition.SourceGuid != helper.Guid)
        {
            yield break;
        }

        var rulesetHelper = helper.RulesetCharacter;
        var intelligence = rulesetHelper.TryGetAttributeValue(AttributeDefinitions.Intelligence);
        var bonus = Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);

        if (!action.RolledSaveThrow ||
            action.SaveOutcome != RollOutcome.Failure ||
            !helper.CanReact() ||
            !helper.CanPerceiveTarget(defender) ||
            rulesetHelper.GetRemainingPowerUses(power) == 0 ||
            action.SaveOutcomeDelta + bonus < 0)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(power, rulesetHelper);
        var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
        {
            StringParameter = "InventorFlashOfGenius",
            StringParameter2 = FormatReactionDescription(action, attacker, defender, helper),
            RulesetEffect = implementationManager
                .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
            UsablePower = usablePower
        };
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToSpendPower(reactionParams);

        yield return battleManager.WaitForReactions(attacker, actionService, count);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        action.SaveOutcomeDelta += bonus;

        if (action.SaveOutcomeDelta >= 0)
        {
            action.SaveOutcome = RollOutcome.Success;
        }

        var extra = action.SaveOutcomeDelta >= 0
            ? (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle")
            : (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");

        helper.RulesetCharacter.LogCharacterUsedPower(
            power,
            "Feedback/&FlashOfGeniusSavingToHitRoll",
            extra:
            [
                (ConsoleStyleDuplet.ParameterType.Positive, bonus.ToString()),
                extra
            ]);
    }

    private static string FormatReactionDescription(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper)
    {
        var text = defender == helper ? "Self" : "Ally";

        return $"SpendPowerInventorFlashOfGeniusReactDescription{text}"
            .Formatted(Category.Reaction, defender.Name, attacker.Name, action.FormatTitle());
    }
}
