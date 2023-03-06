using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class InventorClass
{
    public const string ClassName = "Inventor";
    private const string InfusionsName = "FeatureInventorInfusionPool";
    private const string LimiterName = "Infusion";

    private static readonly AssetReferenceSprite Sprite =
        Sprites.GetSprite("Inventor", Resources.Inventor, 1024, 576);

    internal static readonly AssetReferenceSprite Pictogram =
        Sprites.GetSprite("InventorPictogram", Resources.InventorPictogram, 128);

    private static SpellListDefinition _spellList;
    public static readonly LimitEffectInstances InfusionLimiter = new(LimiterName, GetInfusionLimit);

    private static FeatureDefinitionCustomInvocationPool _learn2, _learn4, _unlearn;
    private static int _infusionPoolIncreases;

    internal static CharacterClassDefinition Class { get; private set; }

    public static FeatureDefinitionPower InfusionPool { get; private set; }
    public static SpellListDefinition SpellList => _spellList ??= BuildSpellList();

    private static FeatureDefinitionCastSpell SpellCasting { get; set; }

    public static CharacterClassDefinition Build()
    {
        if (Class != null)
        {
            throw new ArgumentException("Trying to build Inventor class additional time.");
        }

        InfusionPool = BuildInfusionPool();
        SpellCasting = BuildSpellCasting();

        _learn2 = BuildLearn(2);
        _learn4 = BuildLearn(4);
        _unlearn = BuildUnlearn();

        Infusions.Build();

        var builder = CharacterClassDefinitionBuilder
            .Create(ClassName)

            #region Presentation

            .SetGuiPresentation(Category.Class, Sprite)
            .SetAnimationId(AnimationDefinitions.ClassAnimationId.Fighter)
            .SetPictogram(Pictogram);

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
                AttributeDefinitions.Charisma
            )
            .AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.Athletics,
                DatabaseHelper.SkillDefinitions.History,
                DatabaseHelper.SkillDefinitions.Insight,
                DatabaseHelper.SkillDefinitions.Stealth,
                DatabaseHelper.SkillDefinitions.Religion,
                DatabaseHelper.SkillDefinitions.Perception,
                DatabaseHelper.SkillDefinitions.Survival
            )
            .AddToolPreferences(
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.ScrollKitType
            )
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
                }
            )
            .AddEquipmentRow(
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow,
                    EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt,
                    EquipmentDefinitions.OptionAmmoPack, 1)
            )
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
                    AttributeDefinitions.Constitution)
                .AddToDB())

            // Tools Proficiency
            .AddFeaturesAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyInventorTools")
                .SetGuiPresentation(Category.Feature, "Feature/&ToolProficiencyPluralDescription")
                .SetProficiencies(ProficiencyType.Tool,
                    ToolTypeDefinitions.ThievesToolsType.Name,
                    ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
                .AddToDB())

            // Tool Selection
            .AddFeaturesAtLevel(1, FeatureDefinitionPointPoolBuilder
                .Create("PointPoolInventorTools")
                .SetGuiPresentation(Category.Feature, "Feature/&ToolGainChoicesSingleDescription")
                .SetPool(HeroDefinitions.PointsPoolType.Tool, 1)
                .OnlyUniqueChoices()
                .RestrictChoices(
                    ToolTypeDefinitions.DisguiseKitType.Name,
                    ToolTypeDefinitions.EnchantingToolType.Name,
                    ToolTypeDefinitions.HerbalismKitType.Name,
                    ToolTypeDefinitions.PoisonersKitType.Name,
                    ToolTypeDefinitions.ScrollKitType.Name
                )
                .AddToDB())

            // Skill points
            .AddFeaturesAtLevel(1, FeatureDefinitionPointPoolBuilder
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
                    SkillDefinitions.SleightOfHand
                )
                .AddToDB())

            #endregion

            #region Level 01

            .AddFeaturesAtLevel(1, SpellCasting, BuildBonusCantrips(), BuildRitualCasting())

            #endregion

            #region Level 02

            .AddFeaturesAtLevel(2, BuildInfuseFeatureSet())

            #endregion

            #region Level 03

            .AddFeaturesAtLevel(3, BuildRightToolForTheJob())

            #endregion

            #region Level 04

            .AddFeaturesAtLevel(4,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 05

            #endregion

            #region Level 06

            .AddFeaturesAtLevel(6, _learn2, BuildInfusionPoolIncrease(), BuildToolExpertise())

            #endregion

            #region Level 07

            .AddFeaturesAtLevel(7, BuildFlashOfGenius())

            #endregion

            #region Level 08

            .AddFeaturesAtLevel(8,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 09

            #endregion

            #region Level 10

            .AddFeaturesAtLevel(10, _learn2, BuildMagicAdept(),
                BuildInfusionPoolIncrease())

            #endregion

            #region Level 11

            .AddFeaturesAtLevel(11, BuildSpellStoringItem())

            #endregion

            #region Level 12

            .AddFeaturesAtLevel(12,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 13

            #endregion

            #region Level 14

            .AddFeaturesAtLevel(14, _learn2, BuildInfusionPoolIncrease(), BuildMagicItemSavant())

            #endregion

            #region Level 15

            #endregion

            #region Level 16

            .AddFeaturesAtLevel(16,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 17

            #endregion

            #region Level 18

            .AddFeaturesAtLevel(18, _learn2, BuildInfusionPoolIncrease())

            #endregion

            #region Level 19

            .AddFeaturesAtLevel(19,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            );

        #endregion

        #region Level 20

        #endregion

        for (var i = 3; i <= 20; i++)
        {
            builder.AddFeaturesAtLevel(i, _unlearn);
        }

        Class = builder.AddToDB();

        #region Subclasses

        builder.AddFeaturesAtLevel(3, FeatureDefinitionSubclassChoiceBuilder
            .Create("SubclassChoiceInventor")
            .SetGuiPresentation("InventorInnovation", Category.Subclass)
            .SetSubclassSuffix("InventorInnovation")
            .SetFilterByDeity(false)
            .SetSubclasses(
                InnovationArmor.Build(),
                InnovationAlchemy.Build(),
                InnovationWeapon.Build()
            )
            .AddToDB());

        #endregion

        BuildCancelAllInfusionsRestActivity();

        return Class;
    }

    private static FeatureDefinition BuildToolExpertise()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInventorToolExpertise")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ArtisanToolSmithToolsType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name,
                ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name,
                ToolTypeDefinitions.ScrollKitType.Name
            )
            .AddToDB();
    }

    private static FeatureDefinition BuildRightToolForTheJob()
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
                ToolTypeDefinitions.ScrollKitType.Name
            )
            .AddToDB();
    }

    private static FeatureDefinitionCustomInvocationPool BuildLearn(int points)
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolInfusionLearn" + points)
            .SetGuiPresentation(Category.Feature)
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

    private static FeatureDefinition BuildInfuseFeatureSet()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorInfusions")
            .SetGuiPresentation(InfusionsName, Category.Feature)
            .AddFeatureSet(InfusionPool, _learn4)
            .AddToDB();
    }

    //TODO: rework to be 1 feature
    private static FeatureDefinition BuildInfusionPoolIncrease()
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
                SpellDefinitions.SpareTheDying
            )
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
                SpellDefinitions.Longstrider
            )
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
                SpellDefinitions.SpiderClimb
            )
            // blink, elemental weapon, flame arrows
            .SetSpellsAtLevel(3,
                SpellDefinitions.CreateFood,
                SpellDefinitions.DispelMagic,
                SpellDefinitions.Fly,
                SpellDefinitions.Haste,
                SpellDefinitions.ProtectionFromEnergy,
                SpellDefinitions.Revivify
            )
            // everything
            .SetSpellsAtLevel(4,
                SpellDefinitions.FreedomOfMovement,
                SpellDefinitions.Stoneskin
            )
            // everything
            .SetSpellsAtLevel(5,
                SpellDefinitions.GreaterRestoration
            )
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

    private static FeatureDefinition BuildRitualCasting()
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
                SpellDefinitions.Sparkle
            )
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildInfusionPool()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerInfusionPool")
            .SetGuiPresentation(InfusionsName, Category.Feature)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 2)
            .AddToDB();
    }

    private static void BuildCancelAllInfusionsRestActivity()
    {
        const string POWER_NAME = "PowerAfterRestStopInfusions";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestStopInfusions")
            .SetGuiPresentation(POWER_NAME, Category.Feature)
            .SetCustomSubFeatures(new RestActivityValidationParams(false, false))
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
            .SetCustomSubFeatures(new RestActivityValidationParams(false, false))
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
            .SetCustomSubFeatures(
                new HasActiveInfusions(),
                new LimitEffectInstances(LimiterName, _ => 1))
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.Instantaneous)
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildMagicAdept()
    {
        return FeatureDefinitionCraftingAffinityBuilder
            .Create("CraftingAffinityInventorMagicItemAdept")
            .SetGuiPresentation(Category.Feature)
            //increases attunement limit by 1
            .SetCustomSubFeatures(new AttunementLimitModifier(1))
            .SetAffinityGroups(0.25f, false,
                ToolTypeDefinitions.ThievesToolsType,
                ToolTypeDefinitions.ScrollKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType)
            .AddToDB();
    }

    private static FeatureDefinition BuildMagicItemSavant()
    {
        return FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityInventorMagicItemSavant")
            .SetGuiPresentation(Category.Feature)
            //increases attunement limit by 1
            .SetCustomSubFeatures(new AttunementLimitModifier(1))
            .IgnoreClassRestrictionsOnMagicalItems()
            .AddToDB();
    }

    private static FeatureDefinition BuildSpellStoringItem()
    {
        var master = FeatureDefinitionPowerBuilder
            .Create("PowerInventorSpellStoringItem")
            .SetGuiPresentation(Category.Feature, ItemDefinitions.WandMagicMissile)
            .SetUsesFixed(
                ActivationTime.Action,
                RechargeRate.LongRest)
            .AddToDB();

        var powers = SpellList
            .GetSpellsOfLevels(1, 2)
            .Where(x => x.castingTime == ActivationTime.Action)
            .Select(spell => BuildCreateSpellStoringItemPower(BuildWandOfSpell(spell), spell, master))
            .Cast<FeatureDefinitionPower>()
            .ToArray();

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.InventorSpellStoringItem, powers);
        PowerBundle.RegisterPowerBundle(master, true, powers);

        return master;
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
            .SetCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.Bless)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetSummonItemForm(item, 1, true)
                    .Build())
                .Build())
            .AddToDB();

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
            .SetCustomSubFeatures(InventorClassHolder.Marker)
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
                    .Build()
                )
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildFlashOfGenius()
    {
        const string TEXT = "PowerInventorFlashOfGenius";
        var sprite = Sprites.GetSprite("InventorQuickWit", Resources.InventorQuickWit, 256, 128);

        var bonusPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusBonus")
            .SetGuiPresentation(TEXT, Category.Feature, sprite)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetCustomSubFeatures(PowerVisibilityModifier.Visible)
            .SetReactionContext(ReactionTriggerContext.None)
            .AddToDB();

        //should be hidden from user
        var flashOfGenius = new FlashOfGenius(bonusPower, "InventorFlashOfGenius");

        var auraPower = FeatureDefinitionPowerBuilder
            .Create("PowerInventorFlashOfGeniusAura")
            .SetGuiPresentation(TEXT, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                .SetDurationData(DurationType.Permanent)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionInventorFlashOfGeniusAura")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetCustomSubFeatures(flashOfGenius)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorFlashOfGenius")
            .SetGuiPresentation(TEXT, Category.Feature)
            .AddFeatureSet(auraPower, bonusPower)
            .AddToDB();
    }

    private class HasActiveInfusions : IPowerUseValidity
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            return character.PowersUsedByMe.FirstOrDefault(p =>
                p.PowerDefinition.GetFirstSubFeatureOfType<LimitEffectInstances>()?.Name == LimiterName) != null;
        }
    }
}

internal class InventorClassHolder : IClassHoldingFeature
{
    private InventorClassHolder()
    {
    }

    public static InventorClassHolder Marker { get; } = new();

    public CharacterClassDefinition Class => InventorClass.Class;
}

internal class FlashOfGenius : ConditionSourceCanUsePowerToImproveFailedSaveRoll
{
    internal FlashOfGenius(FeatureDefinitionPower power, string reactionName) : base(power, reactionName)
    {
    }

    private static int GetBonus(RulesetEntity helper)
    {
        var intelligence = helper.TryGetAttributeValue(AttributeDefinitions.Intelligence);

        return Math.Max(AttributeDefinitions.ComputeAbilityScoreModifier(intelligence), 1);
    }

    internal override bool ShouldTrigger(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        if (helper.IsOppositeSide(defender.Side))
        {
            return false;
        }

        if (helper.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) !=
            ActionDefinitions.ActionStatus.Available)
        {
            return false;
        }

        return action.RolledSaveThrow && saveOutcomeDelta + GetBonus(helper.RulesetActor) >= 0;
    }

    internal override bool TryModifyRoll(CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        ref RollOutcome saveOutcome,
        ref int saveOutcomeDelta)
    {
        var bonus = GetBonus(helper.RulesetActor);

        saveOutcomeDelta += bonus;

        //reuse DC modifier from previous checks, not 100% sure this is correct
        var saveDc = action.GetSaveDC() + saveModifier.SaveDCModifier;
        var rolled = saveDc + saveOutcomeDelta;
        var success = saveOutcomeDelta >= 0;

        const string TEXT = "Feedback/&CharacterGivesBonusToSaveWithDCFormat";
        string result;
        ConsoleStyleDuplet.ParameterType resultType;

        if (success)
        {
            result = GameConsole.SaveSuccessOutcome;
            resultType = ConsoleStyleDuplet.ParameterType.SuccessfulRoll;
            saveOutcome = RollOutcome.Success;
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

    internal override string FormatReactionDescription(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        var text = defender == helper
            ? "Reaction/&SpendPowerInventorFlashOfGeniusReactDescriptionSelfFormat"
            : "Reaction/&SpendPowerInventorFlashOfGeniusReactAllyDescriptionAllyFormat";

        return Gui.Format(text, defender.Name, attacker.Name, action.FormatTitle());
    }
}
