using SolastaCommunityExpansion.Spells;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class SpellsContext
    {
        private static List<SpellDefinition> Spells { get; set; } = new List<SpellDefinition>();

        private static IEnumerable<CharacterClassDefinition> GetCasterClasses =>
            DatabaseRepository.GetDatabase<CharacterClassDefinition>().Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell));

        private static IEnumerable<CharacterSubclassDefinition> GetCasterSubclasses =>
            DatabaseRepository.GetDatabase<CharacterSubclassDefinition>().Where(x => x.FeatureUnlocks.Exists(y => y.FeatureDefinition is FeatureDefinitionCastSpell));

        internal static void Load()
        {
            BazouSpells.CreateSpells(Spells);

            var spellNames = Spells.Select(x => x.Name);

            foreach (var casterClass in GetCasterClasses.Where(x => !Main.Settings.ClassSpellEnabled.ContainsKey(x.Name)))
            {
                Main.Settings.ClassSpellEnabled.Add(casterClass.Name, spellNames.ToList());
            }

            foreach (var casterSubclass in GetCasterSubclasses.Where(x => !Main.Settings.SubclassSpellEnabled.ContainsKey(x.Name)))
            {
                Main.Settings.SubclassSpellEnabled.Add(casterSubclass.Name, spellNames.ToList());
            }
        }

        public static string GenerateSpellsDescription()
        {
            var outString = new StringBuilder("[heading]Spells[/heading]");

            outString.Append("\n[list]");

            foreach (var spell in Spells)
            {
                outString.Append("\n[*][b]");
                outString.Append(spell.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(spell.FormatTitle());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
