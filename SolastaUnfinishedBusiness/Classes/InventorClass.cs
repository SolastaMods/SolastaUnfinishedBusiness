using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
    private static readonly List<FeatureDefinitionPowerSharedPool> SpellStoringItemPowers = [];

    private static readonly FeatureDefinitionPower PowerInventorSpellStoringItem = FeatureDefinitionPowerBuilder
        .Create("PowerInventorSpellStoringItem")
        .SetGuiPresentation(Category.Feature, ItemDefinitions.WandMagicMissile)
        .SetUsesFixed(
            ActivationTime.Action,
            RechargeRate.LongRest)
        .AddToDB();

    private static readonly int[] Costs = [0, 0, 0, 0, 0];

    private static FeatureDefinitionCastSpell SpellCasting => _spellCasting ??= BuildSpellCasting();

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
                Sprites.GetSprite("Inventor", Resources.Inventor, 1024, 576),
                hidden: true)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter)
            .SetPictogram(Sprites.GetSprite("InventorPictogram", Resources.InventorPictogram, 128));

        Wizard.personalityFlagOccurences
            .ForEach(fo => builder.AddPersonality(fo.personalityFlag, fo.weight));

        #endregion

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
            );

        #endregion

        for (var i = 3; i <= 20; i++)
        {
            builder.AddFeaturesAtLevel(i, unlearn);
        }

        builder.AddFeaturesAtLevel(3, SubclassChoice);

        BuildCancelAllInfusionsRestActivity();

        RegisterPoILoot();

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
                ToolTypeDefinitions.ArtisanToolSmithToolsType.Name,
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

    //TODO: rework to be 1 feature
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
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                IsModifyPowerPool.Marker,
                HasModifiedUses.Marker)
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

        var power = SpellStoringItemPowers.FirstOrDefault(x => x.SourceDefinition == spell);

        // Main.Enabled as during initialization the powers weren't registered yet
        if (Main.Enabled && power == null)
        {
            Main.Error("found a null power when trying to switch a spell storing item");
        }

        var subPowers = PowerInventorSpellStoringItem.GetBundle()?.SubPowers;

        if (active)
        {
            subPowers?.TryAdd(power);
        }
        else
        {
            subPowers?.Remove(power);
        }
    }

    internal static void LateLoadSpellStoringItem()
    {
        Class.FeatureUnlocks.Add(new FeatureUnlockByLevel(BuildSpellStoringItem(), 11));
        Class.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static FeatureDefinitionPower BuildSpellStoringItem()
    {
        var spells = SpellsContext.Spells
            .Where(x => x.SpellLevel is 1 or 2 && x.castingTime == ActivationTime.Action);

        // build powers for all level 1 and 2 spells to allow better integration with custom spells selection
        SpellStoringItemPowers.AddRange(spells
            .Select(spell =>
                BuildCreateSpellStoringItemPower(BuildWandOfSpell(spell), spell, PowerInventorSpellStoringItem)));

        // only register the ones indeed in the inventor spell list
        var inventorPowers = SpellStoringItemPowers
            .Where(x => SpellList
                .ContainsSpell(x.SourceDefinition as SpellDefinition))
            .Cast<FeatureDefinitionPower>()
            .ToArray();

        PowerBundle.RegisterPowerBundle(PowerInventorSpellStoringItem, true, inventorPowers);

        // need this extra step to avoid co-variant array conversion warning
        var baseDefinitions = new List<BaseDefinition>();

        baseDefinitions.AddRange(inventorPowers);
        ForceGlobalUniqueEffects.AddToGroup(ForceGlobalUniqueEffects.Group.InventorSpellStoringItem,
            [.. baseDefinitions]);

        return PowerInventorSpellStoringItem;
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
            .AddCustomSubFeatures(InventorModifyAdditionalDamageClassLevelHolder.Marker)
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
        const string TEXT = "PowerInventorFlashOfGenius";
        var sprite = Sprites.GetSprite("InventorQuickWit", Resources.InventorQuickWit, 256, 128);

        var bonusPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusBonus")
            .SetGuiPresentation(TEXT, Category.Feature, sprite)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .AddCustomSubFeatures(ModifyPowerVisibility.Visible)
            .SetReactionContext(ReactionTriggerContext.None)
            .AddToDB();

        //should be hidden from user
        var flashOfGenius = new TryAlterOutcomeFailedSavingThrowFlashOfGenius(
            bonusPower, "InventorFlashOfGenius", "ConditionInventorFlashOfGeniusAura");

        var auraPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusAura")
            .SetGuiPresentation(TEXT, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionInventorFlashOfGeniusAura")
                                    .SetGuiPresentationNoContent(true)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .AddCustomSubFeatures(flashOfGenius)
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorFlashOfGenius")
            .SetGuiPresentation(TEXT, Category.Feature)
            .AddFeatureSet(auraPower, bonusPower)
            .AddToDB();
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

internal class InventorModifyAdditionalDamageClassLevelHolder : IModifyAdditionalDamageClassLevel
{
    private InventorModifyAdditionalDamageClassLevelHolder()
    {
    }

    public static InventorModifyAdditionalDamageClassLevelHolder Marker { get; } = new();

    public CharacterClassDefinition Class => InventorClass.Class;
}

internal class TryAlterOutcomeFailedSavingThrowFlashOfGenius : ITryAlterOutcomeFailedSavingThrow
{
    internal TryAlterOutcomeFailedSavingThrowFlashOfGenius(
        FeatureDefinitionPower power, string reactionName, string auraConditionName)
    {
        Power = power;
        ReactionName = reactionName;
        AuraConditionName = auraConditionName;
    }

    private FeatureDefinitionPower Power { get; }
    private string ReactionName { get; }
    private string AuraConditionName { get; }

    public IEnumerator OnFailedSavingTryAlterOutcome(GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck)
    {
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
        {
            yield break;
        }

        rulesetDefender.TryGetConditionOfCategoryAndType(
            AttributeDefinitions.TagEffect,
            AuraConditionName,
            out var activeCondition);

        if (activeCondition == null)
        {
            yield break;
        }

        RulesetEntity.TryGetEntity<RulesetCharacter>(activeCondition.SourceGuid, out var rulesetOriginalHelper);

        var originalHelper = GameLocationCharacter.GetFromActor(rulesetOriginalHelper);

        if (!ShouldTrigger(action, defender, originalHelper))
        {
            yield break;
        }

        if (!rulesetOriginalHelper.CanUsePower(Power))
        {
            yield break;
        }

        var usablePower = PowerProvider.Get(Power, rulesetOriginalHelper);
        var implementationManagerService =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;
        var reactionParams = new CharacterActionParams(originalHelper, ActionDefinitions.Id.SpendPower)
        {
            StringParameter = ReactionName,
            StringParameter2 = FormatReactionDescription(action, attacker, defender, originalHelper),
            RulesetEffect = implementationManagerService
                //CHECK: no need for AddAsActivePowerToSource
                .MyInstantiateEffectPower(rulesetOriginalHelper, usablePower, false),
            UsablePower = usablePower
        };
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToSpendPower(reactionParams);

        yield return battleManager.WaitForReactions(originalHelper, actionService, count);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        rulesetOriginalHelper.LogCharacterUsedPower(Power, indent: true);
        rulesetOriginalHelper.UsePower(usablePower); // non fixed powers must be explicitly used on custom
        action.RolledSaveThrow = TryModifyRoll(action, originalHelper, saveModifier);
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static int GetBonus(RulesetActor helper)
    {
        var intelligence = helper.TryGetAttributeValue(AttributeDefinitions.Intelligence);

        return Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);
    }

    private static bool ShouldTrigger(
        CharacterAction action,
        GameLocationCharacter defender,
        GameLocationCharacter helper)
    {
        return helper.CanReact()
               && !defender.IsOppositeSide(helper.Side)
               && action.SaveOutcomeDelta + GetBonus(helper.RulesetActor) >= 0;
    }

    private static bool TryModifyRoll(
        CharacterAction action,
        GameLocationCharacter helper,
        ActionModifier saveModifier)
    {
        var bonus = GetBonus(helper.RulesetActor);

        //reuse DC modifier from previous checks, not 100% sure this is correct
        var saveDc = action.GetSaveDC() + saveModifier.SaveDCModifier;
        var rolled = saveDc + action.saveOutcomeDelta + bonus;
        var success = rolled >= saveDc;

        const string TEXT = "Feedback/&CharacterGivesBonusToSaveWithDCFormat";
        string result;
        ConsoleStyleDuplet.ParameterType resultType;

        if (success)
        {
            result = GameConsole.SaveSuccessOutcome;
            resultType = ConsoleStyleDuplet.ParameterType.SuccessfulRoll;
            action.saveOutcome = RollOutcome.Success;
            action.saveOutcomeDelta += bonus;
        }
        else
        {
            result = GameConsole.SaveFailureOutcome;
            resultType = ConsoleStyleDuplet.ParameterType.FailedRoll;
        }

        var console = Gui.Game.GameConsole;
        var entry = new GameConsoleEntry(TEXT, console.consoleTableDefinition) { Indent = true };

        console.AddCharacterEntry(helper.RulesetCharacter, entry);
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, $"+{bonus}");
        entry.AddParameter(resultType, Gui.Format(result, rolled.ToString()));
        entry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo, saveDc.ToString());

        console.AddEntry(entry);

        return true;
    }

    private static string FormatReactionDescription(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper)
    {
        var text = defender == helper
            ? "Reaction/&SpendPowerInventorFlashOfGeniusReactDescriptionSelfFormat"
            : "Reaction/&SpendPowerInventorFlashOfGeniusReactAllyDescriptionAllyFormat";

        return Gui.Format(text, defender.Name, attacker.Name, action.FormatTitle());
    }
}
