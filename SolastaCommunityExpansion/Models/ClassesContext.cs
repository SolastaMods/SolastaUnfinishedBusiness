using SolastaCommunityExpansion.Classes;
using SolastaCommunityExpansion.Classes.Witch;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class ClassesContext
    {
        public static Dictionary<string, AbstractClass> Classes = new Dictionary<string, AbstractClass>();

        internal static void Load()
        {
            LoadClass(new Witch());
        }

        private static void LoadClass(AbstractClass classBuilder)
        {
            CharacterClassDefinition customClass = classBuilder.GetClass();
            if (!Classes.ContainsKey(customClass.Name))
            {
                Classes.Add(customClass.Name, classBuilder);
            }

            Classes = Classes.OrderBy(x => Gui.Format(x.Value.GetClass().GuiPresentation.Title)).ToDictionary(x => x.Key, x => x.Value);
        }

        public static string GenerateClassDescription()
        {
            string outString = "[heading]Classes[/heading]";
            outString += "\n[list]";
            foreach (AbstractClass customClass in Classes.Values)
            {
                outString += "\n[*][b]" + Gui.Format(customClass.GetClass().GuiPresentation.Title) + "[/b]: " + Gui.Format(customClass.GetClass().GuiPresentation.Description);

            }
            outString += "\n[/list]";
            return outString;
        }
    }

}
