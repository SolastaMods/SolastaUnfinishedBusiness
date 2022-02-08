using System;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Witch.Subclasses
{
    internal static class WhiteWitch
    {
        private static readonly Guid Namespace = new("2d849694-5cc9-4333-944b-e40cc1e0d0fd");

        private static CharacterSubclassDefinition Subclass;

        internal static CharacterSubclassDefinition GetSubclass(CharacterClassDefinition witchClass)
        {
            return Subclass ??= BuildAndAddSubclass(witchClass);
        }

        private static CharacterSubclassDefinition BuildAndAddSubclass(CharacterClassDefinition witchClass)
        {
            return WitchSubclassHelper.BuildAndAddSubclass(
                "White",
                DomainLife.GuiPresentation.SpriteReference,
                witchClass,
                Namespace,
                AutoPreparedSpellsGroupBuilder.Build(1, Bless, CureWounds),
                AutoPreparedSpellsGroupBuilder.Build(3, LesserRestoration, PrayerOfHealing),
                AutoPreparedSpellsGroupBuilder.Build(5, BeaconOfHope, Revivify),
                AutoPreparedSpellsGroupBuilder.Build(7, DeathWard, GuardianOfFaith),
                AutoPreparedSpellsGroupBuilder.Build(9, MassCureWounds, RaiseDead)
           );
        }
    }
}
