using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal class MonkShieldExpert : AbstractFightingStyle
{
    internal const string ShieldExpertName = "MonkShieldExpert";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(ShieldExpertName)
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite(ShieldExpertName, Resources.ShieldExpert, 256))
        .SetFeatures(
            FeatureDefinitionProficiencyBuilder
                .Create($"Proficiency{ShieldExpertName}")
                .SetGuiPresentation("ShieldCategory", Category.Equipment)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Armor, EquipmentDefinitions.ShieldCategory)
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice =>
        [CharacterContext.FightingStyleChoiceMonk];
}
