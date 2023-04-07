using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Backgrounds;

internal static class BackgroundsBuilders
{
    private const string BackgroundFarmer = "BackgroundFarmer";

    internal static CharacterBackgroundDefinition BuildBackgroundFarmer()
    {
        return CharacterBackgroundDefinitionBuilder
            .Create(BackgroundFarmer)
            .SetGuiPresentation(Category.Background,
                Sprites.GetSprite(BackgroundFarmer, Resources.BackgroundFarmer, 1024, 512))
            .SetBanterList(BanterDefinitions.BanterList.Serious)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"AttackModifier{BackgroundFarmer}")
                    .SetGuiPresentation($"Proficiency{BackgroundFarmer}Weapons", Category.Feature, hidden: true)
                    .SetAttackRollModifier(1)
                    .SetCustomSubFeatures(
                        new RestrictedContextValidator((_, _, _, _, _, mode, _) =>
                            (OperationType.Set,
                                ValidatorsWeapon.IsOfWeaponType(ClubType, HandaxeType)(mode, null, null))))
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundFarmer}Weapons")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon, ClubType.Name, HandaxeType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundFarmer}Skills")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(RuleDefinitions.ProficiencyType.Skill,
                        SkillDefinitions.AnimalHandling,
                        SkillDefinitions.Nature,
                        SkillDefinitions.Perception)
                    .AddToDB())
            .AddOptionalPersonality("Selfishness", 8)
            .AddOptionalPersonality("Pragmatism", 8)
            .AddOptionalPersonality("Friendliness", 8)
            .AddOptionalPersonality("Helpfulness", 8)
            .AddStaticPersonality("Slang", 30)
            .AddStaticPersonality("Normal", 5)
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ClothesCommon,
                    EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Handaxe,
                    EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Torch,
                    EquipmentDefinitions.OptionGenericItem, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Food_Ration,
                    EquipmentDefinitions.OptionGenericItem, 5)
            })
            .AddToDB();
    }
}
