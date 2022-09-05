using System.Collections.Generic;

namespace SolastaCommunityExpansion.Api.Extensions;

internal static class SpellListDefinitionExtensions
{
    public static void AddSpell(this SpellListDefinition list, SpellDefinition spell)
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
}
