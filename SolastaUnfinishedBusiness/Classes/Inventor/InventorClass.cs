using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class InventorClass
{
    private const string ClassName = "Inventor";
    private static CharacterClassDefinition Class { get; set; }

    private static readonly AssetReferenceSprite Sprite =
        CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference;

    private static readonly AssetReferenceSprite Pictogram =
        CharacterClassDefinitions.Wizard.ClassPictogramReference;

    public static CharacterClassDefinition Build()
    {
        if (Class != null)
        {
            throw new ArgumentException("Trying to build Inventor class additional time.");
        }

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

            //TODO: set proper equipment

            #region Equipment

            .AddEquipmentRow(
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Club,
                        EquipmentDefinitions.OptionWeapon, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger,
                        EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
                }
            )
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Leather,
                        EquipmentDefinitions.OptionArmor, 1)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.HideArmor,
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
                }
            )
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dart,
                        EquipmentDefinitions.OptionWeapon, 10)
                },
                new List<CharacterClassDefinition.HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Javelin,
                        EquipmentDefinitions.OptionWeapon, 5)
                })

            #endregion

            #region Proficiencies

            // Weapons
            .AddFeatureAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ClassInventorWeaponProficiency")
                .SetGuiPresentation(Category.Feature, "Feature/&WeaponTrainingShortDescription")
                .SetProficiencies(ProficiencyType.Weapon, WeaponCategoryDefinitions.SimpleWeaponCategory.Name)
                .AddToDB())

            // Saves
            .AddFeatureAtLevel(1, FeatureDefinitionProficiencyBuilder
                .Create("ClassInventorSavingThrowProficiency")
                .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
                .SetProficiencies(ProficiencyType.SavingThrow,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Dexterity)
                .AddToDB())

            // Skill points
            .AddFeatureAtLevel(1, FeatureDefinitionPointPoolBuilder
                .Create("ClassInventorSkillProficiency")
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

            .AddFeatureAtLevel(1, FeatureDefinitionSubclassChoiceBuilder
                .Create("InventorSubclassChoice")
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

            #endregion

            .AddToDB();


        return Class;
    }
}