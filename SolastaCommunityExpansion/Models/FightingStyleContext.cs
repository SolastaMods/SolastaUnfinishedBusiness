using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.FightingStyles;

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

            Styles = Styles.OrderBy(x => x.Value.GetStyle().FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

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

#if DEBUG
        public static string GenerateFightingStyleDescription()
        {
            var outString = new StringBuilder("[size=3][b]Fighting Styles[/b][/size]\n");

            outString.Append("\n[list]");

            foreach (var style in Styles.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(style.GetStyle().FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(style.GetStyle().FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }
}
