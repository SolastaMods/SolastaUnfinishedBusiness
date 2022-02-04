using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class GreenWitch
    {
        private static readonly Guid Namespace = new("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("GreenMagicAutoPreparedSpell", Namespace)
                .SetGuiPresentationNoContent()
                .SetPreparedSpellGroups(
                    AutoPreparedSpellsGroupBuilder.Build(1, Entangle, Goodberry),
                    AutoPreparedSpellsGroupBuilder.Build(3,
                        Barkskin,
                        ProtectionFromPoison),// This should be Beast Sense
                    AutoPreparedSpellsGroupBuilder.Build(5,
                        ConjureAnimals,
                        CreateFood), // This should be Plant Growth
                    AutoPreparedSpellsGroupBuilder.Build(7,
                        GiantInsect, // This should be Conjure Woodland Beings
                        Stoneskin),
                    AutoPreparedSpellsGroupBuilder.Build(9,
                        DispelEvilAndGood, // This should be Awaken
                        InsectPlague))   // This should be Tree Stride
                .SetCharacterClass(witchClass)
                .SetAutoTag("Coven")
                .AddToDB();

            var featureDefinitionFeatureSetGreenMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetGreenWitchMagic", Namespace)
                .SetGuiPresentationGenerate("GreenWitchMagic", Category.Subclass)
                .SetFeatures(preparedSpells)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(true)
                .AddToDB();

            return new CharacterSubclassDefinitionBuilder("GreenWitch", Namespace)
                .SetGuiPresentationGenerate("GreenWitch", Category.Subclass, TraditionGreenmage.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureDefinitionFeatureSetGreenMagic, 3)
                .AddToDB();
        }
    }
}
