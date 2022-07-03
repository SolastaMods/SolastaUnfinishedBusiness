using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses;

internal static class WitchSubclassHelper
{
    public static CharacterSubclassDefinition BuildAndAddSubclass(
        string color, AssetReferenceSprite sprite, CharacterClassDefinition witchClass, Guid namespaceGuid,
        params AutoPreparedSpellsGroup[] autoSpellLists)
    {
        var preparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"{color}MagicAutoPreparedSpell", namespaceGuid)
            // .SetGuiPresentationNoContent()
            .SetGuiPresentation($"Subclass/&{color}WitchMagicTitle", $"Subclass/&{color}WitchMagicDescription")
            .SetPreparedSpellGroups(autoSpellLists)
            .SetCastingClass(witchClass)
            .SetAutoTag("Coven")
            .AddToDB();

        return CharacterSubclassDefinitionBuilder
            .Create($"{color}Witch", namespaceGuid)
            .SetGuiPresentation(Category.Subclass, sprite)
            .AddFeatureAtLevel(preparedSpells, 3)
            .AddToDB();
    }
}
