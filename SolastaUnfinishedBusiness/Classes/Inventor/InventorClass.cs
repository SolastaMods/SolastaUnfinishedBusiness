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
    public static readonly LimitedEffectInstances InfusionLimiter = new("Infusion", _ => 2);

    private static CustomInvocationPoolDefinition Learn2, Learn4, Unlearn;

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

        Learn2 = BuildLearn(2);
        Learn4 = BuildLearn(4);
        Unlearn = BuildUnlearn();

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

            .AddFeaturesAtLevel(1, SpellCasting)

            #endregion

            #region Level 02

            .AddFeaturesAtLevel(2, Learn4, InfusionPool)

            #endregion

            #region Level 03

            #endregion

            #region Level 04

            .AddFeaturesAtLevel(4,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 05

            #endregion

            #region Level 06

            .AddFeaturesAtLevel(6, Learn2)

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

            .AddFeaturesAtLevel(10, Learn2)

            #endregion

            #region Level 11

            #endregion

            #region Level 12

            .AddFeaturesAtLevel(12,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice
            )

            #endregion

            #region Level 13

            #endregion

            #region Level 14

            .AddFeaturesAtLevel(14, Learn2)

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

            .AddFeaturesAtLevel(18, Learn2)

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
            builder.AddFeaturesAtLevel(i, Unlearn);
        }

        Class = builder.AddToDB();

        return Class;
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
            .Create("TestInvocationPoolReplace")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Fly)
            .Setup(CustomInvocationPoolType.Pools.Infusion, 1, true)
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
}
