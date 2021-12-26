using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class GreenWitch
    {

        public static readonly Guid GW_BASE_GUID = new Guid("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");
        private CharacterSubclassDefinition Subclass;
        public static CharacterClassDefinition WitchClass { get; private set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetGreenMagic { get; private set; }

        internal CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            if (Subclass == null)
            {
                Subclass = BuildAndAddSubclass(witchClass);
            }
            return Subclass;
        }

        private static void BuildGreenMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "GreenMagicAutoPreparedSpell",
                    GuidHelper.Create(GW_BASE_GUID, "GreenMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                            FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(
                                    1,
                                    new List<SpellDefinition>{
                                            DatabaseHelper.SpellDefinitions.Entangle, 
                                            DatabaseHelper.SpellDefinitions.Goodberry, 
                                            DatabaseHelper.SpellDefinitions.Barkskin, 
                                            DatabaseHelper.SpellDefinitions.ProtectionFromPoison, // This should be Beast Sense
                                            DatabaseHelper.SpellDefinitions.ConjureAnimals, 
                                            DatabaseHelper.SpellDefinitions.CreateFood, // This should be Plant Growth
                                            DatabaseHelper.SpellDefinitions.GiantInsect, // This should be Conjure Woodland Beings
                                            DatabaseHelper.SpellDefinitions.Stoneskin, 
                                            DatabaseHelper.SpellDefinitions.DispelEvilAndGood, // This should be Awaken
                                            DatabaseHelper.SpellDefinitions.InsectPlague, // This should be Tree Stride
                                            })},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetGreenMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetGreenWitchMagic",
                    GuidHelper.Create(GW_BASE_GUID, "FeatureSetGreenWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&GreenWitchMagicDescription",
                            "Subclass/&GreenWitchMagicTitle").Build())
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
                    "Subclass/&GreenWitchDescription",
                    "Subclass/&GreenWitchTitle")
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