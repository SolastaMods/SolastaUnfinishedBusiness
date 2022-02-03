using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal class BloodWitch
    {
        public static readonly Guid BLOODW_BASE_GUID = new("c9f680ec-7c79-414f-b700-eebc11863105");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WitchClass { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetBloodMagic { get; private set; }

        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        private static void BuildBloodMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup bloodMagicSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 1,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.FalseLife, // This should be Hellish Rebuke
                        DatabaseHelper.SpellDefinitions.InflictWounds, }   // This should be Hollowing Curse
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup bloodMagicSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 3,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.AcidArrow,
                        DatabaseHelper.SpellDefinitions.HoldPerson, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup bloodMagicSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.BestowCurse, // This should be Rube-Eye Curse
                        DatabaseHelper.SpellDefinitions.VampiricTouch, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup bloodMagicSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 7,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.DominateBeast, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup bloodMagicSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                SpellsList = new List<SpellDefinition>() {
                        DatabaseHelper.SpellDefinitions.CloudKill,
                        DatabaseHelper.SpellDefinitions.DominatePerson, }
            };

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "BloodMagicAutoPreparedSpell",
                    GuidHelper.Create(BLOODW_BASE_GUID, "BloodMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>(){
                            bloodMagicSpells1,
                            bloodMagicSpells2,
                            bloodMagicSpells3,
                            bloodMagicSpells4,
                            bloodMagicSpells5},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetBloodMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetBloodWitchMagic",
                    GuidHelper.Create(BLOODW_BASE_GUID, "FeatureSetBloodWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&BloodWitchMagicTitle",
                            "Subclass/&BloodWitchMagicDescription").Build())
                    .ClearFeatures()
                    .AddFeature(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();
        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetBloodMagic, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {
            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&BloodWitchTitle",
                    "Subclass/&BloodWitchDescription")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainOblivion.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "BloodWitch",
                    GuidHelper.Create(BLOODW_BASE_GUID, "BloodWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildBloodMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
    }
}
