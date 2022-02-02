using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class GreenWitch
    {
        public static readonly Guid GW_BASE_GUID = new("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WitchClass { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetGreenMagic { get; private set; }

        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        private static void BuildGreenMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenMagicSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 1,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.Entangle,
                        DatabaseHelper.SpellDefinitions.Goodberry, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenMagicSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 3,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.Barkskin,
                        DatabaseHelper.SpellDefinitions.ProtectionFromPoison, } // This should be Beast Sense
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenMagicSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.ConjureAnimals,
                        DatabaseHelper.SpellDefinitions.CreateFood, }   // This should be Plant Growth
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenMagicSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 7,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.GiantInsect, // This should be Conjure Woodland Beings
                        DatabaseHelper.SpellDefinitions.Stoneskin, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenMagicSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.DispelEvilAndGood, // This should be Awaken
                        DatabaseHelper.SpellDefinitions.InsectPlague, }    // This should be Tree Stride
            };

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "GreenMagicAutoPreparedSpell",
                    GuidHelper.Create(GW_BASE_GUID, "GreenMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>(){
                            greenMagicSpells1,
                            greenMagicSpells2,
                            greenMagicSpells3,
                            greenMagicSpells4,
                            greenMagicSpells5},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetGreenMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetGreenWitchMagic",
                    GuidHelper.Create(GW_BASE_GUID, "FeatureSetGreenWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&GreenWitchMagicTitle",
                            "Subclass/&GreenWitchMagicDescription").Build())
                    .ClearFeatures()
                    .AddFeature(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetGreenMagic, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {
            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&GreenWitchTitle",
                    "Subclass/&GreenWitchDescription")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "GreenWitch",
                    GuidHelper.Create(GW_BASE_GUID, "GreenWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildGreenMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
