using System;
using static SolastaCommunityExpansion.Builders.Features.AutoPreparedSpellsGroupBuilder;
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
            return WitchSubclassHelper.BuildAndAddSubclass(
                "Purple",
                DomainInsight.GuiPresentation.SpriteReference,
                witchClass,
                Namespace,
                BuildSpellGroup(1,
                    CharmPerson,
                    HideousLaughter),   // This should be Silent Image
                BuildSpellGroup(3,
                    CalmEmotions, // This should be Enthrall
                    Invisibility),
                BuildSpellGroup(5,
                    HypnoticPattern,
                    Fear),  // This should be Major Image
                BuildSpellGroup(7,
                    Confusion,
                    PhantasmalKiller),   // This should be Private Sanctum
                BuildSpellGroup(9,
                    DominatePerson, // This should be Modify Memory
                    HoldMonster)    // This should be Seeming
            );
        }
    }
}
