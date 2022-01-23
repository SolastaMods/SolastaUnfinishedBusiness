using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Features;
using SolastaModApi;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class PurpleWitch
    {
        public static readonly Guid PW_BASE_GUID = new Guid("bb8a01e8-7997-4c44-8643-72ac15853b47");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WitchClass { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetPurpleMagic { get; private set; }

        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ?? (Subclass = BuildAndAddSubclass(witchClass));
        }

        private static void BuildPurpleMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup purpleMagicSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 1,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.CharmPerson,
                        DatabaseHelper.SpellDefinitions.HideousLaughter, }   // This should be Silent Image
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup purpleMagicSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 3,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.CalmEmotions, // This should be Enthrall
                        DatabaseHelper.SpellDefinitions.Invisibility, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup purpleMagicSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
                        DatabaseHelper.SpellDefinitions.Fear, } // This should be Major Image
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup purpleMagicSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 7,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.Confusion,
                        DatabaseHelper.SpellDefinitions.PhantasmalKiller, }  // This should be Private Sanctum
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup purpleMagicSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.DominatePerson, // This should be Modify Memory
                        DatabaseHelper.SpellDefinitions.HoldMonster, }    // This should be Seeming
            };

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "PurpleMagicAutoPreparedSpell",
                    GuidHelper.Create(PW_BASE_GUID, "PurpleMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>(){
                            purpleMagicSpells1,
                            purpleMagicSpells2,
                            purpleMagicSpells3,
                            purpleMagicSpells4,
                            purpleMagicSpells5},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetPurpleMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetPurpleWitchMagic",
                    GuidHelper.Create(PW_BASE_GUID, "FeatureSetPurpleWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&PurpleWitchMagicDescription",
                            "Subclass/&PurpleWitchMagicTitle").Build())
                    .ClearFeatures()
                    .AddFeature(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetPurpleMagic, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {
            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&PurpleWitchDescription",
                    "Subclass/&PurpleWitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainInsight.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "PurpleWitch",
                    GuidHelper.Create(PW_BASE_GUID, "PurpleWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildPurpleMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
