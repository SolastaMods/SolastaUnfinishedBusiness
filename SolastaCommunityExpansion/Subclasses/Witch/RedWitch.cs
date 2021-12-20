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
    internal class RedWitch : AbstractSubclass
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

        public static readonly Guid RW_BASE_GUID = new Guid("3cc83deb-e681-4670-9340-33d08b61f599");

        private static CharacterClassDefinition WitchClass { get; set; }
        public static FeatureDefinitionFeatureSet FeatureDefinitionFeatureSetRedMagic { get; private set; }

        private static void BuildRedMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder(
                    "RedMagicAutoPreparedSpell",
                    GuidHelper.Create(RW_BASE_GUID, "RedMagicAutoPreparedSpell").ToString(),
                    new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>{
                            FeatureDefinitionAutoPreparedSpellsBuilder.BuildAutoPreparedSpellGroup(
                                    1,
                                    new List<SpellDefinition>{
                                            DatabaseHelper.SpellDefinitions.BurningHands, 
                                            DatabaseHelper.SpellDefinitions.MagicMissile, 
                                            DatabaseHelper.SpellDefinitions.AcidArrow, 
                                            DatabaseHelper.SpellDefinitions.ScorchingRay, 
                                            DatabaseHelper.SpellDefinitions.Fireball, 
                                            DatabaseHelper.SpellDefinitions.ProtectionFromEnergy, 
                                            DatabaseHelper.SpellDefinitions.IceStorm, 
                                            DatabaseHelper.SpellDefinitions.WallOfFire, 
                                            DatabaseHelper.SpellDefinitions.ConeOfCold, 
                                            DatabaseHelper.SpellDefinitions.MindTwist, // This should be Telekinesis
                                            })},
                    blank)
                    .SetCharacterClass(WitchClass)
                    .SetAutoTag("Coven")
                    .AddToDB();

            FeatureDefinitionFeatureSetRedMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages,
                    "FeatureSetRedWitchMagic",
                    GuidHelper.Create(RW_BASE_GUID, "FeatureSetRedWitchMagic").ToString(),
                    new GuiPresentationBuilder(
                            "Subclass/&RedWitchMagicDescription",
                            "Subclass/&RedWitchMagicTitle").Build())
                    .ClearFeatures()
                    .AddFeature(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();

        }

        private static void BuildProgression(CharacterSubclassDefinitionBuilder subclassBuilder)
        {
            subclassBuilder.AddFeatureAtLevel(FeatureDefinitionFeatureSetRedMagic, 3);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClassDefinition)
        {

            WitchClass = witchClassDefinition;

            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&RedWitchDescription",
                    "Subclass/&RedWitchTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalFire.GuiPresentation.SpriteReference)
                    .Build();

            var subclassBuilder = new CharacterSubclassDefinitionBuilder(
                    "RedWitch", 
                    GuidHelper.Create(RW_BASE_GUID, "RedWitch").ToString())
                    .SetGuiPresentation(subclassGuiPresentation);

            BuildRedMagic();
            BuildProgression(subclassBuilder);

            return subclassBuilder.AddToDB();
        }
        
    }
}