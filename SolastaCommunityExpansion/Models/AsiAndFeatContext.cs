using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class AsiAndFeatContext
    {
        internal static void Switch(bool active)
        {
            if (active)
                FeatureDefinitionFeatureSetExtensions.SetMode<FeatureDefinitionFeatureSet>(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, FeatureDefinitionFeatureSet.FeatureSetMode.Union);
            else
                FeatureDefinitionFeatureSetExtensions.SetMode<FeatureDefinitionFeatureSet>(
                    DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion);
        }

        internal static void Load()
        {
            Switch(Main.Settings.EnablesAsiAndFeat);
        }

    }
}
