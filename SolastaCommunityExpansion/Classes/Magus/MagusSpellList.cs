using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Spells.AceHighSpells;
using static SolastaCommunityExpansion.Spells.EWSpells;

namespace SolastaCommunityExpansion.Classes.Magus;

// keep public as CE:MC depends on it
public static class MagusSpells
{
     public static List<FeatureDefinitionCastSpell.SlotsByLevelDuplet> MagusCastingSlot { get; } = new()
    {
        new()
        {
            Slots = new List<int>
            {
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 01
        },
        new()
        {
            Slots = new List<int>
            {
                3,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 02
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 03
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 04
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 05
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                0,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 06
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                1,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 07
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                2,
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 08
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                1,
                0,
                0,
                0,
                0,
                0
            },
            Level = 09
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                0,
                0,
                0,
                0,
                0
            },
            Level = 10
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                0,
                0,
                0,
                0
            },
            Level = 11
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                0,
                0,
                0,
                0
            },
            Level = 12
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                1,
                0,
                0,
                0
            },
            Level = 13
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                1,
                0,
                0,
                0
            },
            Level = 14
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                1,
                1,
                0,
                0
            },
            Level = 15
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                1,
                1,
                0,
                0
            },
            Level = 16
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                2,
                1,
                1,
                1,
                1,
                0
            },
            Level = 17
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                3,
                1,
                1,
                1,
                1,
                0
            },
            Level = 18
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                3,
                2,
                1,
                1,
                1,
                0
            },
            Level = 19
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
                3,
                3,
                3,
                2,
                2,
                1,
                1,
                0
            },
            Level = 20
        }
    };
    
    internal static SpellListDefinition MagusSpellList { get; } = SpellListDefinitionBuilder
        .Create(DatabaseHelper.SpellListDefinitions.SpellListPaladin, "ClassMagusSpellList", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentationNoContent()
        .ClearSpells()
        .SetSpellsAtLevel(0, SunlightBlade, ResonatingStrike, TrueStrike, Sparkle, Light, FireBolt, ChillTouch, RayOfFrost, ShockingGrasp)
        .SetSpellsAtLevel(1, Shield, MageArmor, ExpeditiousRetreat, ProtectionFromEvilGood, HellishRebukeSpell, PactMarkSpell, Sleep, Bane)
        .SetSpellsAtLevel(2, SeeInvisibility, HoldPerson, Invisibility, MistyStep, RayOfEnfeeblement, Shatter, SpiderClimb, ScorchingRay)
        .SetSpellsAtLevel(3, Counterspell, DispelMagic, Fear, Fly, Slow, Haste, BestowCurse, StinkingCloud)
        .SetSpellsAtLevel(4, Banishment, Blight, DimensionDoor, GreaterInvisibility, PhantasmalKiller)
        .SetSpellsAtLevel(5, HoldMonster, MindTwist, ConjureElemental, Contagion)
        .SetSpellsAtLevel(6, Sunbeam, Disintegrate, GlobeOfInvulnerability, Eyebite)
        .FinalizeSpells()
        .AddToDB();
}
