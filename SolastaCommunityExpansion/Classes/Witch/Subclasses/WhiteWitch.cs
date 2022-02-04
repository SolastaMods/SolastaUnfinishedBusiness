using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class WhiteWitch
    {
        private static readonly Guid Namespace = new("2d849694-5cc9-4333-944b-e40cc1e0d0fd");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        private static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("WhiteMagicAutoPreparedSpell", Namespace)
                .SetGuiPresentationNoContent()
                .SetPreparedSpellGroups(
                    AutoPreparedSpellsGroupBuilder.Build(1, Bless, CureWounds),
                    AutoPreparedSpellsGroupBuilder.Build(3, LesserRestoration, PrayerOfHealing),
                    AutoPreparedSpellsGroupBuilder.Build(5, BeaconOfHope, Revivify),
                    AutoPreparedSpellsGroupBuilder.Build(7, DeathWard, GuardianOfFaith),
                    AutoPreparedSpellsGroupBuilder.Build(9, MassCureWounds, RaiseDead))
                .SetCharacterClass(witchClass)
                .SetAutoTag("Coven")
                .AddToDB();

            var featureDefinitionFeatureSetWhiteMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetWhiteWitchMagic", Namespace)
                .SetGuiPresentation("WhiteWitchMagic", Category.Subclass)
                .SetFeatures(preparedSpells)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(true)
                .AddToDB();

            return new CharacterSubclassDefinitionBuilder("WhiteWitch", Namespace)
                .SetGuiPresentation(Category.Subclass, DomainLife.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureDefinitionFeatureSetWhiteMagic, 3)
                .AddToDB();
        }
    }
}
