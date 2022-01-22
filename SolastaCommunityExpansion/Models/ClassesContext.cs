using SolastaCommunityExpansion.Classes;
using SolastaCommunityExpansion.Classes.Tinkerer;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class ClassesContext
    {
        internal static Dictionary<string, CharacterClassDefinition> Classes { get; private set; } = new Dictionary<string, CharacterClassDefinition>();

        internal static void SortClassesFeatures()
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            foreach (var characterClassDefinition in dbCharacterClassDefinition)
            {
                characterClassDefinition.FeatureUnlocks.Sort((a, b) =>
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
            LoadClass(TinkererClass.BuildTinkererClass());
            LoadClass(Witch.Instance);

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortClassesFeatures();
            }
        }

        private static void LoadClass(CharacterClassDefinition characterClass)
        {
            if (!Classes.ContainsKey(characterClass.Name))
            {
                Classes.Add(characterClass.Name, characterClass);
            }

            Classes = Classes.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            UpdateClassVisibility(characterClass.Name);
        }

        private static void UpdateClassVisibility(string className)
        {
            Classes[className].GuiPresentation.SetHidden(!Main.Settings.ClassEnabled.Contains(className));
        }

        internal static void Switch(string className, bool active)
        {
            if (!Classes.ContainsKey(className))
            {
                return;
            }

            if (active)
            {
                if (!Main.Settings.ClassEnabled.Contains(className))
                {
                    Main.Settings.ClassEnabled.Add(className);
                }
            }
            else
            {
                Main.Settings.ClassEnabled.Remove(className);
            }

            UpdateClassVisibility(className);
        }

        public static string GenerateClassDescription()
        {
            var outString = new StringBuilder("[heading]Classes[/heading]");

            outString.Append("\n[list]");

            foreach (var characterClass in Classes.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(characterClass.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(characterClass.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
