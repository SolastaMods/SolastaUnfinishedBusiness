using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceHalfElfVariantRaceBuilder
{
    internal static CharacterRaceDefinition RaceHalfElfVariant { get; } = BuildRaceHalfElfVariant();

    [NotNull]
    private static CharacterRaceDefinition BuildRaceHalfElfVariant()
    {
        var halfDarkelfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfDarkelf", Resources.HalfDarkelf, 1024, 512);

        var raceHalfElfDark = CharacterRaceDefinitionBuilder
            .Create(DarkelfSubraceBuilder.SubraceDarkelf, "RaceHalfElfDark")
            .SetGuiPresentation(Category.Race, halfDarkelfSpriteReference)
            .SetFeaturesAtLevel(1,
                DarkelfSubraceBuilder.CastSpellDarkelfMagic,
                MoveModeMove6)
            .AddFeaturesAtLevel(3, DarkelfSubraceBuilder.PowerDarkelfFaerieFire)
            .AddFeaturesAtLevel(5, DarkelfSubraceBuilder.PowerDarkelfDarkness)
            .AddToDB();

        var halfHighSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfHighElf", Resources.HalfHighElf, 1024, 512);

        var raceHalfElfHigh = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceHalfElfHigh")
            .SetGuiPresentation(Category.Race, halfHighSpriteReference)
            .SetFeaturesAtLevel(1,
                CastSpellElfHigh,
                MoveModeMove6)
            .AddToDB();

        var halfSylvanSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfSylvanElf", Resources.HalfSylvanElf, 1024, 512);

        var raceHalfElfSylvan = CharacterRaceDefinitionBuilder
            .Create(ElfSylvan, "RaceHalfElfSylvan")
            .SetGuiPresentation(Category.Race, halfSylvanSpriteReference)
            .SetFeaturesAtLevel(1,
                MoveModeMove7)
            .AddToDB();

        var raceHalfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "RaceHalfElfVariant")
            .SetOrUpdateGuiPresentation("HalfElf", Category.Race)
            .AddToDB();

        raceHalfElfVariant.SubRaces.SetRange(new List<CharacterRaceDefinition>
        {
            raceHalfElfHigh, raceHalfElfSylvan, raceHalfElfDark
        });

        raceHalfElfHigh.GuiPresentation.sortOrder = 0;
        raceHalfElfSylvan.GuiPresentation.sortOrder = 1;
        raceHalfElfDark.GuiPresentation.sortOrder = 2;

        raceHalfElfVariant.FeatureUnlocks
            .RemoveAll(x =>
                x.FeatureDefinition == PointPoolHalfElfSkillPool || x.FeatureDefinition == MoveModeMove6);

        return raceHalfElfVariant;
    }
}
