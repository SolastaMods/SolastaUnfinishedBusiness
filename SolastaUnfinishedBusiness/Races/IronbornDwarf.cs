using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceIronbornDwarfBuilder
{
    internal static CharacterRaceDefinition SubraceIronbornDwarf { get; } = BuildIronbornDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildIronbornDwarf()
    {
        var ironbornDwarfSpriteReference = Sprites.GetSprite("IronbornDwarf", Resources.IronbornDwarf, 1024, 512);

        var attributeModifierIronbornDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierIronbornDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var proficiencyIronbornDwarfArmorTraining = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyIronbornDwarfArmorTraining")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.LightArmorCategory)
            .AddToDB();

        var ironbornDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        ironbornDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        ironbornDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;

        // needs dwarven languages

        var raceIronbornDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceIronbornDwarf")
            .SetGuiPresentation(Category.Race, ironbornDwarfSpriteReference)
            .SetRacePresentation(ironbornDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierIronbornDwarfStrengthAbilityScoreIncrease,
                proficiencyIronbornDwarfArmorTraining,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionSenses.SenseNormalVision)
            .AddToDB();

        // using avg heights from PHB, scale factor is 53/49, or about 1.08
        RacesContext.RaceScaleMap[raceIronbornDwarf] =
            5.3f / 4.9f; // Not sure why this is displayed as a ratio for other races

        Dwarf.SubRaces.Add(raceIronbornDwarf);

        return raceIronbornDwarf;
    }
}
