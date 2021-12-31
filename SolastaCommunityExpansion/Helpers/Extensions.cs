using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolastaCommunityExpansion.Helpers
{
    internal static class RulesetActorExtensions
    {
        /// <summary>
        /// Makes using RulesetActor.EnumerateFeaturesToBrowse simpler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actor"></param>
        /// <param name="populateActorFeaturesToBrowse">Set to true to populate actor.FeaturesToBrowse as well as returning features.  false to just return features.</param>
        /// <param name="featuresOrigin"></param>
        /// <returns></returns>
        /// <summary>
        public static ICollection<T> EnumerateFeaturesToBrowse<T>(
            this RulesetActor actor, bool populateActorFeaturesToBrowse = false, Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
        {
            var features = populateActorFeaturesToBrowse ? actor.FeaturesToBrowse : new List<FeatureDefinition>();
            actor.EnumerateFeaturesToBrowse<T>(features, featuresOrigin);
            return features.OfType<T>().ToList();
        }
    }

    internal static class GameGadgetExtensions
    {
        public const string Enabled = "Enabled";
        public const string ParamEnabled = "Param_Enabled";
        public const string Invisible = "Invisible";

        /// <summary>
        /// Returns state of Invisible parameter, or false if not present
        /// </summary>
        public static bool IsInvisible(this GameGadget gadget)
        {
            return gadget.CheckConditionName(Invisible, true, false);
        }

        public static bool CheckConditionName(this GameGadget gadget, string name, bool value, bool valueIfMissing)
        {
            return (bool)CheckConditionNameMethod.Invoke(gadget, new object[] { name, value, valueIfMissing });
        }

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        private static readonly MethodInfo CheckConditionNameMethod
            = typeof(GameGadget).GetMethod("CheckConditionName", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    }
}
