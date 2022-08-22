#if false
using System;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
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
            return WitchSubclassHelper.BuildAndAddSubclass(
                "Blood",
                DomainOblivion.GuiPresentation.SpriteReference,
                witchClass,
                Namespace,
                BuildSpellGroup(1,
                    FalseLife, // This should be Hellish Rebuke
                    InflictWounds),// This should be Hollowing Curse,
                BuildSpellGroup(3, AcidArrow, HoldPerson),
                BuildSpellGroup(5,
                    BestowCurse, // This should be Rube-Eye Curse
                    VampiricTouch),
                BuildSpellGroup(7, Blight, DominateBeast),
                BuildSpellGroup(9, CloudKill, DominatePerson)
            );
        }
    }
}
#endif
