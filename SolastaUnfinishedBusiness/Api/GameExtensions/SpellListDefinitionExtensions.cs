using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class SpellListDefinitionExtensions
{
    internal static void AddSpell(this SpellListDefinition list, SpellDefinition spell)
    {
        if (list.ContainsSpell(spell))
        {
            return;
        }

        var index = list.spellsByLevel.FindIndex(d => d.level == spell.spellLevel);

        if (index < 0)
        {
            list.spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                level = spell.spellLevel, spells = new List<SpellDefinition> { spell }
            });
        }
        else
        {
            list.spellsByLevel[index].spells.TryAdd(spell);
        }
    }

    internal static IEnumerable<SpellDefinition> GetSpellsOfLevels(this SpellListDefinition list, params int[] levels)
    {
        return list.spellsByLevel
            .Where(d => levels.Contains(d.level))
            .SelectMany(d => d.spells);
    }
}
