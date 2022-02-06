using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class WitchSubclassHelper
    {
        public static CharacterSubclassDefinition BuildAndAddSubclass(
            string color, AssetReferenceSprite sprite, CharacterClassDefinition witchClass, Guid namespaceGuid, params AutoPreparedSpellsGroup[] autoSpellLists)
        {
            var preparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
                .Create($"{color}MagicAutoPreparedSpell", namespaceGuid)
                .SetGuiPresentationNoContent()
                .SetPreparedSpellGroups(autoSpellLists)
                .SetCharacterClass(witchClass)
                .SetAutoTag("Coven")
                .AddToDB();

            var featureSet = FeatureDefinitionFeatureSetBuilder
                .Create(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetHumanLanguages, $"FeatureSet{color}WitchMagic", namespaceGuid)
                .SetGuiPresentation($"{color}WitchMagic", Category.Subclass)
                .SetFeatures(preparedSpells)
                .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                .SetUniqueChoices(true)
                .AddToDB();

            return CharacterSubclassDefinitionBuilder
                .Create($"{color}Witch", namespaceGuid)
                .SetGuiPresentation(Category.Subclass, sprite)
                .AddFeatureAtLevel(featureSet, 3)
                .AddToDB();
        }
    }
}
