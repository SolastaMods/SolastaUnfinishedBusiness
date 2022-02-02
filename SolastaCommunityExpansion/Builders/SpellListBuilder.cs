using System.Collections.Generic;
using System.Linq;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    public class SpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        public SpellListBuilder(string name, string guid, string title, SpellListDefinition baseSpellList, params List<SpellDefinition>[] spellsByLevel)
            : base(baseSpellList, name, guid)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Definition.GuiPresentation.Title = title;
            }

            for (int i = 0; i < Definition.SpellsByLevel.Count; i++)
            {
                Definition.SpellsByLevel[i].Spells.Clear();

                if (spellsByLevel.Length > i)
                {
                    Definition.SpellsByLevel[i].Spells.AddRange(spellsByLevel[i].Where(s => s.ContentPack == GamingPlatformDefinitions.ContentPack.BaseGame && s.Implemented));
                }
            }
        }

        public static SpellListDefinition CreateSpellList(string name, string guid, string title, SpellListDefinition baseSpellList, params List<SpellDefinition>[] spellsByLevel)
        {
            return new SpellListBuilder(name, guid, title, baseSpellList, spellsByLevel).AddToDB();
        }
    }
}
