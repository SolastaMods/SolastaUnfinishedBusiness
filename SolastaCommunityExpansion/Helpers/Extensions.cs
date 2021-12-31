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
            return (bool)CheckConditionName.Invoke(gadget, new object[] { Invisible, true, false });
        }

        /// <summary>
        /// Replacement for buggy GameGadget.CheckIsEnabled().
        /// </summary>
        public static bool IsEnabled(this GameGadget gadget)
        {
            return (bool)CheckConditionName.Invoke(gadget, new object[] { ParamEnabled, true, false })
                || (bool)CheckConditionName.Invoke(gadget, new object[] { Enabled, true, false });
        }

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        private static readonly MethodInfo CheckConditionName
            = typeof(GameGadget).GetMethod("CheckConditionName", BindingFlags.Instance | BindingFlags.NonPublic);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
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
