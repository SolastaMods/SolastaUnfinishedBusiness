using System;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class RedWitch
    {
        private static readonly Guid Namespace = new("3cc83deb-e681-4670-9340-33d08b61f599");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        public static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            return WitchSubclassHelper.BuildAndAddSubclass(
                "Red",
                DomainElementalFire.GuiPresentation.SpriteReference,
                witchClass,
                Namespace,
                AutoPreparedSpellsGroupBuilder.Build(1, BurningHands, MagicMissile),
                AutoPreparedSpellsGroupBuilder.Build(3, AcidArrow, ScorchingRay),
                AutoPreparedSpellsGroupBuilder.Build(5, Fireball, ProtectionFromEnergy),
                AutoPreparedSpellsGroupBuilder.Build(7, IceStorm, WallOfFire),
                AutoPreparedSpellsGroupBuilder.Build(9, ConeOfCold, MindTwist)// This should be Telekinesis
            );
        }
    }
}
