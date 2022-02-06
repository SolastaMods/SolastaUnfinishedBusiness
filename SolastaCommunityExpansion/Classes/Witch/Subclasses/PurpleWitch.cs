using System;
using SolastaCommunityExpansion.Builders.Features;
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
            );
        }
    }
}
