using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Classes.Warlock.AHSpells;
using System.Collections.Generic;
using static SolastaCommunityExpansion.Spells.SrdSpells;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    internal static class ClassWarlockSpellList
    {
        public static readonly List<SlotsByLevelDuplet> WarlockCastingSlots = new()
        {
            new() { Slots = new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 01 },
            new() { Slots = new List<int> { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 02 },
            new() { Slots = new List<int> { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 03 },
            new() { Slots = new List<int> { 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 04 },
            new() { Slots = new List<int> { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 05 },
            new() { Slots = new List<int> { 2, 2, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 06 },
            new() { Slots = new List<int> { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 }, Level = 07 },
            new() { Slots = new List<int> { 2, 2, 2, 2, 0, 0, 0, 0, 0, 0 }, Level = 08 },
            new() { Slots = new List<int> { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 }, Level = 09 },
            new() { Slots = new List<int> { 2, 2, 2, 2, 2, 0, 0, 0, 0, 0 }, Level = 10 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 11 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 12 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 13 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 14 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 15 },
            new() { Slots = new List<int> { 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 }, Level = 16 },
            new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 17 },
            new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 18 },
            new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 19 },
            new() { Slots = new List<int> { 4, 4, 4, 4, 4, 0, 0, 0, 0, 0 }, Level = 20 },
        };

        public static SpellListDefinition WarlockSpellList;

        public static void Build()
        {
            WarlockSpellList = SpellListDefinitionBuilder
                .Create(DatabaseHelper.SpellListDefinitions.SpellListWizard, "ClassWarlockSpellList", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.SpellList)
                .SetMaxSpellLevel(9, true)
                .AddToDB();
            WarlockSpellList.ClearSpellsByLevel();
            WarlockSpellList.SpellsByLevel.AddRange(new List<SpellListDefinition.SpellsByLevelDuplet>()
            {
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 0,
                    Spells = new()
                    {
                        Features.DHEldritchInvocationsBuilder.EldritchBlast,
                        DatabaseHelper.SpellDefinitions.AnnoyingBee,
                        DatabaseHelper.SpellDefinitions.ChillTouch,
                        DatabaseHelper.SpellDefinitions.DancingLights,
                        DatabaseHelper.SpellDefinitions.PoisonSpray,
                        DatabaseHelper.SpellDefinitions.TrueStrike
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 1,
                    Spells = new()
                    {
                        DatabaseHelper.SpellDefinitions.CharmPerson,
                        DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                        DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                        DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                        // seems like it is unfinished?
                        // HellishRebukeSpellBuilder.HellishRebukeSpell,
                        PactMarkSpellBuilder.PactMarkSpell
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 2,
                    Spells = new()
                    {
                        DatabaseHelper.SpellDefinitions.Darkness,
                        DatabaseHelper.SpellDefinitions.HoldPerson,
                        DatabaseHelper.SpellDefinitions.Invisibility,
                        DatabaseHelper.SpellDefinitions.MistyStep,
                        DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
                        DatabaseHelper.SpellDefinitions.Shatter,
                        DatabaseHelper.SpellDefinitions.SpiderClimb
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 3,
                    Spells = new()
                    {
                        DatabaseHelper.SpellDefinitions.Counterspell,
                        DatabaseHelper.SpellDefinitions.DispelMagic,
                        DatabaseHelper.SpellDefinitions.Fear,
                        DatabaseHelper.SpellDefinitions.Fly,
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
                        DatabaseHelper.SpellDefinitions.RemoveCurse,
                        DatabaseHelper.SpellDefinitions.Tongues,
                        DatabaseHelper.SpellDefinitions.VampiricTouch
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 4,
                    Spells =  new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Banishment,
                        DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.DimensionDoor
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 5,
                    Spells = new()
                    {
                        DatabaseHelper.SpellDefinitions.HoldMonster,
                        DatabaseHelper.SpellDefinitions.MindTwist
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 6,
                    Spells = new()
                    {
                        DatabaseHelper.SpellDefinitions.CircleOfDeath,
                        DatabaseHelper.SpellDefinitions.Eyebite,
                        DatabaseHelper.SpellDefinitions.ConjureFey,
                        DatabaseHelper.SpellDefinitions.TrueSeeing,
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 7,
                    Spells = new()
                    {
                        FingerOfDeath,
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 8,
                    Spells = new()
                    {
                        DominateMonster,
                        Feeblemind,
                        PowerWordStun,
                    }
                },
                new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = 9,
                    Spells = new()
                    {
                        Weird,
                        Foresight,
                        PowerWordKill,
                    }
                }
            });
        }
    }
}
