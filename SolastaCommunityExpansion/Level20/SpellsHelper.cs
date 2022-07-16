using System.Collections.Generic;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Level20;

// keep public as CE:MC depends on it
public static class SpellsHelper
{
    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    public static List<SlotsByLevelDuplet> FullCastingSlots { get; } = new()
    {
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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
        new SlotsByLevelDuplet
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

    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    internal static IEnumerable<SlotsByLevelDuplet> HalfCastingSlots { get; } = new List<SlotsByLevelDuplet>
    {
        new()
        {
            Slots = new List<int>
            {
                0,
                0,
                0,
                0,
                0,
                0
            },
            Level = 1
        },
        new()
        {
            Slots = new List<int>
            {
                2,
                0,
                0,
                0,
                0,
                0
            },
            Level = 2
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
                0
            },
            Level = 3
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
                0
            },
            Level = 4
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
                0
            },
            Level = 5
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
                0
            },
            Level = 6
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
                0
            },
            Level = 7
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
                0
            },
            Level = 8
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
                0
            },
            Level = 9
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
                1,
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
                1,
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
                2,
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
                2,
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
                2,
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
                2,
                0
            },
            Level = 20
        }
    };

    // game uses IndexOf(0) on these sub lists reason why the last 0 there
    internal static IEnumerable<SlotsByLevelDuplet> OneThirdCastingSlots { get; } = new List<SlotsByLevelDuplet>
    {
        new()
        {
            Slots = new List<int>
            {
                0,
                0,
                0,
                0,
                0
            },
            Level = 1
        },
        new()
        {
            Slots = new List<int>
            {
                0,
                0,
                0,
                0,
                0
            },
            Level = 2
        },
        new()
        {
            Slots = new List<int>
            {
                2,
                0,
                0,
                0,
                0
            },
            Level = 3
        },
        new()
        {
            Slots = new List<int>
            {
                3,
                0,
                0,
                0,
                0
            },
            Level = 4
        },
        new()
        {
            Slots = new List<int>
            {
                3,
                0,
                0,
                0,
                0
            },
            Level = 5
        },
        new()
        {
            Slots = new List<int>
            {
                3,
                0,
                0,
                0,
                0
            },
            Level = 6
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                2,
                0,
                0,
                0
            },
            Level = 7
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                2,
                0,
                0,
                0
            },
            Level = 8
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                2,
                0,
                0,
                0
            },
            Level = 9
        },
        new()
        {
            Slots = new List<int>
            {
                4,
                3,
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
                2,
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
                2,
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
                2,
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
                0,
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
                0,
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
                1,
                0
            },
            Level = 20
        }
    };

    internal static IEnumerable<int> EmptyReplacedSpells { get; } = new List<int>
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    internal static IEnumerable<int> FullCasterReplacedSpells { get; } = new List<int>
    {
        0,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1
    };

    internal static IEnumerable<int> HalfCasterReplacedSpells { get; } = new List<int>
    {
        0,
        0,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1
    };

    internal static IEnumerable<int> OneThirdCasterReplacedSpells { get; } = new List<int>
    {
        0,
        0,
        0,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1,
        1
    };

    internal static IEnumerable<int> SorcererKnownSpells { get; } = new List<int>
    {
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        12,
        13,
        13,
        14,
        14,
        15,
        15,
        15,
        15
    };
}
