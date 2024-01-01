using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static SpellListDefinition;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class SpellListDefinitionBuilder : DefinitionBuilder<SpellListDefinition, SpellListDefinitionBuilder>
{
    [NotNull]
    internal SpellListDefinitionBuilder ClearSpells()
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
                Definition.SpellsByLevel.Add(new SpellsByLevelDuplet { Level = level, Spells = [] });
            }

            // Check this level matches
            var spells = Definition.SpellsByLevel[level];

            if (spells.Level != level)
            {
                throw new InvalidOperationException($"Spell list not configured correctly for level={level}");
            }

            // Ensure spells list is set
            spells.Spells ??= [];
        }
    }

    [NotNull]
    internal SpellListDefinitionBuilder SetSpellsAtLevel(int level, [NotNull] params SpellDefinition[] spells)
    {
        if (level is > 9 or < 0)
        {
            throw new ArgumentException($"Spell level {level} is not supported.");
        }

#if DEBUG
        if (spells.GroupBy(s => s.GUID).Any(g => g.Count() > 1))
        {
            throw new ArgumentException(
                $"{Definition.Name}. There are duplicate spells in the supplied level {level} spell list.");
        }
#endif

        // Set the spells - remove duplicates - sort to add to list in deterministic order
        Definition.SpellsByLevel[level].Spells
            .SetRange(spells
                .Where(s => s.Implemented)
                .OrderBy(s => s.Name)
                .Distinct());

        return this;
    }

    /// <summary>
    ///     Sets the max spell level and whether this list has cantrips
    ///     calculated from the spells currently in the list.
    /// </summary>
    /// <returns></returns>
    [NotNull]
    internal SpellListDefinitionBuilder FinalizeSpells(bool hasCantrip = true, int maxLevel = 0)
    {
        // Will throw if anything incorrect
        EnsureSpellListsConfigured();

        Definition.hasCantrips = hasCantrip;
        Definition.maxSpellLevel = maxLevel;
        return this;
    }

    #region Constructors

    protected SpellListDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected SpellListDefinitionBuilder(SpellListDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
