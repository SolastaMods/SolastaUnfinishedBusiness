using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
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
        var darkelfDarkMagic = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>()
            .GetElement("CastSpellDarkelfMagic");

        var darkelfFaerieFire = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .GetElement("PowerDarkelfFaerieFire");

        var darkelfDarkness = DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
            .GetElement("PowerDarkelfDarkness");

        var halfDarkelfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfDarkelf", Resources.HalfDarkelf, 1024, 512);

        var halfElfDarkElf = CharacterRaceDefinitionBuilder
            .Create(DarkelfSubraceBuilder.DarkelfSubrace, "RaceHalfElfDark")
            .SetGuiPresentation(Category.Race, halfDarkelfSpriteReference)
            .SetFeaturesAtLevel(1,
                darkelfDarkMagic,
                MoveModeMove6)
            .AddFeaturesAtLevel(3, darkelfFaerieFire)
            .AddFeaturesAtLevel(5, darkelfDarkness)
            .AddToDB();

        var halfHighSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfHighElf", Resources.HalfHighElf, 1024, 512);

        #if false
        var castSpellHalfElfHigh = FeatureDefinitionCastSpellBuilder
            .Create(CastSpellElfHigh, "CastSpellHalfElfHigh")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .AddToDB();
        // Feature/&CastSpellHalfElfHighDescription=Choose one Wizard cantrip, using Charisma as the spellcasting ability.
        // Feature/&CastSpellHalfElfHighTitle=High Elf Cantrip
        #endif
        
        var halfElfHighElf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceHalfElfHigh")
            .SetGuiPresentation(Category.Race, halfHighSpriteReference)
            .SetFeaturesAtLevel(1,
                // Main.Settings.HalfHighElfUseCharisma ? castSpellHalfElfHigh : CastSpellElfHigh,
                // ModUi/&HalfHighElfUseCharisma=<color=#D89555>Half-High Elf</color> uses Charisma instead of Intelligence as the cantrip score ability <b><i><color=#C04040E0>[Requires Restart]</color></i></b>
                CastSpellElfHigh,
                MoveModeMove6)
            .AddToDB();

        var halfSylvanSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("HalfSylvanElf", Resources.HalfSylvanElf, 1024, 512);

        var halfElfSylvanElf = CharacterRaceDefinitionBuilder
            .Create(ElfSylvan, "RaceHalfElfSylvan")
            .SetGuiPresentation(Category.Race, halfSylvanSpriteReference)
            .SetFeaturesAtLevel(1,
                MoveModeMove7)
            .AddToDB();

        var halfElfVariant = CharacterRaceDefinitionBuilder
            .Create(HalfElf, "RaceHalfElfVariant")
            .SetOrUpdateGuiPresentation("HalfElf", Category.Race)
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
