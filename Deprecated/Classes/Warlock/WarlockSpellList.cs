using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using static FeatureDefinitionCastSpell;
using static SolastaCommunityExpansion.Classes.Warlock.Features.EldritchInvocationsBuilder;
using static SolastaCommunityExpansion.Spells.AceHighSpells;
using static SolastaCommunityExpansion.Spells.SrdSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock;

// keep public as CE:MC depends on it
public static class WarlockSpells
{
    public const int PactMagicSlotTabIndex = -1;

    // ideally this would be immutable. Could return a copy so that it can't be accidentally modified.
    public static List<SlotsByLevelDuplet> WarlockCastingSlots { get; } = new()
    {
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                1,
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
            Level = 02
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
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
                2,
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
            Level = 04
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                2,
                2,
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
                2,
                2,
                2,
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
                2,
                2,
                2,
                2,
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
                2,
                2,
                2,
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
                2,
                2,
                2,
                2,
                2,
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
                2,
                2,
                2,
                2,
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
                3,
                3,
                3,
                3,
                3,
                0,
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
                3,
                3,
                3,
                3,
                3,
                0,
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
                3,
                3,
                3,
                3,
                3,
                0,
                0,
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
                3,
                3,
                3,
                3,
                3,
                0,
                0,
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
                3,
                3,
                3,
                3,
                3,
                0,
                0,
                0,
                0,
                0
            },
            Level = 15
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                3,
                3,
                3,
                3,
                3,
                0,
                0,
                0,
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
                4,
                4,
                4,
                4,
                0,
                0,
                0,
                0,
                0
            },
            Level = 17
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0,
                0,
                0,
                0,
                0
            },
            Level = 18
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0,
                0,
                0,
                0,
                0
            },
            Level = 19
        },
        new SlotsByLevelDuplet
        {
            Slots = new List<int>
            {
                4,
                4,
                4,
                4,
                4,
                0,
                0,
                0,
                0,
                0
            },
            Level = 20
        }
    };

    internal static SpellListDefinition WarlockSpellList { get; } = SpellListDefinitionBuilder
        .Create(DatabaseHelper.SpellListDefinitions.SpellListWizard, "ClassWarlockSpellList",
            DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentation(Category.SpellList)
        .ClearSpells()
        .SetSpellsAtLevel(0, EldritchBlast, AnnoyingBee, ChillTouch, DancingLights, PoisonSpray, TrueStrike)
        .SetSpellsAtLevel(1, CharmPerson, ComprehendLanguages, ExpeditiousRetreat, ProtectionFromEvilGood,
            HellishRebukeSpell, PactMarkSpell)
        .SetSpellsAtLevel(2, Darkness, HoldPerson, Invisibility, MistyStep, RayOfEnfeeblement, Shatter, SpiderClimb)
        .SetSpellsAtLevel(3, Counterspell, DispelMagic, Fear, Fly, HypnoticPattern, RemoveCurse, Tongues,
            VampiricTouch)
        .SetSpellsAtLevel(4, Banishment, Blight, DimensionDoor)
        .SetSpellsAtLevel(5, HoldMonster, MindTwist)
        .SetSpellsAtLevel(6, CircleOfDeath, Eyebite, ConjureFey, TrueSeeing)
        .SetSpellsAtLevel(7, FingerOfDeath)
        .SetSpellsAtLevel(8, DominateMonster, Feeblemind, PowerWordStun)
        .SetSpellsAtLevel(9, Weird, Foresight, PowerWordKill)
        .FinalizeSpells()
        .AddToDB();
}
