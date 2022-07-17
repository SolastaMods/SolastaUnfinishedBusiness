using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Magus;

// keep public as CE:MC depends on it
public static class MagusSpells
{
    public static List<FeatureDefinitionCastSpell.SlotsByLevelDuplet> MagusCastingSlot { get; } = new()
    {
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        new FeatureDefinitionCastSpell.SlotsByLevelDuplet
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
        .Create("ClassMagusSpellList", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentation("ClassMagusSpellList", Category.Class)
        .ClearSpells()
        .SetSpellsAtLevel(0, Sparkle, Light, FireBolt, ChillTouch, RayOfFrost, ShockingGrasp, TrueStrike, ShadowDagger)
        .SetSpellsAtLevel(1, Shield, MageArmor, ExpeditiousRetreat, ProtectionFromEvilGood, FeatherFall, InflictWounds,
            Sleep, BurningHands, Jump, Longstrider, GuidingBolt)
        .SetSpellsAtLevel(2, SeeInvisibility, HoldPerson, MistyStep, FlameBlade, Shatter, SpiderClimb, ScorchingRay,
            Blur, FlamingSphere, AcidArrow, MagicWeapon, Blindness, RayOfEnfeeblement)
        .SetSpellsAtLevel(3, Counterspell, DispelMagic, Fear, Fly, Slow, Haste, BestowCurse, StinkingCloud, Fireball,
            LightningBolt, ProtectionFromEnergy, SleetStorm)
        .SetSpellsAtLevel(4, Banishment, Blight, DimensionDoor, GreaterInvisibility, PhantasmalKiller)
        .SetSpellsAtLevel(5, HoldMonster, MindTwist, ConjureElemental, Contagion)
        .SetSpellsAtLevel(6, Sunbeam, Disintegrate, GlobeOfInvulnerability, Eyebite)
        .FinalizeSpells()
        .AddToDB();
}
