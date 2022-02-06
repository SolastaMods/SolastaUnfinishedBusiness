using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SolastaCommunityExpansion.Feats;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class FeatsContext
    {
        internal static bool HasFeatsFromOtherMods { get; private set; }

        internal static Dictionary<FeatDefinition, bool> Feats { get; private set; } = new Dictionary<FeatDefinition, bool>();

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
            FightingStyleFeats.CreateFeats(feats);
            OtherFeats.CreateFeats(feats);
            HealingFeats.CreateFeats(feats);
            PickPocketContext.CreateFeats(feats);
            CraftyFeats.CreateFeats(feats);
            ElAntoniousFeats.CreateFeats(feats);

            // Use the list of all unofficial feats to get the settings and ui set up
            foreach (FeatDefinition feat in GetAllUnofficialFeats())
            {
                var isFromOtherMod = !feats.Contains(feat);

                if (isFromOtherMod && !Main.Settings.AllowDisplayAllUnofficialContent)
                {
                    continue;
                }

                if (!Feats.ContainsKey(feat))
                {
                    Feats.Add(feat, isFromOtherMod);
                }

                feat.GuiPresentation.SetHidden(!Main.Settings.FeatEnabled.Contains(feat.Name));
            }

            Feats = Feats.OrderBy(x => x.Key.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);
            HasFeatsFromOtherMods = Feats.Any(x => x.Value);
        }

        internal static void Switch(FeatDefinition feat, bool active)
        {
            if (!Feats.ContainsKey(feat))
            {
                return;
            }

            feat.GuiPresentation.SetHidden(!active);

            if (active)
            {
                if (!Main.Settings.FeatEnabled.Contains(feat.Name))
                {
                    Main.Settings.FeatEnabled.Add(feat.Name);
                }
            }
            else
            {
                Main.Settings.FeatEnabled.Remove(feat.Name);
            }

            GuiWrapperContext.RecacheFeats();
        }

        public static string GenerateFeatsDescription()
        {
            var outString = new StringBuilder("[heading]Feats[/heading]");

            outString.Append("\n[list]");

            foreach (var feat in Feats.Keys)
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
