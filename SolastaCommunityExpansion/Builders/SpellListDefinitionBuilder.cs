using System.Collections.Generic;
using System.Linq;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    public class SpellListDefinitionBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        public SpellListDefinitionBuilder(SpellListDefinition original, string name, string guid, string title, params List<SpellDefinition>[] spellsByLevel)
            : base(original, name, guid)
        {
            // TODO: split out
            if (!string.IsNullOrEmpty(title))
            {
                Definition.GuiPresentation.Title = title;
            }

            // TODO: split out
            for (int i = 0; i < Definition.SpellsByLevel.Count; i++)
            {
                Definition.SpellsByLevel[i].Spells.Clear();

                if (spellsByLevel.Length > i)
                {
                    Definition.SpellsByLevel[i].Spells.AddRange(spellsByLevel[i].Where(s => s.ContentPack == GamingPlatformDefinitions.ContentPack.BaseGame && s.Implemented));
                }
            }
        }

        public static SpellListDefinition CreateSpellList(SpellListDefinition original, string name, string guid, string title, params List<SpellDefinition>[] spellsByLevel)
        {
            return new SpellListDefinitionBuilder(original, name, guid, title, spellsByLevel).AddToDB();
        }
    }
}
