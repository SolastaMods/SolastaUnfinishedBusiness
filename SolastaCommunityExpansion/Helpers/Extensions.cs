using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        /// Makes using RulesetActor.EnumerateFeaturesToBrowse simpler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actor"></param>
        /// <param name="featuresOrigin"></param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateFeaturesToBrowse<T>(
            this RulesetActor actor, Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
        {
            var features = new List<FeatureDefinition>();
            actor.EnumerateFeaturesToBrowse<T>(features, featuresOrigin);
            return features.OfType<T>().ToList();
        }
    }

    public enum ExtraRitualCasting
    {
        None = RuleDefinitions.RitualCasting.None,
        Prepared = RuleDefinitions.RitualCasting.Prepared,
        Spellbook = RuleDefinitions.RitualCasting.Spellbook,
        Known = 1337
    }

    public enum ExtraTargetFilteringTags
    {
        No = RuleDefinitions.TargetFilteringTag.No,
        Unarmored = RuleDefinitions.TargetFilteringTag.Unarmored,
        MetalArmor = RuleDefinitions.TargetFilteringTag.MetalArmor,
        CursedByMalediction = 1337
    }
}
