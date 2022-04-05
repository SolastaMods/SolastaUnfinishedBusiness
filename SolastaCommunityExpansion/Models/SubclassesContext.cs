using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Subclasses;
using SolastaCommunityExpansion.Subclasses.Barbarian;
using SolastaCommunityExpansion.Subclasses.Druid;
using SolastaCommunityExpansion.Subclasses.Fighter;
using SolastaCommunityExpansion.Subclasses.Ranger;
using SolastaCommunityExpansion.Subclasses.Rogue;
using SolastaCommunityExpansion.Subclasses.Wizard;

namespace SolastaCommunityExpansion.Models
{
    internal static class SubclassesContext
    {
        private static Dictionary<string, FeatureDefinitionSubclassChoice> SubclassesChoiceList { get; set; } = new();

        internal static Dictionary<string, CharacterSubclassDefinition> Subclasses { get; private set; } = new();

        private static void SortSubclassesFeatures()
        {
            var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

            foreach (var characterSubclassDefinition in dbCharacterSubclassDefinition)
            {
                characterSubclassDefinition.FeatureUnlocks.Sort((a, b) =>
                {
                    var result = a.Level - b.Level;

                    if (result == 0)
                    {
                        result = a.FeatureDefinition.FormatTitle().CompareTo(b.FeatureDefinition.FormatTitle());
                    }

                    return result;
                });
            }
        }

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
            LoadSubclass(new CircleOfTheForestGuardian());

            Subclasses = Subclasses.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortSubclassesFeatures();
            }
        }

        private static void LoadSubclass(AbstractSubclass subclassBuilder)
        {
            var subclass = subclassBuilder.GetSubclass();
            var name = subclass.Name;

            if (!Subclasses.ContainsKey(name))
            {
                SubclassesChoiceList.Add(name, subclassBuilder.GetSubclassChoiceList());
                Subclasses.Add(name, subclass);
            }

            UpdateSubclassVisibility(name);
        }

        private static void UpdateSubclassVisibility(string name)
        {
            var choiceList = SubclassesChoiceList[name];

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

#if DEBUG
        public static string GenerateSubclassDescription()
        {
            var outString = new StringBuilder("[size=3][b]Subclasses[/b][/size]\n");

            outString.Append("\n[list]");

            foreach (var subclass in Subclasses.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(subclass.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(subclass.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }
}
