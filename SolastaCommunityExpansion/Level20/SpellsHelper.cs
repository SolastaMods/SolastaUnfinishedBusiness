using System.Collections.Generic;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Level20
{
    // keep public as CE:MC depends on it 
    public static class SpellsHelper
    {
        internal static void UpdateSpellLists()
        {
            var dbSpellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();

            foreach (var kvp in SpellListDefinitionList)
            {
                var spellListDefinitionName = kvp.Key;
                var maxSpellLevel = kvp.Value;

                if (dbSpellListDefinition.TryGetElement(spellListDefinitionName, out SpellListDefinition spellListDefinition))
                {
                    var accountForCantrips = spellListDefinition.HasCantrips ? 1 : 0;

                    while (spellListDefinition.SpellsByLevel.Count < maxSpellLevel + accountForCantrips)
                    {
                        spellListDefinition.SpellsByLevel.Add(
                            new SpellListDefinition.SpellsByLevelDuplet
                            {
                                Level = spellListDefinition.SpellsByLevel.Count,
                                Spells = new List<SpellDefinition>()
                            });
                    }

                    spellListDefinition.SetMaxSpellLevel(maxSpellLevel);
                }
            }
        }

        internal static readonly Dictionary<string, int> SpellListDefinitionList = new()
        {
            { "SpellListCleric", 9 },
            { "SpellListDruid", 9 },
            { "SpellListPaladin", 5 },
            { "SpellListRanger", 5 },
            { "SpellListShockArcanist", 9 },
            { "SpellListSorcerer", 9 },
            { "SpellListWizard", 9 },
            { "SpellListWizardGreenmage", 9 },
        };

        // keep public as CE:MC depends on it
        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        public static List<SlotsByLevelDuplet> FullCastingSlots { get; private set; } = new()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 01 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 02 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 03 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0, 0, 0, 0, 0, 0 }, Level = 04 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0, 0, 0, 0, 0, 0 }, Level = 05 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0, 0, 0, 0, 0, 0 }, Level = 06 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0, 0, 0, 0, 0, 0 }, Level = 07 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 2, 0, 0, 0, 0, 0, 0 }, Level = 08 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 1, 0, 0, 0, 0, 0 }, Level = 09 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 0, 0, 0, 0, 0 }, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 }, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 0, 0, 0, 0 }, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 }, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 0, 0, 0 }, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 }, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 0, 0 }, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 1, 1, 1, 1, 0 }, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 3, 1, 1, 1, 1, 0 }, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 3, 2, 1, 1, 1, 0 }, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 3, 2, 2, 1, 1, 0 }, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static List<SlotsByLevelDuplet> HalfCastingSlots { get; private set; } = new()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> { 0, 0, 0, 0, 0, 0 }, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 2, 0, 0, 0, 0, 0 }, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0, 0 }, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0, 0 }, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0, 0 }, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0, 0 }, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0, 0 }, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0, 0 }, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0, 0 }, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0, 0 }, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0, 0 }, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0, 0 }, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0, 0 }, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0, 0 }, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 2, 0, 0 }, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 2, 0, 0 }, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 1, 0 }, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 1, 0 }, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 0 }, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 0 }, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static List<SlotsByLevelDuplet> ArtificerCastingSlots { get; private set; } = new()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> { 2, 0, 0, 0, 0, 0 }, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 2, 0, 0, 0, 0, 0 }, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0, 0 }, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0, 0 }, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0, 0 }, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0, 0 }, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0, 0 }, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0, 0 }, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0, 0 }, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0, 0 }, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0, 0 }, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0, 0 }, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0, 0 }, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0, 0 }, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 2, 0, 0 }, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 2, 0, 0 }, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 1, 0 }, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 1, 0 }, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 0 }, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 3, 2, 0 }, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static List<SlotsByLevelDuplet> OneThirdCastingSlots { get; private set; } = new()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> { 0, 0, 0, 0, 0 }, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 0, 0, 0, 0, 0 }, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 2, 0, 0, 0, 0 }, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0 }, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0 }, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 3, 0, 0, 0, 0 }, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0 }, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0 }, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 2, 0, 0, 0 }, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0 }, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0 }, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 0, 0, 0 }, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0 }, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0 }, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 2, 0, 0 }, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0 }, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0 }, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 0, 0 }, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0 }, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> { 4, 3, 3, 1, 0 }, Level = 20 },
        };

        internal static List<int> EmptyReplacedSpells { get; private set; } = new()
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
            0,
        };

        internal static List<int> FullCasterReplacedSpells { get; private set; } = new()
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
            1,
        };

        internal static List<int> HalfCasterReplacedSpells { get; private set; } = new()
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
            1,
        };

        internal static List<int> OneThirdCasterReplacedSpells { get; private set; } = new()
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
            1,
        };

        internal static List<int> SorcererKnownSpells { get; private set; } = new()
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
            15,
        };
    }
}
