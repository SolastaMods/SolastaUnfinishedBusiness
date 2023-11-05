using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceOniBuilder
{
    private const string Name = "Oni";

    internal static CharacterRaceDefinition RaceOni { get; } = BuildOni();

    [NotNull]
    private static CharacterRaceDefinition BuildOni()
    {
        var featureSetOniAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseStr,
                FeatureDefinitionAttributeModifiers.AttributeModifierDwarfHillAbilityScoreIncrease)
            .AddToDB();

        var proficiencyOniThirdEye = FeatureDefinitionProficiencyBuilder.Create($"Proficiency{Name}ThirdEye")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Perception)
            .AddToDB();

        var proficiencyOniOgreMight = FeatureDefinitionProficiencyBuilder.Create($"Proficiency{Name}OgreMight")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var spellListOni = SpellListDefinitionBuilder
            .Create($"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();
        // SpellListDefinitionBuilder doesn't seem to work for Cantrip-less spell list
        // and I don't want to break it
        spellListOni.hasCantrips = false;
        spellListOni.maxSpellLevel = 1;
        spellListOni.spellsByLevel.Clear();
        spellListOni.spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
        {
            level = 1, spells = new List<SpellDefinition> { SpellsContext.ThunderousSmite }
        });

        var castSpellOniOgreMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{Name}OgreMagic")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(0, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(spellListOni)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .AddToDB();

        castSpellOniOgreMagic.slotsPerLevels.Clear();

        for (var level = 1; level <= 20; level++)
        {
            castSpellOniOgreMagic.slotsPerLevels.Add(new FeatureDefinitionCastSpell.SlotsByLevelDuplet
            {
                Level = level, Slots = new List<int> { level >= 3 ? 1 : 0 }
            });
        }

        var raceOni = CharacterRaceDefinitionBuilder
            .Create(HalfOrc, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Oni, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetBaseHeight(92)
            .SetBaseWeight(185)
            .SetMinimalAge(18)
            .SetMaximalAge(750)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseDarkvision,
                FeatureDefinitionSenses.SenseNormalVision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne,
                featureSetOniAbilityScoreIncrease,
                proficiencyOniThirdEye,
                castSpellOniOgreMagic,
                proficiencyOniOgreMight)
            .AddToDB();

        var racePresentation = raceOni.RacePresentation;
        var availableMorphotypeCategories = racePresentation.AvailableMorphotypeCategories.ToList();

        availableMorphotypeCategories.Add(MorphotypeElementDefinition.ElementCategory.Horns);

        racePresentation.availableMorphotypeCategories = availableMorphotypeCategories.ToArray();
        racePresentation.maleHornsOptions = new List<string>();
        racePresentation.hornsTailAssetPrefix = Tiefling.RacePresentation.hornsTailAssetPrefix;
        racePresentation.maleHornsOptions.AddRange(Tiefling.RacePresentation.maleHornsOptions);
        racePresentation.femaleHornsOptions = new List<string>();
        racePresentation.femaleHornsOptions.AddRange(Tiefling.RacePresentation.femaleHornsOptions);
        RacesContext.RaceScaleMap[raceOni] = 7.4f / 6.4f;

        return raceOni;
    }
}
