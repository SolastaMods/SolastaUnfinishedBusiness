using SolastaCommunityExpansion.Subclasses;
using SolastaCommunityExpansion.Subclasses.Barbarian;
using SolastaCommunityExpansion.Subclasses.Fighter;
using SolastaCommunityExpansion.Subclasses.Ranger;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class SubclassesContext
    {
        public static Dictionary<string, AbstractSubclass> Subclasses = new Dictionary<string, AbstractSubclass>();

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
        }

        private static void LoadSubclass(AbstractSubclass subclassBuilder)
        {
            CharacterSubclassDefinition subclass = subclassBuilder.GetSubclass();
            if (!Subclasses.ContainsKey(subclass.Name))
            {
                Subclasses.Add(subclass.Name, subclassBuilder);
            }

            Subclasses = Subclasses.OrderBy(x => Gui.Format(x.Value.GetSubclass().GuiPresentation.Title)).ToDictionary(x => x.Key, x => x.Value);

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
            string outString = "[heading]Subclasses[/heading]";
            outString += "\n[list]";
            foreach (AbstractSubclass subclass in Subclasses.Values)
            {
                outString += "\n[*][b]" + Gui.Format(subclass.GetSubclass().GuiPresentation.Title) + "[/b]: " + Gui.Format(subclass.GetSubclass().GuiPresentation.Description);

            }
            outString += "\n[/list]";
            return outString;
        }
    }

}
