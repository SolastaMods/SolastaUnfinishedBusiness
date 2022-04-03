using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Models
{
    internal static class RacesContext
    {
        internal static CharacterRaceDefinition BolgrifRace { get; private set; } = BuildBolgrif();

        internal static CharacterRaceDefinition GnomeRace { get; private set; } = BuildGnome();

        internal static void Load()
        {
            //
            // TODO: consider a setting on UI for this and add a Switch. It's a bit more complicated than it should as we need a patch
            //
            _ = BolgrifRace;
            _ = GnomeRace;
        }

        internal static CharacterRaceDefinition BuildBolgrif()
        {
            var bolgrifSpriteReference = Utils.CustomIcons.CreateAssetReferenceSprite("Bolgrif", Properties.Resources.Bolgrif, 1024, 512);

            var bolgrifAbilityScoreModifier1 = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierBolgrifWisdomAbilityScoreIncrease", "4099c645-fc05-4ba1-833f-eabb94b865d0")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Wisdom, 2)
                .AddToDB();

            var bolgrifAbilityScoreModifier2 = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierBolgrifStrengthAbilityScoreIncrease", "7b8d459b-c1f2-4373-bc4d-5e29ea4851f3")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
                .AddToDB();

            var bolgrifAbilityScoreModifierSet = FeatureDefinitionFeatureSetBuilder
                .Create("AttributeModifierBolgrifAbilityScoreSet", "0a6efe74-ebcf-455c-9eb3-31741d22d3bd")
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetFeatureSet(bolgrifAbilityScoreModifier1, bolgrifAbilityScoreModifier2)
                .AddToDB();

            var bolgrifPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
                .Create(FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler, "BolgrifPowerfulBuild", "3f635935-28a3-4bfd-8f51-77417ad7eb8a")
                .SetGuiPresentation(Category.Feature)
                .AddToDB();

            var bolgrifInvisibilityEffect = EffectDescriptionBuilder
                .Create(SpellDefinitions.Invisibility.EffectDescription)
                .SetDurationData(RuleDefinitions.DurationType.Instantaneous, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Self, 1,
                    RuleDefinitions.TargetType.Self, 1, 1,
                    ActionDefinitions.ItemSelectionType.None)
                .Build();

            bolgrifInvisibilityEffect.EffectAdvancement.Clear();

            var bolgrifInvisibilityPower = FeatureDefinitionPowerBuilder
                .Create("BolgrifInvisibilityPower", "36dcb055-372c-4abf-83b7-4405475d9295")
                .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
                .SetEffectDescription(bolgrifInvisibilityEffect)
                .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
                .SetUsesFixed(1)
                .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
                .SetShowCasting(true)
                .AddToDB();

            var bolgrifDruidicMagicSpellList = SpellListDefinitionBuilder
                .Create(SpellListDefinitions.SpellListWizard, "BolgrifDruidicMagicSpellList", "3ac97eec-8d09-4ce3-8d29-40ea8b423798")
                .SetGuiPresentationNoContent()
                .SetSpellsAtLevel(0, SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells)
                .FinalizeSpells()
                .AddToDB();

            var bolgrifDruidicMagic = FeatureDefinitionCastSpellBuilder
                .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "BolgrifDruidicMagic", "ea2a9c8e-6ca9-490b-a056-d768182b5cd2")
                .SetGuiPresentation(Category.Feature)
                .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
                .SetSpellList(bolgrifDruidicMagicSpellList)
                .AddToDB();

            var bolgrifLanguageProficiency = FeatureDefinitionProficiencyBuilder
                .Create("ProficiencyBolgrifLanguages", "dc03c8d7-5098-4dec-9f60-46e3af0d63c9")
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", "Language_Giant", "Language_Elvish")
                .AddToDB();

            var bolgrifRacePresentation = CharacterRaceDefinitions.Elf.RacePresentation.DeepCopy();

            bolgrifRacePresentation.SetBodyAssetPrefix(CharacterRaceDefinitions.Elf.RacePresentation.BodyAssetPrefix);
            bolgrifRacePresentation.SetMorphotypeAssetPrefix(CharacterRaceDefinitions.Elf.RacePresentation.MorphotypeAssetPrefix);
            bolgrifRacePresentation.SetPreferedSkinColors(new TA.RangedInt(45, 48));
            bolgrifRacePresentation.SetPreferedHairColors(new TA.RangedInt(16, 32));
            bolgrifRacePresentation.SetMaleBeardShapeOptions(CharacterRaceDefinitions.Dwarf.RacePresentation.MaleBeardShapeOptions);

            var bolgrif = CharacterRaceDefinitionBuilder
                .Create(CharacterRaceDefinitions.Human, "BolgrifRace", "346b7f90-973f-425f-8342-d534759e65aa")
                .SetGuiPresentation(Category.Race, bolgrifSpriteReference)
                .SetSizeDefinition(CharacterSizeDefinitions.Medium)
                .SetMinimalAge(30)
                .SetMaximalAge(500)
                .SetBaseHeight(96)
                .SetBaseWeight(130)
                .SetFeatures(
                    (FeatureDefinitionMoveModes.MoveModeMove6, 1),
                    (bolgrifAbilityScoreModifierSet, 1),
                    (FeatureDefinitionSenses.SenseNormalVision, 1),
                    (bolgrifPowerfulBuild, 1),
                    (bolgrifInvisibilityPower, 1),
                    (bolgrifDruidicMagic, 1),
                    (bolgrifLanguageProficiency, 1))
                .AddToDB();

            FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(bolgrif.name);

            return bolgrif;
        }

        internal static CharacterRaceDefinition BuildGnome()
        {
            var gnomeSpriteReference = Utils.CustomIcons.CreateAssetReferenceSprite("Gnome", Properties.Resources.Gnome, 1024, 512);

            var gnomeAbilityScoreModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierGnomeAbilityScoreIncrease", "b1475c33-f9ba-4224-b4b1-a55621f4dcd1")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
                .AddToDB();

            var forestGnomeAbilityScoreModifier = FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierForestGnomeAbilityScoreIncrease", "b7f18e2f-532f-46bf-96d2-f3612026295f")
                .SetGuiPresentationNoContent(true)
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1)
                .AddToDB();

            var gnomeAbilityScoreModifierSet = FeatureDefinitionFeatureSetBuilder
                .Create("AttributeModifierGnomeAbilityScoreSet", "b239a9b0-f964-48b4-8958-a631ee3d6178")
                .SetGuiPresentation(Category.Feature)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetFeatureSet(gnomeAbilityScoreModifier, forestGnomeAbilityScoreModifier)
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
                .Create(SpellListDefinitions.SpellListWizard, "NaturalIllusionistSpellList", "ead60aeb-3a72-4296-af36-2e14b110bb0b")
                .SetGuiPresentationNoContent()
                .SetSpellsAtLevel(0, SpellDefinitions.AnnoyingBee)
                .FinalizeSpells()
                .AddToDB();

            var gnomeNaturalIllusionist = FeatureDefinitionCastSpellBuilder
                .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "GnomeNaturalIllusionist", "bfb1b55f-dbce-49b7-b76d-5b5b3c4fe992")
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

            var gnomeRacePresentation = CharacterRaceDefinitions.Halfling.RacePresentation.DeepCopy();

            gnomeRacePresentation.SetBodyAssetPrefix(CharacterRaceDefinitions.Elf.RacePresentation.BodyAssetPrefix);
            gnomeRacePresentation.SetMorphotypeAssetPrefix(CharacterRaceDefinitions.Elf.RacePresentation.MorphotypeAssetPrefix);
            gnomeRacePresentation.SetPreferedHairColors(new TA.RangedInt(26, 47));

            var gnome = CharacterRaceDefinitionBuilder
                .Create(CharacterRaceDefinitions.Human, "GnomeRace", "ce63140e-c018-4f83-8e6e-bc7bbc815a17")
                .SetGuiPresentation(Category.Race, gnomeSpriteReference)
                .SetRacePresentation(gnomeRacePresentation)
                .SetSizeDefinition(CharacterSizeDefinitions.Small)
                .SetMinimalAge(40)
                .SetMaximalAge(350)
                .SetBaseHeight(47)
                .SetBaseWeight(35)
                .SetFeatures(
                    (FeatureDefinitionMoveModes.MoveModeMove5, 1),
                    (gnomeAbilityScoreModifierSet, 1),
                    (FeatureDefinitionSenses.SenseNormalVision, 1),
                    (FeatureDefinitionSenses.SenseDarkvision, 1),
                    (gnomeCunning, 1),
                    (gnomeNaturalIllusionist, 1),
                    (gnomeLanguageProficiency, 1))
                .AddToDB();

            FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(gnome.name);

            return gnome;
        }
    }
}
