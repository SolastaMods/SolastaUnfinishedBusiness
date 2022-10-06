using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class InventorClass
{
    public const string ClassName = "Inventor";

    private static readonly AssetReferenceSprite Sprite =
        CustomIcons.CreateAssetReferenceSprite("Inventor", Resources.Inventor, 1024, 576);

    private static readonly AssetReferenceSprite Pictogram =
        Wizard.ClassPictogramReference;

    private static SpellListDefinition _spellList;
    public static readonly LimitedEffectInstances InfusionLimiter = new("Infusion", GetInfusionLimit);

    private static CustomInvocationPoolDefinition _learn2, _learn4, _unlearn;
    private static int infusionPoolIncreases;

    internal static CharacterClassDefinition Class { get; set; }

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

        _learn2 = BuildLearn(2);
        _learn4 = BuildLearn(4);
        _unlearn = BuildUnlearn();

        Infusions.Build();

        var builder = CharacterClassDefinitionBuilder
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
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.ScrollKitType
            )
            //TODO: Add more preferred feats
            .AddFeatPreferences(EwFeats.WarcasterFeat)

            #endregion

            .SetBattleAI(DecisionPackageDefinitions.DefaultMeleeWithBackupRangeDecisions)
            .SetIngredientGatheringOdds(Fighter.IngredientGatheringOdds)
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
                    ToolTypeDefinitions.ThievesToolsType,
                    ToolTypeDefinitions.ArtisanToolSmithToolsType)
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
                ).AddToDB())

            #endregion

            #region Subclasses

            .AddFeaturesAtLevel(3, FeatureDefinitionSubclassChoiceBuilder
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

            .AddFeaturesAtLevel(1, SpellCasting, BuildBonusCantrips())

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

            #endregion

            #region Level 08

            .AddFeaturesAtLevel(8,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 09

            #endregion

            #region Level 10

            .AddFeaturesAtLevel(10, _learn2, Infusions.ImprovedInfusions, BuildMagicAdept(),
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

        // Inventor appears after Fighter
        Class.GuiPresentation.sortOrder = Fighter.GuiPresentation.sortOrder + 1;

        return Class;
    }

    private static FeatureDefinition BuildToolExpertise()
    {
        return FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInventorToolExpertise")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ArtisanToolSmithToolsType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.ScrollKitType
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
                ToolTypeDefinitions.DisguiseKitType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.ScrollKitType
            )
            .AddToDB();
    }

    private static CustomInvocationPoolDefinition BuildLearn(int points)
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolInfusionLearn" + points)
            .SetGuiPresentation(Category.Feature)
            .Setup(CustomInvocationPoolType.Pools.Infusion, points)
            .AddToDB();
    }

    private static CustomInvocationPoolDefinition BuildUnlearn()
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolInventorUnlearnInfusion")
            .SetGuiPresentationNoContent(true)
            .Setup(CustomInvocationPoolType.Pools.Infusion, 1, true)
            .AddToDB();
    }

    private static FeatureDefinition BuildInfuseFeatureSet()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorInfusions")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(InfusionPool, _learn4)
            .AddToDB();
    }

    private static FeatureDefinition BuildInfusionPoolIncrease()
    {
        return FeatureDefinitionPowerPoolModifierBuilder
            .Create($"PowerIncreaseInventorInfusionPool{infusionPoolIncreases++:D2}")
            .SetGuiPresentation("PowerIncreaseInventorInfusionPool", Category.Feature)
            .Configure(1, UsesDetermination.Fixed, "", InfusionPool)
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
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(2)
            .SetRechargeRate(RechargeRate.LongRest)
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
            .SetActivationTime(ActivationTime.Action)
            .SetUsesFixed(1)
            .SetCostPerUse(1)
            .SetRechargeRate(RechargeRate.LongRest)
            .AddToDB();

        var powers = new List<FeatureDefinitionPower>();
        foreach (var spell in SpellList.GetSpellsOfLevels(1, 2))
        {
            if (spell.castingTime != ActivationTime.Action) { continue; }

            var power = BuildCreateSpellStoringItemPower(BuildWandOfSpell(spell), spell, master);
            powers.Add(power);
        }

        GlobalUniqueEffects.AddToGroup(GlobalUniqueEffects.Group.InventorSpellStoringItem, powers);
        PowersBundleContext.RegisterPowerBundle(master, true, powers);

        return master;
    }

    public static FeatureDefinitionPowerSharedPool BuildCreateSpellStoringItemPower(ItemDefinition item,
        SpellDefinition spell, FeatureDefinitionPower pool)
    {
        var description = Gui.Format("Item/&CreateSpellStoringWandFormatDescription", spell.FormatTitle(),
            Gui.ToRoman(spell.spellLevel));

        var power = FeatureDefinitionPowerSharedPoolBuilder.Create($"PowerCreate{item.name}")
            .SetGuiPresentation(spell.FormatTitle(), description, spell)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetSharedPool(pool)
            .SetUniqueInstance()
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker, SkipEffectRemovalOnLocationChange.Always)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetAnimation(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.Bless)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetSummonItemForm(item, 1, true)
                    .Build())
                .Build())
            .AddToDB();

        return power;
    }

    private static ItemDefinition BuildWandOfSpell(SpellDefinition spell)
    {
        return ItemDefinitionBuilder
            .Create(ItemDefinitions.WandMagicMissile, $"SpellStoringWandOf{spell.Name}")
            .SetOrUpdateGuiPresentation($"Wand of {spell.FormatTitle()}",
                $"This wand allows casting of the <b>{spell.Name}</b> spell using spell casting stats of the Inventor who created it.")
            .SetRequiresIdentification(false)
            .HideFromDungeonEditor()
            .SetCustomSubFeatures(InventorClassHolder.Marker)
            .SetUsableDeviceDescription(new UsableDeviceDescriptionBuilder()
                .SetUsage(EquipmentDefinitions.ItemUsage.Charges)
                .SetChargesCapitalNumber(6) //TODO: try to make this based off Inventor's INT bonus x2
                .SetOutOfChargesConsequence(EquipmentDefinitions.ItemOutOfCharges.Destroy)
                .SetRecharge(RechargeRate.None)
                .SetSaveDc(EffectHelpers.BASED_ON_ITEM_SUMMONER)
                .SetMagicAttackBonus(EffectHelpers.BASED_ON_ITEM_SUMMONER)
                .AddFunctions(new DeviceFunctionDescriptionBuilder()
                    .SetUsage(useAmount: 1, useAffinity: DeviceFunctionDescription.FunctionUseAffinity.ChargeCost)
                    .SetSpell(spell)
                    .Build()
                )
                .Build())
            .AddToDB();
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
