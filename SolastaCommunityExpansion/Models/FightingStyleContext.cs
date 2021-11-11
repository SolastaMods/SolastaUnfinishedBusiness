using SolastaCommunityExpansion.FightingStyles;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    class FightingStyleContext
    {
        public static Dictionary<string, AbstractFightingStyle> Styles = new Dictionary<string, AbstractFightingStyle>();

        internal static void Load()
        {
            LoadStyle(new BlindFighting());
            LoadStyle(new Pugilist());
        }

        private static void LoadStyle(AbstractFightingStyle styleBuilder)
        {
            FightingStyleDefinition style = styleBuilder.GetStyle();
            if (!Styles.ContainsKey(style.Name))
            {
                Styles.Add(style.Name, styleBuilder);
            }

            Styles = Styles.OrderBy(x => Gui.Format(x.Value.GetStyle().GuiPresentation.Title)).ToDictionary(x => x.Key, x => x.Value);

            UpdateStyleVisibility(style.Name);
        }

        private static void UpdateStyleVisibility(string name)
        {
            List<FeatureDefinitionFightingStyleChoice> choiceLists = Styles[name].GetChoiceLists();
            foreach (FeatureDefinitionFightingStyleChoice choiceList in choiceLists)
            {
                if (Main.Settings.FightingStyleEnabled.Contains(name))
                {
                    if (!choiceList.FightingStyles.Contains(name))
                    {
                        choiceList.FightingStyles.Add(name);
                    }
                }
                else
                {
                    if (choiceList.FightingStyles.Contains(name))
                    {
                        choiceList.FightingStyles.Remove(name);
                    }
                }
            }
            
        }

        internal static void Switch(string styleName, bool active)
        {
            if (!Styles.ContainsKey(styleName))
            {
                return;
            }

            if (active)
            {
                if (!Main.Settings.FightingStyleEnabled.Contains(styleName))
                {
                    Main.Settings.FightingStyleEnabled.Add(styleName);
                }
            }
            else
            {
                Main.Settings.FightingStyleEnabled.Remove(styleName);
            }

            UpdateStyleVisibility(styleName);
        }

        public static string GenerateFightingStyleDescription()
        {
            string outString = "[heading]Fighting Styles[/heading]";
            outString += "\n[list]";
            foreach (AbstractFightingStyle style in Styles.Values)
            {
                outString += "\n[*][b]" + Gui.Format(style.GetStyle().GuiPresentation.Title) + "[/b]: " + Gui.Format(style.GetStyle().GuiPresentation.Description);

            }
            outString += "\n[/list]";
            return outString;
        }
    }
}

