using SolastaCommunityExpansion.Powers;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolastaCommunityExpansion.Models
{
    internal static class PowersContext
    {
        public static Dictionary<string, FeatureDefinitionPower> Powers { get; private set; } = new Dictionary<string, FeatureDefinitionPower>();

        internal static void Load()
        {
            // Generate Powers here and fill the list
            List<FeatureDefinitionPower> powers = new List<FeatureDefinitionPower>();

            // Build Powers
            BazouPowers.CreatePowers(powers);

            // Use the list of Powers to get the settings and ui set up.

            foreach (FeatureDefinitionPower power in powers)
            {
                if (!Powers.ContainsKey(power.Name))
                {
                    Powers.Add(power.Name, power);
                }

                power.GuiPresentation.SetHidden(!Main.Settings.PowerEnabled.Contains(power.Name));
            }

            Powers = Powers.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);
        }

        internal static void Switch(string powerName, bool active)
        {
            if (!Powers.ContainsKey(powerName))
            {
                return;
            }

            Powers[powerName].GuiPresentation.SetHidden(!active);

            if (active)
            {
                if (!Main.Settings.PowerEnabled.Contains(powerName))
                {
                    Main.Settings.PowerEnabled.Add(powerName);
                }
            }
            else
            {
                Main.Settings.PowerEnabled.Remove(powerName);
            }
        }

        public static string GeneratePowersDescription()
        {
            var outString = new StringBuilder("[heading]Powers[/heading]");

            outString.Append("\n[list]");

            foreach (var feat in Powers.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(feat.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(feat.FormatTitle());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
    }
}
