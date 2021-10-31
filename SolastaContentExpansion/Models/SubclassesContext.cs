using SolastaContentExpansion.Subclasses;
using SolastaContentExpansion.Subclasses.Fighter;
using SolastaContentExpansion.Subclasses.Rogue;
using System.Collections.Generic;

namespace SolastaContentExpansion.Models
{
    class SubclassesContext
    {
        public static SortedDictionary<string, AbstractSubclass> Subclasses = new SortedDictionary<string, AbstractSubclass>();

        internal static void Load()
        {
            LoadSubclass(new SpellShield());
            LoadSubclass(new ConArtist());
        }

        private static void LoadSubclass(AbstractSubclass subclassBuilder)
        {
            CharacterSubclassDefinition subclass = subclassBuilder.GetSubclass();
            if (!Subclasses.ContainsKey(subclass.Name))
            {
                Subclasses.Add(subclass.Name, subclassBuilder);
            }

            UpdateSubclassVisibility(subclass.Name);
        }

        private static void UpdateSubclassVisibility(string name)
        {
            FeatureDefinitionSubclassChoice choiceList = Subclasses[name].GetSubclassChoiceList();
            if (Main.Settings.SubclassHidden.Contains(name))
            {
                if (choiceList.Subclasses.Contains(name))
                {
                    choiceList.Subclasses.Remove(name);
                }
            } else
            {
                if (!choiceList.Subclasses.Contains(name))
                {
                    choiceList.Subclasses.Add(name);
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
                Main.Settings.SubclassHidden.Remove(subclassName);
            }
            else
            {
                Main.Settings.SubclassHidden.Add(subclassName);
            }

            UpdateSubclassVisibility(subclassName);
        }
    }

}
