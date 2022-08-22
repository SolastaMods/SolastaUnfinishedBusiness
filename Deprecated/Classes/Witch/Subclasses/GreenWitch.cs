using System;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses;

internal static class GreenWitch
{
    private static readonly Guid Namespace = new("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");

    private static CharacterSubclassDefinition Subclass;

    internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
    {
        return Subclass ??= BuildAndAddSubclass(witchClass);
    }

    private static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
    {
        return WitchSubclassHelper.BuildAndAddSubclass(
            "Green",
            TraditionGreenmage.GuiPresentation.SpriteReference,
            witchClass,
            Namespace,
            BuildSpellGroup(1, Entangle, Goodberry),
            BuildSpellGroup(3,
                Barkskin,
                ProtectionFromPoison), // This should be Beast Sense
            BuildSpellGroup(5,
                ConjureAnimals,
                CreateFood), // This should be Plant Growth
            BuildSpellGroup(7,
                GiantInsect, // This should be Conjure Woodland Beings
                Stoneskin),
            BuildSpellGroup(9,
                DispelEvilAndGood, // This should be Awaken
                InsectPlague) // This should be Tree Stride
        );
    }
}
