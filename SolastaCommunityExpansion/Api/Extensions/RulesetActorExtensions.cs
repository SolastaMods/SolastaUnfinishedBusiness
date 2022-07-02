using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class RulesetActorExtensions
{
    /// <summary>
    ///     Makes using RulesetActor.EnumerateFeaturesToBrowse simpler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="actor"></param>
    /// <param name="populateActorFeaturesToBrowse">
    ///     Set to true to populate actor.FeaturesToBrowse as well as returning
    ///     features.  false to just return features.
    /// </param>
    /// <param name="featuresOrigin"></param>
    [NotNull]
    public static ICollection<T> EnumerateFeaturesToBrowse<T>(
        [NotNull] this RulesetActor actor, bool populateActorFeaturesToBrowse = false,
        [CanBeNull] Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        var features = populateActorFeaturesToBrowse ? actor.FeaturesToBrowse : new List<FeatureDefinition>();
        actor.EnumerateFeaturesToBrowse<T>(features, featuresOrigin);
        return features.OfType<T>().ToList();
    }

    [NotNull]
    private static List<T> FeaturesByType<T>([CanBeNull] RulesetActor actor) where T : class
    {
        var list = new List<FeatureDefinition>();

        actor?.EnumerateFeaturesToBrowse<T>(list);

        return list
            .Select(s => s as T)
            .Where(f => f != null)
            .ToList();
    }

    [NotNull]
    public static List<T> GetFeaturesByType<T>(this RulesetActor actor) where T : class
    {
        return FeaturesByType<T>(actor);
    }

#if false
    public static List<T> GetFeaturesByTypeAndTag<T>(this RulesetCharacterHero hero, string tag) where T : class
    {
        return hero.ActiveFeatures
            .Where(e => e.Key.Contains(tag))
            .SelectMany(e => e.Value)
            .SelectMany(Unfold)
            .OfType<T>()
            .ToList();
    }

    private static IEnumerable<FeatureDefinition> Unfold(FeatureDefinition feature)
    {
        return feature is FeatureDefinitionFeatureSet {Mode: FeatureSetMode.Union} set
            ? set.FeatureSet.SelectMany(Unfold)
            : new[] {feature};
    }
#endif

    public static bool HasAnyFeature(this RulesetActor actor, params FeatureDefinition[] features)
    {
        return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
    }

    public static bool HasAnyFeature(this RulesetActor actor, IEnumerable<FeatureDefinition> features)
    {
        return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
    }

    public static bool HasAllFeatures(this RulesetActor actor, [NotNull] params FeatureDefinition[] features)
    {
        return HasAllFeatures(actor, features.ToList());
    }

    public static bool HasAllFeatures(this RulesetActor actor, [NotNull] IEnumerable<FeatureDefinition> features)
    {
        var all = FeaturesByType<FeatureDefinition>(actor);
        return FlattenFeatureList(features).All(f => all.Contains(f));
    }

    [NotNull]
    public static IEnumerable<FeatureDefinition> FlattenFeatureList([NotNull] IEnumerable<FeatureDefinition> features)
    {
        //TODO: should we add FeatureDefinitionFeatureSetCustom flattening too?
        return features.SelectMany(f => f is FeatureDefinitionFeatureSet set
            ? FlattenFeatureList(set.FeatureSet)
            : new List<FeatureDefinition> {f});
    }

    [NotNull]
    public static List<T> GetSubFeaturesByType<T>(this RulesetActor actor) where T : class
    {
        return FeaturesByType<FeatureDefinition>(actor)
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .ToList();
    }

    public static bool HasSubFeatureOfType<T>(this RulesetActor actor) where T : class
    {
        return FeaturesByType<FeatureDefinition>(actor)
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .FirstOrDefault() != null;
    }
}
