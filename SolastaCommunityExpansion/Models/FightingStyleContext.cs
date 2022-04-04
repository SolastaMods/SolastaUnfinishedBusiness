using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.FightingStyles;

namespace SolastaCommunityExpansion.Models
{
    internal static class FightingStyleContext
    {
        private static Dictionary<string, List<FeatureDefinitionFightingStyleChoice>> FightingStylesChoiceList { get; set; } = new();

        internal static Dictionary<string, FightingStyleDefinition> FightingStyles { get; private set; } = new();

        internal static void Load()
        {
            LoadStyle(new BlindFighting());
            LoadStyle(new Crippling());
            LoadStyle(new Pugilist());
            LoadStyle(new TitanFighting());

            FightingStyles = FightingStyles.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);
        }

        private static void LoadStyle(AbstractFightingStyle styleBuilder)
        {
            var style = styleBuilder.GetStyle();
            var name = style.Name;

            if (!FightingStyles.ContainsKey(name))
            {
                FightingStylesChoiceList.Add(name, styleBuilder.GetChoiceLists());
                FightingStyles.Add(name, style);
            }

            UpdateStyleVisibility(name);
        }

        private static void UpdateStyleVisibility(string name)
        {
            var choiceLists = FightingStylesChoiceList[name];

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
            if (!FightingStyles.ContainsKey(styleName))
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

            foreach (var style in FightingStyles.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(style.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(style.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }
}
