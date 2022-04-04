using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Feats;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class FeatsContext
    {
        internal static Dictionary<string, FeatDefinition> Feats { get; private set; } = new();

        internal static void Load()
        {
            var feats = new List<FeatDefinition>();

            // Generate feats here and fill the list
            AcehighFeats.CreateFeats(feats);
            ArmorFeats.CreateArmorFeats(feats);
            CasterFeats.CreateFeats(feats);
            FightingStyleFeats.CreateFeats(feats);
            OtherFeats.CreateFeats(feats);
            HealingFeats.CreateFeats(feats);
            PickPocketContext.CreateFeats(feats);
            CraftyFeats.CreateFeats(feats);
            ElAntoniousFeats.CreateFeats(feats);
            ZappaFeats.CreateFeats(feats);

            feats.ForEach(f => LoadFeat(f));

            Feats = Feats.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);
        }

        private static void LoadFeat(FeatDefinition definition)
        {
            var name = definition.Name;

            if (!Feats.ContainsKey(name))
            {
                Feats.Add(name, definition);
            }

            UpdateFeatsVisibility(name);
        }

        private static void UpdateFeatsVisibility(string featName)
        {
            Feats[featName].GuiPresentation.SetHidden(!Main.Settings.FeatEnabled.Contains(featName));
           
        }

        internal static void Switch(string feat, bool active)
        {
            if (!Feats.ContainsKey(feat))
            {
                return;
            }

            if (active)
            {
                if (!Main.Settings.FeatEnabled.Contains(feat))
                {
                    Main.Settings.FeatEnabled.Add(feat);
                }
            }
            else
            {
                Main.Settings.FeatEnabled.Remove(feat);
            }

            UpdateFeatsVisibility(feat);
            GuiWrapperContext.RecacheFeats();
        }

#if DEBUG
        public static string GenerateFeatsDescription()
        {
            var outString = new StringBuilder("[size=3][b]Feats[/b][/size]\n");

            outString.Append("\n[list]");

            foreach (var feat in Feats.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(feat.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(feat.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }

}
