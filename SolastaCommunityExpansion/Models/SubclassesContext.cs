using SolastaCommunityExpansion.Subclasses;
using SolastaCommunityExpansion.Subclasses.Barbarian;
using SolastaCommunityExpansion.Subclasses.Fighter;
using SolastaCommunityExpansion.Subclasses.Ranger;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class SubclassesContext
    {
        public static Dictionary<string, AbstractSubclass> Subclasses { get; private set; } = new Dictionary<string, AbstractSubclass>();

        internal static void Load()
        {
            LoadSubclass(new SpellShield());
            LoadSubclass(new ConArtist());
            LoadSubclass(new MasterManipulator());
            LoadSubclass(new SpellMaster());
            LoadSubclass(new ArcaneFighter());
            LoadSubclass(new LifeTransmuter());
            LoadSubclass(new Arcanist());
            LoadSubclass(new Tactician());
            LoadSubclass(new RoyalKnight());
            LoadSubclass(new PathOfTheLight());
            LoadSubclass(new Thug());
        }

        private static void LoadSubclass(AbstractSubclass subclassBuilder)
        {
            CharacterSubclassDefinition subclass = subclassBuilder.GetSubclass();
            if (!Subclasses.ContainsKey(subclass.Name))
            {
                Subclasses.Add(subclass.Name, subclassBuilder);
            }

            Subclasses = Subclasses.OrderBy(x => x.Value.GetSubclass().FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            UpdateSubclassVisibility(subclass.Name);
        }

        private static void UpdateSubclassVisibility(string name)
        {
            FeatureDefinitionSubclassChoice choiceList = Subclasses[name].GetSubclassChoiceList();
            if (Main.Settings.SubclassEnabled.Contains(name))
            {
                if (!choiceList.Subclasses.Contains(name))
                {
                    choiceList.Subclasses.Add(name);
                }
            }
            else
            {
                if (choiceList.Subclasses.Contains(name))
                {
                    choiceList.Subclasses.Remove(name);
                }
            }
        }

        internal static void Switch(string subclassName, bool active)
        {
            if (!Subclasses.ContainsKey(subclassName))
            {
                return;
            }

            if (active)
            {
                if (!Main.Settings.SubclassEnabled.Contains(subclassName))
                {
                    Main.Settings.SubclassEnabled.Add(subclassName);
                }
            }
            else
            {
                Main.Settings.SubclassEnabled.Remove(subclassName);
            }

            UpdateSubclassVisibility(subclassName);
        }

        public static string GenerateSubclassDescription()
        {
            var outString = new StringBuilder("[heading]Subclasses[/heading]");

            outString.Append("\n[list]");

            foreach (var subclass in Subclasses.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(subclass.GetSubclass().FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(subclass.GetSubclass().FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }

}
