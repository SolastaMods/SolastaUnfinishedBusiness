using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    public class RacesContext
    {
        internal static T MemberwiseClone<T>(T base_object)
        {
            MethodInfo memberwiseCloneMethod = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            var copy = (T)memberwiseCloneMethod.Invoke(base_object, null);
            return copy;
        }

        internal static void Load()
        {
            CreateGnome();
        }

        internal static void CreateGnome()
        {
            var gnomeSpriteReference = Utils.CustomIcons.CreateAssetReferenceSprite("Warlock", Properties.Resources.Warlock, 1024, 576);

            var abilityScoreModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierGnomeAbilityScoreIncrease", "")
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
                .AddToDB();

            var forestGnomeScoreModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierForestGnomeAbilityScoreIncrease", "")
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
                .AddToDB();

            var gnomeCunning = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("GnomeCunningFeature", "")
                .SetAffinities(
                    RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                    true,
                    AttributeDefinitions.Intelligence,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Charisma)
                .AddToDB();

            var naturalIllusionistSpellList = SpellListDefinitionBuilder
                .Create(DatabaseHelper.SpellListDefinitions.SpellListWizard, "NaturalIllusionistSpellList", "")
                .SetGuiPresentationNoContent()
                .SetSpellsAtLevel(0, DatabaseHelper.SpellDefinitions.AnnoyingBee)
                .AddToDB();

            var naturalIllusionist = FeatureDefinitionCastSpellBuilder
                .Create(DatabaseHelper.FeatureDefinitionCastSpells.CastSpellElfHigh, "GnomeNaturalIllusionist", "")
                .SetGuiPresentation(Category.Feature)
                .SetSpellList(naturalIllusionistSpellList)
                .AddToDB();

            //
            // TODO: write a builder for Languages
            //
            var languageGnomish = Object.Instantiate(DatabaseHelper.LanguageDefinitions.Language_Goblin);

            languageGnomish.name = "Gnomish";
            languageGnomish.GuiPresentation.SetTitle("Language/&GnomishTitle");
            languageGnomish.GuiPresentation.SetDescription("Language/&GnomishDescription");

            var languageGnomeProficiency = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyGnomishLanguages", "")
                .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", languageGnomish.Name)
                .AddToDB();

            //
            // TODO: write a builder for Races
            //
            var gnome = Object.Instantiate(DatabaseHelper.CharacterRaceDefinitions.Human);

            gnome.name = "GnomeRace";
            gnome.GuiPresentation.SetTitle("Race/&ForestGnomeTitle");
            gnome.GuiPresentation.SetDescription("Race/&ForestGnomeDescription");
            gnome.GuiPresentation.SetSpriteReference(gnomeSpriteReference);
            gnome.SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Small);
            gnome.SetMinimalAge(40);
            gnome.SetMaximalAge(350);
            gnome.SetBaseHeight(47);
            gnome.SetBaseWeight(35);
            gnome.FeatureUnlocks.Clear();
            gnome.SetRacePresentation(MemberwiseClone(DatabaseHelper.CharacterRaceDefinitions.Halfling.RacePresentation));
            gnome.RacePresentation.SetBodyAssetPrefix(DatabaseHelper.CharacterRaceDefinitions.Elf.RacePresentation.BodyAssetPrefix);
            gnome.RacePresentation.SetMorphotypeAssetPrefix(DatabaseHelper.CharacterRaceDefinitions.Elf.RacePresentation.MorphotypeAssetPrefix);
            gnome.RacePresentation.SetPreferedHairColors(new TA.RangedInt(26, 47));
            gnome.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>()
            {
                new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove5, 1),
                new FeatureUnlockByLevel(abilityScoreModifier, 1),
                new FeatureUnlockByLevel(forestGnomeScoreModifier, 1),
                new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision, 1),
                new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision, 1),
                new FeatureUnlockByLevel(gnomeCunning, 1),
                new FeatureUnlockByLevel(naturalIllusionist, 1),
                new FeatureUnlockByLevel(languageGnomeProficiency, 1)
            });

            DatabaseHelper.FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(gnome.name);

            DatabaseRepository.GetDatabase<CharacterRaceDefinition>().Add(gnome);
        }
    }
}
