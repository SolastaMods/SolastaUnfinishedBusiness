using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Races;

internal static class GnomeRaceBuilder
{
    internal static CharacterRaceDefinition RaceGnome { get; } = BuildGnome();

    [NotNull]
    private static CharacterRaceDefinition BuildGnome()
    {
        var gnomeSpriteReference = Sprites.GetSprite("Gnome", Resources.Gnome, 1024, 512);

        var attributeModifierGnomeAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGnomeAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
            .AddToDB();

        var attributeModifierForestGnomeAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGnomeAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        var savingThrowAffinityGnomeCunningFeature = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinityGnomeCunningFeature")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(
                CharacterSavingThrowAffinity.Advantage,
                true,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var spellListGnome = SpellListDefinitionBuilder
            .Create("SpellListGnome")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellDefinitions.AnnoyingBee)
            .FinalizeSpells(maxLevel: -1)
            .AddToDB();

        var castSpellGnomeNaturalIllusionist = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellGnomeNaturalIllusionist")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(spellListGnome)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .AddToDB();

        var languageGnomish = LanguageDefinitionBuilder
            .Create("LanguageGnomish")
            .SetGuiPresentation(Category.Language)
            .AddToDB();

        var proficiencyGnomeLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyGnomeLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", languageGnomish.Name)
            .AddToDB();

        var gnomeRacePresentation = CharacterRaceDefinitions.HalfElf.RacePresentation.DeepCopy();

        gnomeRacePresentation.preferedHairColors = new RangedInt(26, 47);

        var raceGnome = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, "RaceGnome")
            .SetGuiPresentation(Category.Race, gnomeSpriteReference)
            .SetRacePresentation(gnomeRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetMinimalAge(40)
            .SetMaximalAge(350)
            .SetBaseHeight(47)
            .SetBaseWeight(35)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove5,
                attributeModifierGnomeAbilityScoreIncrease,
                attributeModifierForestGnomeAbilityScoreIncrease,
                SenseNormalVision,
                SenseDarkvision,
                savingThrowAffinityGnomeCunningFeature,
                castSpellGnomeNaturalIllusionist,
                proficiencyGnomeLanguages)
            .AddToDB();

        RacesContext.RaceScaleMap[raceGnome] = 7f / 9.4f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceGnome.name);

        return raceGnome;
    }
}
