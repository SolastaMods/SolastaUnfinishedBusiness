using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public sealed class SpellListDefinitionBuilder : DefinitionBuilder<SpellListDefinition>
    {
        private SpellListDefinitionBuilder(SpellListDefinition original, string name, Guid guidNamespace)
            : base(original, name, guidNamespace)
        {
        }

        public static SpellListDefinitionBuilder Create(SpellListDefinition original, string name, Guid guidNamespace)
        {
            return new SpellListDefinitionBuilder(original, name, guidNamespace);
        }

        public SpellListDefinitionBuilder ClearSpells()
        {
            Definition.SpellsByLevel.ForEach(s => s.Spells.Clear());
            return this;
        }

        public SpellListDefinitionBuilder SetSpellsAtLevel(int level, params SpellDefinition[] spellsByLevel)
        {
            return SetSpellsByLevel(level, spellsByLevel.AsEnumerable());
        }

        public SpellListDefinitionBuilder SetSpellsByLevel(int level, IEnumerable<SpellDefinition> spellsByLevel)
        {
            if(level >= Definition.SpellsByLevel.Count)
            {
                throw new ArgumentException($"Spell level {level} is not supported.");
            }

            Definition.SpellsByLevel[level].Spells.SetRange(
                spellsByLevel
                    .Where(s => s.Implemented)
                    .Where(s => s.ContentPack == GamingPlatformDefinitions.ContentPack.BaseGame));

            return this;
        }

        public SpellListDefinitionBuilder SetMaxSpellLevel(int maxLevel, bool hasCantrips)
        {
            Definition.SetMaxSpellLevel(maxLevel);
            Definition.SetHasCantrips(hasCantrips);
            return this;
        }

        /*        public SpellListDefinitionBuilder SetSpellsByLevel(params IEnumerable<SpellDefinition>[] spellsByLevel)
                {
                    for (int i = 0; i < Definition.SpellsByLevel.Count; i++)
                    {
                        Definition.SpellsByLevel[i].Spells.Clear();

                        if (spellsByLevel.Length > i)
                        {
                            Definition.SpellsByLevel[i].Spells.AddRange(
                                spellsByLevel[i]
                                    .Where(s => s.Implemented)
                                    .Where(s => s.ContentPack == GamingPlatformDefinitions.ContentPack.BaseGame));
                        }
                    }

                    return this;
                }
        */
    }
}
