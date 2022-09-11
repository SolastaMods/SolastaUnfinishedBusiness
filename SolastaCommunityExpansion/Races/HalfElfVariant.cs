using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Races;

internal static class RaceHalfElfVariantRaceBuilder
{
    internal static CharacterRaceDefinition RaceHalfElfVariantRace { get; } = BuildRaceHalfElfVariant();

    [NotNull]
    private static CharacterRaceDefinition BuildRaceHalfElfVariant()
    {
        var darkelfDarkMagic = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>()
            .GetElement("CastSpellDarkelfMagic");

        var darkelfFaerieFire = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .GetElement("PowerDarkelfFaerieFire");

        var darkelfDarkness = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .GetElement("PowerDarkelfDarkness");

        var halfDarkelfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfDarkelf", Resources.HalfDarkelf, 1024, 512);

        var halfElfDarkElf = CharacterRaceDefinitionBuilder
            .Create(DarkelfSubraceBuilder.DarkelfSubrace, "RaceHalfElfDarkElf", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Race, halfDarkelfSpriteReference)
            .SetFeaturesAtLevel(1,
                darkelfDarkMagic,
                MoveModeMove6)
            .AddFeaturesAtLevel(3, darkelfFaerieFire)
            .AddFeaturesAtLevel(5, darkelfDarkness)
            .AddToDB();

        var halfHighSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfHighElf", Resources.HalfHighElf, 1024, 512);

        var castSpellHalfElfHigh = FeatureDefinitionCastSpellBuilder
            .Create(CastSpellElfHigh, "CastSpellHalfElfHigh", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .AddToDB();

        var halfElfHighElf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceHalfElfHighElf", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Race, halfHighSpriteReference)
            .SetFeaturesAtLevel(1,
                Main.Settings.HalfHighElfUseCharisma ? castSpellHalfElfHigh : CastSpellElfHigh,
                MoveModeMove6)
            .AddToDB();

        var halfSylvanSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfSylvanElf", Resources.HalfSylvanElf, 1024, 512);

        var halfElfSylvanElf = CharacterRaceDefinitionBuilder
            .Create(ElfSylvan, "RaceHalfElfSylvanElf", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Race, halfSylvanSpriteReference)
            .SetFeaturesAtLevel(1,
                MoveModeMove7)
            .AddToDB();

        var halfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "RaceHalfElfVariant", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(
                "Race/&HalfElfTitle",
                "Race/&HalfElfDescription",
                HalfElf.guiPresentation.SpriteReference)
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
