using System.Collections.Generic;
using System.Linq;

namespace SolastaModApi.Extensions
{
    public static partial class RulesetActorExtensions
    {
        /// <summary>
        /// Makes using RulesetActor.EnumerateFeaturesToBrowse simpler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actor"></param>
        /// <param name="populateActorFeaturesToBrowse">Set to true to populate actor.FeaturesToBrowse as well as returning features.  false to just return features.</param>
        /// <param name="featuresOrigin"></param>
        public static ICollection<T> EnumerateFeaturesToBrowse<T>(
            this RulesetActor actor, bool populateActorFeaturesToBrowse = false, Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
        {
            var features = populateActorFeaturesToBrowse ? actor.FeaturesToBrowse : new List<FeatureDefinition>();
            actor.EnumerateFeaturesToBrowse<T>(features, featuresOrigin);
            return features.OfType<T>().ToList();
        }
        
        private static List<T> FeaturesByType<T>(RulesetActor actor) where T : class
        {
            var list = new List<FeatureDefinition>();

            actor.EnumerateFeaturesToBrowse<T>(list);

            return list
                .Select(s => s as T)
                .ToList();
        }
        
        public static List<T> GetFeaturesByType<T>(this RulesetActor actor) where T : class
        {
            return FeaturesByType<T>(actor);
        }

        public static bool HasAnyFeature(this RulesetActor actor, params FeatureDefinition[] features)
        {
            return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
        }

        public static bool HasAnyFeature(this RulesetActor actor, IEnumerable<FeatureDefinition> features)
        {
            return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
        }
        
        public static bool HasAllFeatures(this RulesetActor actor, params FeatureDefinition[] features)
        {
            var all = FeaturesByType<FeatureDefinition>(actor);
            return features.All(f => all.Contains(f));
        }
        
        public static bool HasAllFeatures(this RulesetActor actor, IEnumerable<FeatureDefinition> features)
        {
            var all = FeaturesByType<FeatureDefinition>(actor);
            return features.All(f => all.Contains(f));
        }
    }
}
