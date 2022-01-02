using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class AsiAndFeatContext
    {
        internal static void Switch(bool active)
        {
            if (active)
            {
                FeatureDefinitionFeatureSetExtensions.SetMode(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            }
            else
            {
                FeatureDefinitionFeatureSetExtensions.SetMode(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion);
            }
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnablesAsiAndFeat);
        }

    }
}
