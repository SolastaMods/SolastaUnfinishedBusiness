using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class BloodWitch
    {
        private static readonly Guid Namespace = new("c9f680ec-7c79-414f-b700-eebc11863105");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            var preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("BloodMagicAutoPreparedSpell", Namespace)
                .SetGuiPresentationNoContent()
                .SetPreparedSpellGroups(
                    AutoPreparedSpellsGroupBuilder.Build(1,
                        FalseLife, // This should be Hellish Rebuke
                        InflictWounds),// This should be Hollowing Curse,
                    AutoPreparedSpellsGroupBuilder.Build(3, AcidArrow, HoldPerson),
                    AutoPreparedSpellsGroupBuilder.Build(5,
                        BestowCurse, // This should be Rube-Eye Curse
                        VampiricTouch),
                    AutoPreparedSpellsGroupBuilder.Build(7, Blight, DominateBeast),
                    AutoPreparedSpellsGroupBuilder.Build(9, CloudKill, DominatePerson))
                .SetCharacterClass(witchClass)
                .SetAutoTag("Coven")
                .AddToDB();

            var featureDefinitionFeatureSetBloodMagic = new FeatureDefinitionFeatureSetBuilder(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, "FeatureSetBloodWitchMagic", Namespace)
                .SetGuiPresentationGenerate("BloodWitchMagic", Category.Subclass)
                .SetFeatures(preparedSpells)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(true)
                .AddToDB();

            return new CharacterSubclassDefinitionBuilder("BloodWitch", Namespace)
                .SetGuiPresentationGenerate("BloodWitch", Category.Subclass, DomainOblivion.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(featureDefinitionFeatureSetBloodMagic, 3)
                .AddToDB();
        }
    }
}
