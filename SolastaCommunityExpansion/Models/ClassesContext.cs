using SolastaCommunityExpansion.Classes;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class ClassesContext
    {
        public static Dictionary<string, AbstractClass> Classes { get; private set; } = new Dictionary<string, AbstractClass>();

        internal static void Load()
        {
            //LoadClass(new Tinkerer());
            LoadClass(new Witch());
        }

        private static void LoadClass(AbstractClass classBuilder)
        {
            CharacterClassDefinition characterClass = classBuilder.GetClass();

            if (!Classes.ContainsKey(characterClass.Name))
            {
                Classes.Add(characterClass.Name, classBuilder);
            }

            Classes = Classes.OrderBy(x => x.Value.GetClass().FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            UpdateClassVisibility(characterClass.Name);
        }

        private static void UpdateClassVisibility(string className)
        {
            Classes[className].GetClass().GuiPresentation.SetHidden(!Main.Settings.ClassEnabled.Contains(className));
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
                outString.Append(characterClass.GetClass().FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(characterClass.GetClass().FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
