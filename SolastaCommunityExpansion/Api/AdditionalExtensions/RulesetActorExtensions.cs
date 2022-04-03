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
    }
}
