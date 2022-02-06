using System;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class GreenWitch
    {
        private static readonly Guid Namespace = new("5d595308-bcf8-4a9f-a9a0-d2ae85c243e7");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            return WitchSubclassHelper.BuildAndAddSubclass(
                "Green",
                TraditionGreenmage.GuiPresentation.SpriteReference,
                witchClass,
                Namespace,
                AutoPreparedSpellsGroupBuilder.Build(1, Entangle, Goodberry),
                AutoPreparedSpellsGroupBuilder.Build(3,
                    Barkskin,
                    ProtectionFromPoison),// This should be Beast Sense
                AutoPreparedSpellsGroupBuilder.Build(5,
                    ConjureAnimals,
                    CreateFood), // This should be Plant Growth
                AutoPreparedSpellsGroupBuilder.Build(7,
                    GiantInsect, // This should be Conjure Woodland Beings
                    Stoneskin),
                AutoPreparedSpellsGroupBuilder.Build(9,
                    DispelEvilAndGood, // This should be Awaken
                    InsectPlague)   // This should be Tree Stride
            );
        }
    }
}
