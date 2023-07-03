using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceIronbornDwarfBuilder
{
    internal static CharacterRaceDefinition SubraceIronbornDwarf { get; } = BuildIronbornDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildIronbornDwarf()
    {
        var IronbornDwarfSpriteReference = Sprites.GetSprite("IronbornDwarf", Resources.IronbornDwarf, 1024, 512);

        var attributeModifierIronbornDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierIronbornDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var proficiencyIronbornDwarfArmorTraining = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyIronbornDwarfArmorTraining")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.Armor,
                ArmorCategoryDefinitions.MediumArmorCategory.Name,
                ArmorCategoryDefinitions.LightArmorCategory.Name)
            .AddToDB();

        var IronbornDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        IronbornDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        IronbornDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;

        // needs dwarven languages

        var raceIronbornDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceIronbornDwarf")
            .SetGuiPresentation(Category.Race, IronbornDwarfSpriteReference)
            .SetRacePresentation(IronbornDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierIronbornDwarfStrengthAbilityScoreIncrease,
                proficiencyIronbornDwarfArmorTraining,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages
                )
            .AddToDB();

        // using avg heights from PHB, scale factor is 53/49, or about 1.08
        RacesContext.RaceScaleMap[raceIronbornDwarf] = 5.3f / 4.9f; // Not sure why this is displayed as a ratio for other races

        Dwarf.SubRaces.Add(raceIronbornDwarf);

        return raceIronbornDwarf;
    }
}
