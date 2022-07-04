using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SpellListDefinition;

namespace SolastaCommunityExpansion.Builders;

public class SpellListDefinitionBuilder : DefinitionBuilder<SpellListDefinition, SpellListDefinitionBuilder>
{
    public SpellListDefinitionBuilder ClearSpells()
    {
        // Clear everything
        Definition.SpellsByLevel.Clear();

        // Add empty spells at level for level 0..9
        EnsureSpellListsConfigured();

        return this;
    }

    private void EnsureSpellListsConfigured()
    {
        // should be levels 0..9 in that order
        for (var level = 0; level < 10; level++)
        {
            if (Definition.SpellsByLevel.Count < level + 1)
            {
                // Add new duplet
                Definition.SpellsByLevel.Add(new SpellsByLevelDuplet
                {
                    Level = level, Spells = new List<SpellDefinition>()
                });
            }

            // Check this level matches
            var spells = Definition.SpellsByLevel[level];

            if (spells.Level != level)
            {
                throw new InvalidOperationException($"Spell list not configured correctly for level={level}");
            }

            // Ensure spells list is set
            spells.Spells ??= new List<SpellDefinition>();
        }
    }

    public SpellListDefinitionBuilder SetSpellsAtLevel(int level, params SpellDefinition[] spellsByLevel)
    {
        return SetSpellsAtLevel(level, spellsByLevel.AsEnumerable());
    }

    public SpellListDefinitionBuilder SetSpellsAtLevel(int level, IEnumerable<SpellDefinition> spells)
    {
        if (level > 9 || level < 0)
        {
            throw new ArgumentException($"Spell level {level} is not supported.");
        }

        // Ensure all levels set up
        EnsureSpellListsConfigured();

#if DEBUG
        if (spells.GroupBy(s => s.GUID).Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                $"{Definition.Name}. There are duplicate spells in the supplied level {level} spell list.");
        }
#endif

        // Set the spells - remove duplicates - sort to add to list in determistic order
        Definition.SpellsByLevel[level].Spells
            .SetRange(spells.Where(s => s.Implemented).OrderBy(s => s.Name).Distinct());

        return this;
    }

    /// <summary>
    ///     Sets the max spell level and whether this list has cantrips
    ///     calculated from the spells currently in the list.
    /// </summary>
    /// <returns></returns>
    public SpellListDefinitionBuilder FinalizeSpells()
    {
        // Will throw if anything incorrect
        EnsureSpellListsConfigured();

        var maxLevel =
            Definition.SpellsByLevel.Where(s => s.Spells.Any()).Max(s => s.Level);

        var hasCantrips =
            Definition.SpellsByLevel.Where(s => s.Spells.Any()).Any(s => s.Level == 0);

        SetMaxSpellLevel(maxLevel, hasCantrips);

        return this;
    }

    /// <summary>
    ///     Explicitly set the max spell level and whether this list has cantrips
    /// </summary>
    /// <param name="maxLevel"></param>
    /// <param name="hasCantrips"></param>
    /// <returns></returns>
    public SpellListDefinitionBuilder SetMaxSpellLevel(int maxLevel, bool hasCantrips)
    {
        Definition.maxSpellLevel = maxLevel;
        Definition.hasCantrips = hasCantrips;
        return this;
    }

    #region Constructors

    protected SpellListDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected SpellListDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected SpellListDefinitionBuilder(SpellListDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected SpellListDefinitionBuilder(SpellListDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
