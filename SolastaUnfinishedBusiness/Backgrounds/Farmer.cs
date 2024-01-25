using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SkillDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Backgrounds;

internal static partial class BackgroundsBuilders
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
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
                            (OperationType.Set,
                                ValidatorsWeapon.IsOfWeaponType(ClubType, HandaxeType)(mode, null, null))))
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundFarmer}Weapons")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Weapon, ClubType.Name, HandaxeType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundFarmer}Skills")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Skill,
                        AnimalHandling,
                        Nature,
                        Perception)
                    .AddToDB())
            .AddDefaultOptionalPersonality("Pragmatism")
            .AddDefaultOptionalPersonality("Friendliness")
            .AddOptionalPersonality("Self-Preservation", 8)
            .AddOptionalPersonality("Selfishness", 8)
            .AddOptionalPersonality("Pragmatism", 8)
            .AddOptionalPersonality("Friendliness", 8)
            .AddStaticPersonality("Slang", 30)
            .AddStaticPersonality("Normal", 5)
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ClothesCommon, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(Handaxe, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(Torch, EquipmentDefinitions.OptionGenericItem, 1),
                EquipmentOptionsBuilder.Option(Food_Ration, EquipmentDefinitions.OptionGenericItem, 5)
            })
            .AddToDB();
    }
}
