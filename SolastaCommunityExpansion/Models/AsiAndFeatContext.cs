using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionFeatureSet;

namespace SolastaCommunityExpansion.Models
{
    internal static class AsiAndFeatContext
    {
        internal static void Switch(bool active)
        {
            if (active)
            {
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice.SetMode(FeatureSetMode.Union);
            }
            else
            {
                DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice.SetMode(FeatureSetMode.Exclusion);
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnablesAsiAndFeat);
        }
    }
}
