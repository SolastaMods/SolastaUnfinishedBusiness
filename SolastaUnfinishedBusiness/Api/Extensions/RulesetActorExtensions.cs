using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class RulesetActorExtensions
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
    internal static ICollection<T> EnumerateFeaturesToBrowse<T>(
        [NotNull] this RulesetActor actor,
        bool populateActorFeaturesToBrowse = false,
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
    internal static List<T> GetFeaturesByType<T>(this RulesetActor actor) where T : class
    {
        return FeaturesByType<T>(actor);
    }

    [NotNull]
    private static IEnumerable<BaseDefinition> AllActiveDefinitions([CanBeNull] RulesetActor actor)
    {
        var list = FeaturesByType<BaseDefinition>(actor);

        //TODO: add other non-feature sources of sub-features like invocations, fighting styles or metamagic if necessary
        if (actor is RulesetCharacterHero hero)
        {
            list.AddRange(hero.trainedFeats);
        }

        return list;
    }

#if false
    internal static List<T> GetFeaturesByTypeAndTag<T>(this RulesetCharacterHero hero, string tag) where T : class
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

    internal static bool HasAnyFeature(this RulesetActor actor, params FeatureDefinition[] features)
    {
        return FeaturesByType<FeatureDefinition>(actor).Any(features.Contains);
    }

#if false
    internal static bool HasAllFeatures(this RulesetActor actor, [NotNull] params FeatureDefinition[] features)
    {
        var all = FeaturesByType<FeatureDefinition>(actor);
        return FlattenFeatureList(features).All(f => all.Contains(f));
    }
#endif

    [NotNull]
    public static IEnumerable<FeatureDefinition> FlattenFeatureList([NotNull] IEnumerable<FeatureDefinition> features)
    {
        return features.SelectMany(f =>
            f is FeatureDefinitionFeatureSet set
                ? FlattenFeatureList(set.FeatureSet)
                : new List<FeatureDefinition> { f });
    }

    [NotNull]
    internal static List<T> GetSubFeaturesByType<T>(this RulesetActor actor, params Type[] typesToSkip) where T : class
    {
        return AllActiveDefinitions(actor)
            .Where(f => !typesToSkip.Contains(f.GetType()))
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .ToList();
    }

    internal static bool HasSubFeatureOfType<T>(this RulesetActor actor, params Type[] typesToSkip) where T : class
    {
        return AllActiveDefinitions(actor)
            .Where(f => !typesToSkip.Contains(f.GetType()))
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .FirstOrDefault() != null;
    }

    internal static float DistanceTo(this RulesetActor actor, RulesetActor target)
    {
        var locA = GameLocationCharacter.GetFromActor(actor);
        var locB = GameLocationCharacter.GetFromActor(target);

        if (locA == null || locB == null)
        {
            return 0;
        }

        var service = ServiceRepository.GetService<IGameLocationPositioningService>();

        return Vector3.Distance(service.ComputeGravityCenterPosition(locA), service.ComputeGravityCenterPosition(locB));
    }

    internal static bool IsTouchingGround(this RulesetActor actor)
    {
        return !actor.HasConditionOfType(RuleDefinitions.ConditionFlying)
               && !actor.HasConditionOfType(RuleDefinitions.ConditionLevitate)
               && !(actor is RulesetCharacter character &&
                    character.MoveModes.ContainsKey((int)RuleDefinitions.MoveMode.Fly));
    }

    internal static bool HasAnyConditionOfType(this RulesetActor actor, params string[] conditions)
    {
        return conditions.Any(actor.HasConditionOfType);
    }
}
