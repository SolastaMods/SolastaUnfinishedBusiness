using SolastaCommunityExpansion.FightingStyles;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class FightingStyleContext
    {
        public static Dictionary<string, AbstractFightingStyle> Styles { get; private set; } = new Dictionary<string, AbstractFightingStyle>();

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
            foreach (var fightingStyles in choiceLists.Select(cl => cl.FightingStyles))
            {
                if (Main.Settings.FightingStyleEnabled.Contains(name))
                {
                    if (!fightingStyles.Contains(name))
                    {
                        fightingStyles.Add(name);
                    }
                }
                else
                {
                    if (fightingStyles.Contains(name))
                    {
                        fightingStyles.Remove(name);
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
            var outString = new StringBuilder("[heading]Fighting Styles[/heading]");
            outString.Append("\n[list]");

            foreach (AbstractFightingStyle style in Styles.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(Gui.Format(style.GetStyle().GuiPresentation.Title));
                outString.Append("[/b]: ");
                outString.Append(Gui.Format(style.GetStyle().GuiPresentation.Description));
            }

            outString.Append("\n[/list]");
            return outString.ToString();
        }
    }
}

