using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Models
{
    internal static class AsiAndFeatContext
    {
        internal static void Switch(bool active)
        {
            if (active)
            {
                FeatureSetAbilityScoreChoice.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            }
            else
            {
                FeatureSetAbilityScoreChoice.SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion);
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnablesAsiAndFeat);
        }
    }
}
