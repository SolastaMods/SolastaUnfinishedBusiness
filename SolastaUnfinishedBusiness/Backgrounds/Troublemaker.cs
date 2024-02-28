using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SkillDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Backgrounds;

internal static partial class BackgroundsBuilders
{
    private const string BackgroundTroublemaker = "BackgroundTroublemaker";

    internal static CharacterBackgroundDefinition BuildBackgroundTroublemaker()
    {
        return CharacterBackgroundDefinitionBuilder
            .Create(BackgroundTroublemaker)
            .SetGuiPresentation(Category.Background,
                Sprites.GetSprite(BackgroundTroublemaker, Resources.BackgroundTroublemaker, 1024, 512))
            .SetBanterList(BanterDefinitions.BanterList.Formal)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundTroublemaker}Weapons")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Weapon, RapierType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundTroublemaker}Skills")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Skill,
                        Athletics,
                        Deception,
                        Intimidation)
                    .AddToDB(),
                DatabaseHelper.FeatureDefinitionPointPools.PointPoolBackgroundLanguageChoice_one)
            .AddDefaultOptionalPersonality("Pragmatism")
            .AddDefaultOptionalPersonality("Greed")
            .AddOptionalPersonality("Cynicism", 8)
            .AddOptionalPersonality("Selfishness", 8)
            .AddOptionalPersonality("Pragmatism", 8)
            .AddOptionalPersonality("Greed", 8)
            .AddStaticPersonality("Slang", 30)
            .AddStaticPersonality("Normal", 5)
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ClothesNoble_Valley_Red, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(Rapier, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(_10_Gold_Coins, EquipmentDefinitions.OptionGenericItem, 1)
            })
            .AddToDB();
    }
}
