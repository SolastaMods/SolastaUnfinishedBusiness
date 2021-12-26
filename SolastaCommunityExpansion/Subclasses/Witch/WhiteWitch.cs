using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class WhiteWitch
    {
        
        public static readonly Guid WW_BASE_GUID = new Guid("2d849694-5cc9-4333-944b-e40cc1e0d0fd");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WitchClass { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetWhiteMagic { get; private set; }

        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            if (Subclass == null)
            {
                Subclass = BuildAndAddSubclass(witchClass);
            }
            return Subclass;
        }

        private static void BuildWhiteMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "WhiteMagicAutoPreparedSpell",
                    GuidHelper.Create(WW_BASE_GUID, "WhiteMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                            FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(
                                    1,
                                    new List<SpellDefinition>{
                                            DatabaseHelper.SpellDefinitions.Bless, 
                                            DatabaseHelper.SpellDefinitions.CureWounds, 
                                            DatabaseHelper.SpellDefinitions.LesserRestoration, 
                                            DatabaseHelper.SpellDefinitions.PrayerOfHealing, 
                                            DatabaseHelper.SpellDefinitions.BeaconOfHope, 
                                            DatabaseHelper.SpellDefinitions.Revivify, 
                                            DatabaseHelper.SpellDefinitions.DeathWard, 
                                            DatabaseHelper.SpellDefinitions.GuardianOfFaith, 
                                            DatabaseHelper.SpellDefinitions.MassCureWounds, 
                                            DatabaseHelper.SpellDefinitions.RaiseDead, 
                                            })},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetWhiteMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetWhiteWitchMagic",
                    GuidHelper.Create(WW_BASE_GUID, "FeatureSetWhiteWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&WhiteWitchMagicDescription",
                            "Subclass/&WhiteWitchMagicTitle").Build())
                    .ClearFeatures()
                    .AddFeature(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();

        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetWhiteMagic, 3);
        }

        private static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {

            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&WhiteWitchDescription",
                    "Subclass/&WhiteWitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "WhiteWitch", 
                    GuidHelper.Create(WW_BASE_GUID, "WhiteWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildWhiteMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
        
    }
}