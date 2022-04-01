using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SpellListDefinition;

namespace SolastaCommunityExpansion.Builders
{
    public class SpellListDefinitionBuilder : DefinitionBuilder<SpellListDefinition, SpellListDefinitionBuilder>
    {
        #region Constructors
        protected SpellListDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected SpellListDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected SpellListDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected SpellListDefinitionBuilder(SpellListDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected SpellListDefinitionBuilder(SpellListDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected SpellListDefinitionBuilder(SpellListDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        protected SpellListDefinitionBuilder(SpellListDefinition original) : base(original)
        {
        }
        #endregion

        public SpellListDefinitionBuilder ClearSpells()
        {
            Definition.SpellsByLevel.ForEach(s => s.Spells.Clear());
            return this;
        }

        public SpellListDefinitionBuilder ClearSpellsAtLevel(int level)
        {
            Definition.SpellsByLevel.SingleOrDefault(s => s.Level == level)?.Spells.Clear();
            return this;
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

            var spellsByLevel = Definition.SpellsByLevel.SingleOrDefault(s => s.Level == level);

            if (spellsByLevel == null)
            {
                spellsByLevel = new SpellsByLevelDuplet
                {
                    Level = level,
                    Spells = spells.Where(s => s.Implemented).ToList()
                };

                Definition.SpellsByLevel.Add(spellsByLevel);
            }
            else
            {
                if(spellsByLevel.Spells == null)
                {
                    spellsByLevel.Spells = new();
                }

                spellsByLevel.Spells.SetRange(spells.Where(s => s.Implemented));
            }

            return this;
        }

        /// <summary>
        /// Set the max spell level and whether this list has cantrips calculated from
        /// the spells currently in the list.
        /// </summary>
        /// <returns></returns>
        public SpellListDefinitionBuilder SetMaxSpellLevel()
        {
            Definition.SetMaxSpellLevel(Definition.SpellsByLevel.Max(s => s.Level));
            Definition.SetHasCantrips(Definition.SpellsByLevel.Any(s => s.Level == 0));
            return this;
        }

        /// <summary>
        /// Explicitly set the max spell level and whether this list has cantrips
        /// </summary>
        /// <param name="maxLevel"></param>
        /// <param name="hasCantrips"></param>
        /// <returns></returns>
        public SpellListDefinitionBuilder SetMaxSpellLevel(int maxLevel, bool hasCantrips)
        {
            Definition.SetMaxSpellLevel(maxLevel);
            Definition.SetHasCantrips(hasCantrips);
            return this;
        }
    }
}
