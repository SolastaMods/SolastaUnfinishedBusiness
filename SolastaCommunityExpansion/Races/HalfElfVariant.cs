#if false
using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;

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
            .Create(HalfElf, "HalfElfDarkElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1, darkelfDarkMagic)
            .AddToDB();

        var halfElfHighElf = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "HalfElfHighElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1, CastSpellElfHigh)
            .AddToDB();

        var halfElfSylvanElf = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "HalfElfSylvanElfRace", RaceNamespace)
            .SetGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1, ProficiencyElfWeaponTraining)
            .AddToDB();

        var halfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "HalfElfVariant", RaceNamespace)
            .SetGuiPresentation(Category.Race)
            .AddToDB();

        halfElfVariant.SubRaces.SetRange(new List<CharacterRaceDefinition>()
        {
            halfElfDarkElf, halfElfHighElf, halfElfSylvanElf
        });
        halfElfVariant.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == PointPoolHalfElfSkillPool);

        return halfElfVariant;
    }
}
#endif
