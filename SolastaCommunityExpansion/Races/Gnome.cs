using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Races;

internal static class GnomeRaceBuilder
{
    internal static CharacterRaceDefinition GnomeRace { get; } = BuildGnome();

    private static CharacterRaceDefinition BuildGnome()
    {
        var gnomeSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Gnome", Resources.Gnome, 1024, 512);

        var gnomeAbilityScoreModifier = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGnomeAbilityScoreIncrease", "b1475c33-f9ba-4224-b4b1-a55621f4dcd1")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Intelligence, 2)
            .AddToDB();

        var forestGnomeAbilityScoreModifier = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierForestGnomeAbilityScoreIncrease", "b7f18e2f-532f-46bf-96d2-f3612026295f")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Dexterity, 1)
            .AddToDB();

        var gnomeCunning = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("GnomeCunningFeature", "8804963d-8214-480a-be62-98a5652f69e5")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                true,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var gnomeNaturalIllusionistSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "NaturalIllusionistSpellList",
                "ead60aeb-3a72-4296-af36-2e14b110bb0b")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.AnnoyingBee)
            .FinalizeSpells()
            .AddToDB();

        var gnomeNaturalIllusionist = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "GnomeNaturalIllusionist",
                "bfb1b55f-dbce-49b7-b76d-5b5b3c4fe992")
            .SetGuiPresentation(Category.Feature)
            .SetSpellList(gnomeNaturalIllusionistSpellList)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .AddToDB();

        var languageGnomish = LanguageDefinitionBuilder
            .Create("GnomishLanguage", "ca2c78df-6308-4fd6-b0ae-0204b8e5ff6f")
            .SetGuiPresentation(Category.Language)
            .AddToDB();

        var gnomeLanguageProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyGnomishLanguages", "c497f363-c5d9-4255-9549-2db054976361")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", languageGnomish.Name)
            .AddToDB();

        var gnomeRacePresentation = CharacterRaceDefinitions.HalfElf.RacePresentation.DeepCopy();

        gnomeRacePresentation.preferedHairColors = new RangedInt(26, 47);

        var gnome = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, "GnomeRace", "ce63140e-c018-4f83-8e6e-bc7bbc815a17")
            .SetGuiPresentation(Category.Race, gnomeSpriteReference)
            .SetRacePresentation(gnomeRacePresentation)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetMinimalAge(40)
            .SetMaximalAge(350)
            .SetBaseHeight(47)
            .SetBaseWeight(35)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove5,
                gnomeAbilityScoreModifier,
                forestGnomeAbilityScoreModifier,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                gnomeCunning,
                gnomeNaturalIllusionist,
                gnomeLanguageProficiency)
            .AddToDB();

        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(gnome.name);

        return gnome;
    }
}
