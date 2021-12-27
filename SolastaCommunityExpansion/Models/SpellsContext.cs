using SolastaCommunityExpansion.Spells;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        public static Dictionary<string, SpellDefinition> Spells { get; private set; } = new Dictionary<string, SpellDefinition>();

        internal static void Load()
        {
            // Generate Spells here and fill the list
            List<SpellDefinition> spells = new List<SpellDefinition>();

            // Build Spells
            BazouSpells.CreateSpells(spells);

            // Use the list of Spells to get the settings and ui set up.

            foreach (SpellDefinition spell in spells)
            {
                if (!Spells.ContainsKey(spell.Name))
                {
                    Spells.Add(spell.Name, spell);
                }

                spell.GuiPresentation.SetHidden(!Main.Settings.SpellEnabled.Contains(spell.Name));
            }

            Spells = Spells.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);
        }

        internal static void Switch(string spellName, bool active)
        {
            if (!Spells.ContainsKey(spellName))
            {
                return;
            }

            Spells[spellName].GuiPresentation.SetHidden(!active);

            if (active)
            {
                if (!Main.Settings.SpellEnabled.Contains(spellName))
                {
                    Main.Settings.SpellEnabled.Add(spellName);
                }
            }
            else
            {
                Main.Settings.SpellEnabled.Remove(spellName);
            }
        }

        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[heading]Spells[/heading]");

            outString.Append("\n[list]");

            foreach (var feat in Spells.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(feat.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(feat.FormatTitle());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
