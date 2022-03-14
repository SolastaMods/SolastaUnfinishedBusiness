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

        public SpellListDefinitionBuilder SetSpellsAtLevel(int level, params SpellDefinition[] spellsByLevel)
        {
            return SetSpellsByLevel(level, spellsByLevel.AsEnumerable());
        }

        public SpellListDefinitionBuilder SetSpellsByLevel(int level, IEnumerable<SpellDefinition> spells)
        {
            if (level > 9 || level < 0)
            {
                throw new ArgumentException($"Spell level {level} is not supported.");
            }

            var spellsByLevel = Definition.SpellsByLevel;

            for (int i = 0; i <= level; i++)
            {
                if (i >= spellsByLevel.Count)
                {
                    spellsByLevel.Add(new SpellsByLevelDuplet { Level = level });
                }
                else if (spellsByLevel[i] == null)
                {
                    spellsByLevel[i] = new SpellsByLevelDuplet { Level = level };
                }
            }

            spellsByLevel[level].Spells ??= new();
            spellsByLevel[level].Spells.SetRange(spells.Where(s => s.Implemented));

            return this;
        }

        public SpellListDefinitionBuilder SetMaxSpellLevel(int maxLevel, bool hasCantrips)
        {
            Definition.SetMaxSpellLevel(maxLevel);
            Definition.SetHasCantrips(hasCantrips);
            return this;
        }
    }
}
