using System;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Witch.Subclasses;

internal static class RedWitch
{
    private static readonly Guid Namespace = new("3cc83deb-e681-4670-9340-33d08b61f599");

    private static CharacterSubclassDefinition Subclass;

    internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
    {
        return Subclass ??= BuildAndAddSubclass(witchClass);
    }

    private static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
    {
        return WitchSubclassHelper.BuildAndAddSubclass(
            "Red",
            DomainElementalFire.GuiPresentation.SpriteReference,
            witchClass,
            Namespace,
            BuildSpellGroup(1, BurningHands, MagicMissile),
            BuildSpellGroup(3, AcidArrow, ScorchingRay),
            BuildSpellGroup(5, Fireball, ProtectionFromEnergy),
            BuildSpellGroup(7, IceStorm, WallOfFire),
            BuildSpellGroup(9, ConeOfCold, MindTwist) // This should be Telekinesis
        );
    }
}
