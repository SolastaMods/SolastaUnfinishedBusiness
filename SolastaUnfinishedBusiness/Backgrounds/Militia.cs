using System.Collections.Generic;
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
    private const string BackgroundMilitia = "BackgroundMilitia";

    internal static CharacterBackgroundDefinition BuildBackgroundMilitia()
    {
        return CharacterBackgroundDefinitionBuilder
            .Create(BackgroundMilitia)
            .SetGuiPresentation(Category.Background,
                Sprites.GetSprite(BackgroundMilitia, Resources.BackgroundMilitia, 1024, 512))
            .SetBanterList(BanterDefinitions.BanterList.Formal)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundMilitia}Armor")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundMilitia}Weapons")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Weapon, ShortswordType.Name, SpearType.Name)
                    .AddToDB(),
                FeatureDefinitionProficiencyBuilder
                    .Create($"Proficiency{BackgroundMilitia}Skills")
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(ProficiencyType.Skill,
                        Perception,
                        Intimidation,
                        Investigation)
                    .AddToDB())
            .AddDefaultOptionalPersonality("Lawfulness")
            .AddDefaultOptionalPersonality("Authority")
            .AddOptionalPersonality("Lawfulness", 8)
            .AddOptionalPersonality("Authority", 8)
            .AddOptionalPersonality("Helpfulness", 8)
            .AddOptionalPersonality("Self-Preservation", 8)
            .AddStaticPersonality("Slang", 30)
            .AddStaticPersonality("Normal", 5)
            .AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(Torch, EquipmentDefinitions.OptionGenericItem, 3),
                EquipmentOptionsBuilder.Option(_10_Gold_Coins, EquipmentDefinitions.OptionGenericItem, 1)
            })
            .AddToDB();
    }
}
