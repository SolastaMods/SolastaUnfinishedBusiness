using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Races;

internal static class HalfElfVariantRaceBuilder
{
    private static readonly Guid RaceNamespace = new("f5efd735-ff95-4256-ba17-dde585aec5e2");

    internal static CharacterRaceDefinition HalfElfVariantRace { get; } = BuildHalfElfVariant();

    private static CharacterRaceDefinition BuildHalfElfVariant()
    {
        var darkelfDarkMagic = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>()
            .GetElement("DarkelfMagic");

        var halfElfDarkElf = CharacterRaceDefinitionBuilder
            .Create(DarkelfSubraceBuilder.DarkelfSubrace, "HalfElfDarkElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race, DarkelfSubraceBuilder.DarkelfSubrace.GuiPresentation.SpriteReference)
            .SetFeaturesAtLevel(1,
                darkelfDarkMagic,
                MoveModeMove6)
            .AddToDB();

        var halfElfHighElf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "HalfElfHighElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race, ElfHigh.guiPresentation.SpriteReference)
            .SetFeaturesAtLevel(1,
                CastSpellElfHigh,
                MoveModeMove6)
            .AddToDB();

        var halfElfSylvanElf = CharacterRaceDefinitionBuilder
            .Create(ElfSylvan, "HalfElfSylvanElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race, ElfSylvan.guiPresentation.SpriteReference)
            .SetFeaturesAtLevel(1,
                MoveModeMove7)
            .AddToDB();

        var halfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "HalfElfVariant", RaceNamespace)
            .SetGuiPresentation(Category.Race, HalfElf.guiPresentation.SpriteReference)
            .AddToDB();

        halfElfVariant.SubRaces.SetRange(new List<CharacterRaceDefinition>
        {
            halfElfDarkElf, halfElfHighElf, halfElfSylvanElf
        });

        halfElfVariant.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PointPoolHalfElfSkillPool
                            || x.FeatureDefinition == MoveModeMove6);

        return halfElfVariant;
    }
}
