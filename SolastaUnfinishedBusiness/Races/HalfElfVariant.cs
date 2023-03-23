using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Races.DarkelfSubraceBuilder;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceHalfElfVariantRaceBuilder
{
    internal static CharacterRaceDefinition RaceHalfElfVariant { get; } = BuildRaceHalfElfVariant();
    internal static CharacterRaceDefinition RaceHalfElfHighVariant { get; private set; }
    internal static CharacterRaceDefinition RaceHalfElfSylvanVariant { get; private set; }
    internal static CharacterRaceDefinition RaceHalfElfDarkVariant { get; private set; }

    [NotNull]
    private static CharacterRaceDefinition BuildRaceHalfElfVariant()
    {
        var halfDarkelfSpriteReference = Sprites.GetSprite("HalfDarkelf", Resources.HalfDarkelf, 1024, 512);

        RaceHalfElfDarkVariant = CharacterRaceDefinitionBuilder
            .Create(SubraceDarkelf, "RaceHalfElfDark")
            .SetGuiPresentation(Category.Race, halfDarkelfSpriteReference)
            .SetFeaturesAtLevel(1,
                CastSpellDarkelfMagic,
                MoveModeMove6)
            .AddToDB();

        var halfHighSpriteReference = Sprites.GetSprite("HalfHighElf", Resources.HalfHighElf, 1024, 512);

        RaceHalfElfHighVariant = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceHalfElfHigh")
            .SetGuiPresentation(Category.Race, halfHighSpriteReference)
            .SetFeaturesAtLevel(1,
                CastSpellElfHigh,
                MoveModeMove6)
            .AddToDB();

        var halfSylvanSpriteReference =
            Sprites.GetSprite("HalfSylvanElf", Resources.HalfSylvanElf, 1024, 512);

        RaceHalfElfSylvanVariant = CharacterRaceDefinitionBuilder
            .Create(ElfSylvan, "RaceHalfElfSylvan")
            .SetGuiPresentation(Category.Race, halfSylvanSpriteReference)
            .SetFeaturesAtLevel(1,
                MoveModeMove7)
            .AddToDB();

        var raceHalfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "RaceHalfElfVariant")
            .SetOrUpdateGuiPresentation("HalfElf", Category.Race)
            .AddToDB();

        raceHalfElfVariant.SubRaces.SetRange(RaceHalfElfHighVariant, RaceHalfElfSylvanVariant, RaceHalfElfDarkVariant);

        RaceHalfElfHighVariant.GuiPresentation.sortOrder = 0;
        RaceHalfElfSylvanVariant.GuiPresentation.sortOrder = 1;
        RaceHalfElfDarkVariant.GuiPresentation.sortOrder = 2;

        raceHalfElfVariant.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == PointPoolHalfElfSkillPool || x.FeatureDefinition == MoveModeMove6);

        return raceHalfElfVariant;
    }
}
