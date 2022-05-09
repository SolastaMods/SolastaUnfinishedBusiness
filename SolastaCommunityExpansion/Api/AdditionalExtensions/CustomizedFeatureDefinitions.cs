using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Api.AdditionalExtensions
{
    public static class CustomizedFeatureDefinitions
    {
        private static readonly Dictionary<FeatureDefinition, List<object>> CustomFeatures = new();

        private static List<object> GetOrCreateForKey(FeatureDefinition feature)
        {
            if (!CustomFeatures.ContainsKey(feature))
            {
                CustomFeatures.Add(feature, new List<object>());
            }

            return CustomFeatures[feature];
        }

        private static List<object> GetForKey(FeatureDefinition feature)
        {
            if (!CustomFeatures.ContainsKey(feature))
            {
                return null;
            }

            return CustomFeatures[feature];
        }

        public static T SetCustomFeatures<T>(this T feature, params object[] features) where T : FeatureDefinition
        {
            GetOrCreateForKey(feature).SetRange(features);
            return feature;
        }

        public static IEnumerable<T> GetCustomFeaturesOfType<T>(this FeatureDefinition feature) where T : class
        {
            return GetForKey(feature)?.OfType<T>();
        }
    }
}
