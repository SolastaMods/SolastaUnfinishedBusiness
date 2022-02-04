using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class PurpleWitch
    {
        private static readonly Guid Namespace = new("bb8a01e8-7997-4c44-8643-72ac15853b47");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("PurpleMagicAutoPreparedSpell", Namespace)
                .SetGuiPresentationNoContent()
                .SetPreparedSpellGroups(
                    AutoPreparedSpellsGroupBuilder.Build(1,
                        CharmPerson,
                        HideousLaughter),   // This should be Silent Image
                    AutoPreparedSpellsGroupBuilder.Build(3,
                        CalmEmotions, // This should be Enthrall
                        Invisibility),
                    AutoPreparedSpellsGroupBuilder.Build(5,
                        HypnoticPattern,
                        Fear),  // This should be Major Image
                    AutoPreparedSpellsGroupBuilder.Build(7,
                        Confusion,
                        PhantasmalKiller),   // This should be Private Sanctum
                    AutoPreparedSpellsGroupBuilder.Build(9,
                        DominatePerson, // This should be Modify Memory
                        HoldMonster)    // This should be Seeming
                )
                .SetCharacterClass(witchClass)
                .SetAutoTag("Coven")
                .AddToDB();

            var featureDefinitionFeatureSetPurpleMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetPurpleWitchMagic", Namespace)
                    .SetGuiPresentationGenerate("PurpleWitchMagic", Category.Subclass)
                    .SetFeatures(preparedSpells)
                    .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    .SetUniqueChoices(true)
                    .AddToDB();

            return new CharacterSubclassDefinitionBuilder("PurpleWitch", Namespace)
                .SetGuiPresentationGenerate("PurpleWitch", Category.Subclass, DomainInsight.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureDefinitionFeatureSetPurpleMagic, 3)
                .AddToDB();
        }
    }
}
