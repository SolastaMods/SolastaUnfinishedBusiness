using SolastaCommunityExpansion.Subclasses;
using SolastaCommunityExpansion.Subclasses.Fighter;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    class SubclassesContext
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
            } else
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
    }

}
