using System.Collections.Generic;
using System.Linq;
using static FeatureDefinitionCastSpell;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Level20
{
    internal static class SpellsHelper
    {
        internal static void FixCastSpellTables()
        {
            var dbFeatureDefinitionCastSpell = DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>();

            foreach (var featureDefinitionCastSpell in dbFeatureDefinitionCastSpell
                .Where(x => x.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Monster))
            {
                while (featureDefinitionCastSpell.KnownCantrips.Count < MOD_MAX_LEVEL + 1)
                {
                    featureDefinitionCastSpell.KnownCantrips.Add(0);
                }
                while (featureDefinitionCastSpell.KnownSpells.Count < MOD_MAX_LEVEL + 1)
                {
                    featureDefinitionCastSpell.KnownSpells.Add(0);
                }
                while (featureDefinitionCastSpell.ScribedSpells.Count < MOD_MAX_LEVEL + 1)
                {
                    featureDefinitionCastSpell.ScribedSpells.Add(0);
                }
                while (featureDefinitionCastSpell.ReplacedSpells.Count < MOD_MAX_LEVEL + 1)
                {
                    featureDefinitionCastSpell.ReplacedSpells.Add(0);
                }
            }
        }

        internal static void UpdateSpellLists()
        {
            var dbSpellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();

            foreach (var spellListDefinitionName in SpellListDefinitionList)
            {
                if (dbSpellListDefinition.TryGetElement(spellListDefinitionName, out SpellListDefinition spellListDefinition))
                {
                    var accountForCantrips = spellListDefinition.HasCantrips ? 1 : 0;

                    while (spellListDefinition.SpellsByLevel.Count < MAX_SPELL_LEVEL + accountForCantrips)
                    {
                        spellListDefinition.SpellsByLevel.Add(
                            new SpellListDefinition.SpellsByLevelDuplet
                            { 
                                Level = spellListDefinition.SpellsByLevel.Count, 
                                Spells = new List<SpellDefinition>() 
                            });
                    }
                }
            }
        }

        internal static readonly List<string> SpellListDefinitionList = new List<string>()
        {
            "SpellListCleric",
            "SpellListDruid",
            "SpellListPaladin",
            "SpellListRanger",
            "SpellListShockArcanist",
            "SpellListSorcerer",
            "SpellListWizard",
            "SpellListWizardGreenmage",
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static readonly List<SlotsByLevelDuplet> FullCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static readonly List<SlotsByLevelDuplet> HalfCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {0,0,0,0,0,0}, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0}, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0}, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0}, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0}, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0}, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0}, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0}, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0}, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0}, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static readonly List<SlotsByLevelDuplet> ArtificerCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0}, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0}, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0}, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0}, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0}, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0}, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0}, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0}, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0}, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0}, Level = 20 },
        };

        // game uses IndexOf(0) on these sub lists reason why the last 0 there
        internal static readonly List<SlotsByLevelDuplet> OneThirdCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {0,0,0,0,0}, Level = 1 },
            new SlotsByLevelDuplet() { Slots = new List<int> {0,0,0,0,0}, Level = 2 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0}, Level = 3 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0}, Level = 4 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0}, Level = 5 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0}, Level = 6 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0}, Level = 7 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0}, Level = 8 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0}, Level = 9 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0}, Level = 20 },
        };

        internal static readonly List<int> EmptyReplacedSpells = new List<int>
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

        internal static readonly List<int> FullCasterReplacedSpells = new List<int>
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

        internal static readonly List<int> HalfCasterReplacedSpells = new List<int>
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

        internal static readonly List<int> OneThirdCasterReplacedSpells = new List<int>
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

        internal static readonly List<int> SorcererKnownSpells = new List<int>
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
