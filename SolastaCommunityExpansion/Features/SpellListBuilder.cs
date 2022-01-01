using SolastaModApi;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Features
{

    public class SpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {

        public SpellListBuilder(string name, string guid, string title_string, SpellListDefinition base_list, params List<SpellDefinition>[] spells_by_level) : base(DatabaseHelper.SpellListDefinitions.SpellListWizard, name, guid)
        {
            if (title_string != "")
            {
                Definition.GuiPresentation.Title = title_string;
            }

            for (int i = 0; i < Definition.SpellsByLevel.Count; i++)
            {
                Definition.SpellsByLevel[i].Spells.Clear();
                if (spells_by_level.Length > i)
                {
                    Definition.SpellsByLevel[i].Spells.AddRange(spells_by_level[i].Where(s => s.ContentPack == GamingPlatformDefinitions.ContentPack.BaseGame && s.Implemented));
                }
            }
        }

        public static SpellListDefinition createSpellList(string name, string guid, string title_string, params List<SpellDefinition>[] spells_by_level)
        {
            return new SpellListBuilder(name, guid, title_string, DatabaseHelper.SpellListDefinitions.SpellListWizard, spells_by_level).AddToDB();
        }    

    }
}