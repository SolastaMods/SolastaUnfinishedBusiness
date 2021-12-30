using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Helpers
{
    internal static class Extensions
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

    public enum ExtraRitualCasting
    {
        None = RuleDefinitions.RitualCasting.None,
        Prepared = RuleDefinitions.RitualCasting.Prepared,
        Spellbook = RuleDefinitions.RitualCasting.Spellbook,
        Known = 9000
    }

    public enum ExtraTargetFilteringTags
    {
        No = RuleDefinitions.TargetFilteringTag.No,
        Unarmored = RuleDefinitions.TargetFilteringTag.Unarmored,
        MetalArmor = RuleDefinitions.TargetFilteringTag.MetalArmor,
        CursedByMalediction = 9000
    }

    public enum ExtraOriginOfAmount
    {
        None = 0,
        SourceDamage = 1,
        SourceGain = 2,
        AddDice = 3,
        Fixed = 4,
        SourceHalfHitPoints = 5,
        SourceSpellCastingAbility = 6,
        SourceSpellAttack = 7,
        SourceProficiencyBonus = 9000,
        SourceCharacterLevel = 9001,
        SourceClassLevel = 9002
    }

}
