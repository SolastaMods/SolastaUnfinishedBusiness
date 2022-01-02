using SolastaCommunityExpansion.Feats;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using static SolastaModApi.DatabaseHelper.FeatDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class FeatsContext
    {
        public static Dictionary<string, FeatDefinition> Feats { get; private set; } = new Dictionary<string, FeatDefinition>();

        internal static List<FeatDefinition> GetAllUnofficialFeats()
        {
            var officialFeatNames = typeof(SolastaModApi.DatabaseHelper.FeatDefinitions)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.PropertyType == typeof(FeatDefinition))
                .Select(f => f.Name).ToList();

            return DatabaseRepository.GetDatabase<FeatDefinition>()
                .Where(f => !officialFeatNames.Contains(f.Name)).ToList();
        }

        internal static void Load()
        {
            // Generate feats here and fill the list
            List<FeatDefinition> feats = new List<FeatDefinition>();

            // Build feats
            AcehighFeats.CreateFeats(feats);
            ArmorFeats.CreateArmorFeats(feats);
            CasterFeats.CreateFeats(feats);
            FightingStlyeFeats.CreateFeats(feats);
            OtherFeats.CreateFeats(feats);
            HealingFeats.CreateFeats(feats);
            PickPocketContext.CreateFeats(feats);
            CraftyFeats.CreateFeats(feats);
            ElAntoniousFeats.CreateFeats(feats);

            if (Main.Settings.AllowDisplayAllUnofficialContent)
            {
                feats = GetAllUnofficialFeats();
            }

            // Use the list of feats to get the settings and ui set up.

            foreach (FeatDefinition feat in feats)
            {
                if (!Feats.ContainsKey(feat.Name))
                {
                    Feats.Add(feat.Name, feat);
                }

                feat.GuiPresentation.SetHidden(!Main.Settings.FeatEnabled.Contains(feat.Name));
            }

            Feats = Feats.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            GuiWrapperContext.RecacheFeats();
        }

        internal static void Switch(string featName, bool active)
        {
            if (!Feats.ContainsKey(featName))
            {
                return;
            }

            Feats[featName].GuiPresentation.SetHidden(!active);

            if (active)
            {
                if (!Main.Settings.FeatEnabled.Contains(featName))
                {
                    Main.Settings.FeatEnabled.Add(featName);
                }
            }
            else
            {
                Main.Settings.FeatEnabled.Remove(featName);
            }
        }

        public static string GenerateFeatsDescription()
        {
            var outString = new StringBuilder("[heading]Feats[/heading]");

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
    }
}
