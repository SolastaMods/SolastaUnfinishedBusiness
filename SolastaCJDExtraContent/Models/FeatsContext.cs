
using SolastaCJDExtraContent.Feats;
using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCJDExtraContent.Models
{
    class FeatsContext
    {
        public static Dictionary<string, FeatDefinition> Feats = new Dictionary<string, FeatDefinition>();

        internal static void Load()
        {
            // Generate feats here and fill the list
            List<FeatDefinition> feats = new List<FeatDefinition>();

            // Build feats
            ArmorFeats.CreateArmorFeats(feats);
            FightingStlyeFeats.CreateFeats(feats);
            OtherFeats.CreateFeats(feats);

            // Use the list of feats to get the settings and ui set up.

            foreach(FeatDefinition feat in feats)
            {
                if (!Feats.ContainsKey(feat.Name))
                {
                    Feats.Add(feat.Name, feat);
                }

                feat.GuiPresentation.SetHidden(Main.Settings.FeatHidden.Contains(feat.Name));
            }
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
                Main.Settings.FeatHidden.Remove(featName);
            }
            else
            {
                Main.Settings.FeatHidden.Add(featName);
            }
        }
    }
}
