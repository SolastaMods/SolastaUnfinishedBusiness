using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.BuilderHelpers;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class PurpleWitch : AbstractSubclass
    {
        private CharacterSubclassDefinition Subclass;
        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            DatabaseRepository.GetDatabase<FeatureDefinitionSubclassChoice>().TryGetElement("SubclassChoiceWitchCovens", out FeatureDefinitionSubclassChoice featureDefinitionSubclassChoice);
            return featureDefinitionSubclassChoice;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        public CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            if (GetSubclass() == null)
            {
                Subclass = BuildAndAddSubclass(witchClass);
            }
            return Subclass;
        }

        public static readonly Guid PW_BASE_GUID = new Guid("bb8a01e8-7997-4c44-8643-72ac15853b47");

        private static CharacterClassDefinition WitchClass { get; set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetPurpleMagic { get; private set; }

        private static void BuildPurpleMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "PurpleMagicAutoPreparedSpell",
                    GuidHelper.Create(PW_BASE_GUID, "PurpleMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                            FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(
                                    1,
                                    new List<SpellDefinition>{
                                            DatabaseHelper.SpellDefinitions.Entangle, 
                                            DatabaseHelper.SpellDefinitions.Goodberry, 
                                            DatabaseHelper.SpellDefinitions.Barkskin, 
                                            DatabaseHelper.SpellDefinitions.ProtectionFromPoison, 
                                            DatabaseHelper.SpellDefinitions.ConjureAnimals, 
                                            DatabaseHelper.SpellDefinitions.CreateFood, 
                                            DatabaseHelper.SpellDefinitions.GiantInsect, 
                                            DatabaseHelper.SpellDefinitions.Stoneskin, 
                                            DatabaseHelper.SpellDefinitions.DispelEvilAndGood, 
                                            DatabaseHelper.SpellDefinitions.InsectPlague, 
                                            })},
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