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
    internal class BloodWitch : AbstractSubclass
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

        public static readonly Guid BW_BASE_GUID = new Guid("c9f680ec-7c79-414f-b700-eebc11863105");

        private static CharacterClassDefinition WitchClass { get; set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetBloodMagic { get; private set; }

        private static void BuildBloodMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "BloodMagicAutoPreparedSpell",
                    GuidHelper.Create(BW_BASE_GUID, "BloodMagicAutoPreparedSpell").ToString(),
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

            FeatureDefinitionFeatureSetBloodMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetBloodWitchMagic",
                    GuidHelper.Create(BW_BASE_GUID, "FeatureSetBloodWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&BloodWitchMagicDescription",
                            "Subclass/&BloodWitchMagicTitle").Build())
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
                    "Subclass/&BloodWitchDescription",
                    "Subclass/&BloodWitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainOblivion.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "BloodWitch", 
                    GuidHelper.Create(BW_BASE_GUID, "BloodWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildBloodMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
        
    }
}