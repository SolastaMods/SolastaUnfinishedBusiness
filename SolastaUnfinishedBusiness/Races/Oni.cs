using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;
namespace SolastaUnfinishedBusiness.Races;
internal class RaceOniBuilder
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
            level = 1,
            spells = new List<SpellDefinition>
            {
                SpellsContext.ThunderousSmite
            }
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
                Level = level,
                Slots = new List<int> { level >= 3 ? 1 : 0 }
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

        var list = raceOni.RacePresentation.AvailableMorphotypeCategories.ToList();
        list.Add(MorphotypeElementDefinition.ElementCategory.Horns);
        raceOni.RacePresentation.availableMorphotypeCategories = list.ToArray();
        raceOni.RacePresentation.maleHornsOptions = new List<string>();
        raceOni.RacePresentation.hornsTailAssetPrefix = Tiefling.RacePresentation.hornsTailAssetPrefix;
        raceOni.RacePresentation.maleHornsOptions.AddRange(Tiefling.RacePresentation.maleHornsOptions);
        raceOni.RacePresentation.femaleHornsOptions = new List<string>();
        raceOni.RacePresentation.femaleHornsOptions.AddRange(Tiefling.RacePresentation.femaleHornsOptions);
        RacesContext.RaceScaleMap[raceOni] = 7.4f / 6.4f;
        return raceOni;
    }
}
