using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Subclasses.Witch
{
    internal class GreenWitch : AbstractSubclass
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

        public static readonly Guid GW_BASE_GUID = new Guid("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");

        private static CharacterClassDefinition WitchClass { get; set; }
        public static readonly FeatureDefinitionFeatureSet greenWitchMagic = createGreenWitchMagic(WitchClass);

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {

            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&GreenWitchDescription",
                    "Subclass/&GreenWitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.CircleLand.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(
                    "GreenWitch", 
                    GuidHelper.Create(GW_BASE_GUID, "GreenWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(greenWitchMagic, 3)
                    .AddToDB();

            return definition;
        }

        private static FeatureDefinitionFeatureSet createGreenWitchMagic(CharacterClassDefinition witchClass)
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenWitchSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 1,
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Entangle, DatabaseHelper.SpellDefinitions.Goodberry, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenWitchSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 3,
                // The second spell here should be BeastSense
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Barkskin, DatabaseHelper.SpellDefinitions.ProtectionFromPoison, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenWitchSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                // The second spell here should be PlantGrowth
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.ConjureAnimals, DatabaseHelper.SpellDefinitions.CreateFood, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenWitchSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 7,
                // The first spell here should be Conjure Woodland Beings
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.GiantInsect, DatabaseHelper.SpellDefinitions.Stoneskin, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup greenWitchSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                // The spells here should be Awaken and Tree Stride
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.DispelEvilAndGood, DatabaseHelper.SpellDefinitions.InsectPlague, }
            };

            FeatureDefinitionAutoPreparedSpells preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("GreenWitchAutoPreparedSpells",
                GuidHelper.Create(GW_BASE_GUID, "GreenWitchAutoPreparedSpells").ToString(),
               new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    greenWitchSpells1, greenWitchSpells2, greenWitchSpells3, greenWitchSpells4, greenWitchSpells5},
               WitchClass, blank).AddToDB();

            var greenWitchAffinity = new FeatureDefinitionMagicAffinityBuilder(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic,
                "MagicAffinityGreenWitch",
                GuidHelper.Create(GW_BASE_GUID, "MagicAffinityGreenWitch").ToString(), blank).AddToDB();

            GuiPresentation greenWitchMagicGui = new GuiPresentationBuilder("Subclass/&GreenWitchMagicDescription", "Subclass/&GreenWitchMagicTitle").Build();
            return new FeatureDefinitionFeatureSetBuilder("GreenWitchMagic",
                GuidHelper.Create(GW_BASE_GUID, "GreenWitchMagic").ToString(),
                new List<FeatureDefinition>() { preparedSpells, greenWitchAffinity },
                FeatureDefinitionFeatureSet.FeatureSetMode.Union, greenWitchMagicGui).AddToDB();
        }

        private sealed class FeatureDefinitionAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
        {
            public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid, List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetField("autoPreparedSpellsGroups", autospelllists);
                Definition.SetSpellcastingClass(characterclass);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        private sealed class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
        {
            public FeatureDefinitionFeatureSetBuilder(string name, string guid, List<FeatureDefinition> features,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetField("featureSet", features);
                Definition.SetMode(mode);
                Definition.SetGuiPresentation(guiPresentation);
                // enumerateInDescription and uniqueChoices default to false.
            }
        }
    }
}