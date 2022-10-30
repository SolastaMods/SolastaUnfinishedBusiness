using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using TA;
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
        var gnomeSpriteReference =
            CustomIcons.GetSprite("Gnome", Resources.Gnome, 1024, 512);

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
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                true,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Charisma)
            .AddToDB();

        var spellListGnome = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListGnome")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.AnnoyingBee)
            .FinalizeSpells()
            .AddToDB();

        var castSpellGnomeNaturalIllusionist = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellGnomeNaturalIllusionist")
            .SetGuiPresentation(Category.Feature)
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
            .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", languageGnomish.Name)
            .AddToDB();

        var gnomeRacePresentation = CharacterRaceDefinitions.HalfElf.RacePresentation.DeepCopy();

        gnomeRacePresentation.preferedHairColors = new RangedInt(26, 47);

        var gnome = CharacterRaceDefinitionBuilder
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

        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(gnome.name);

        return gnome;
    }
}
