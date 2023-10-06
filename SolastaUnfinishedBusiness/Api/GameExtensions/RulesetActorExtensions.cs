using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

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
        [CanBeNull] Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin = null)
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
            .OfType<T>()
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

        RulesetCharacterHero hero = null;

        switch (actor)
        {
            case RulesetCharacterHero rulesetCharacterHero:
                hero = rulesetCharacterHero;
                break;
            //WILDSHAPE: Original hero features
            case RulesetCharacterMonster { originalFormCharacter: RulesetCharacterHero rulesetCharacterHero }:
                hero = rulesetCharacterHero;
                list.AddRange(FeaturesByType<BaseDefinition>(hero)
                    .Where(f => !list.Contains(f))
                    .ToList());
                break;
        }

        if (hero == null)
        {
            return list;
        }

        list.AddRange(hero.trainedFeats);
        // metamagic sub-features processed when they are selected for spell cast
        // list.AddRange(hero.trainedMetamagicOptions);
        list.AddRange(hero.trainedInvocations);
        list.AddRange(hero.trainedFightingStyles);

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
    internal static List<T> GetSubFeaturesByType<T>([CanBeNull] this RulesetActor actor, params Type[] typesToSkip)
        where T : class
    {
        var list = AllActiveDefinitions(actor)
            .Where(f => !typesToSkip.Contains(f.GetType()))
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .ToList();

        if (actor != null)
        {
            list.AddRange(actor.AllConditions.SelectMany(x => x.ConditionDefinition.GetAllSubFeaturesOfType<T>()));
        }

        return list;
    }

    internal static bool HasSubFeatureOfType<T>([CanBeNull] this RulesetActor actor, params Type[] typesToSkip)
        where T : class
    {
        if (AllActiveDefinitions(actor)
                .Where(f => !typesToSkip.Contains(f.GetType()))
                .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
                .FirstOrDefault() != null)
        {
            return true;
        }

        return actor?.AllConditions
            .SelectMany(x => x.ConditionDefinition.GetAllSubFeaturesOfType<T>())
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
        return !actor.HasConditionOfType(ConditionFlying)
               && !actor.HasConditionOfType(ConditionLevitate)
               && !(actor is RulesetCharacter character &&
                    character.MoveModes.ContainsKey((int)MoveMode.Fly));
    }

    internal static bool IsTemporarilyFlying(this RulesetActor actor)
    {
        return actor is RulesetCharacter character
               && character.HasTemporaryConditionOfType(ConditionFlying)
               && !character.HasConditionOfType(ConditionLevitate);
        /*
         * For future use, when can allow flying wildshape to temporarily walk
         * 
        || (actor.HasConditionOfType(RuleDefinitions.ConditionWildShapeSubstituteForm)
                && actor is RulesetCharacterMonster monster
                && monster.MoveModes.ContainsKey((int)RuleDefinitions.MoveMode.Fly)
                && !actor.HasConditionOfType("ConditionFlightSuspended")
        
        );*/
    }

    internal static bool HasAnyConditionOfType(this RulesetActor actor, params string[] conditions)
    {
        return actor is RulesetCharacter && conditions.Any(actor.HasConditionOfType);
    }

    internal static bool HasAnyConditionOfTypeOrSubType(this RulesetActor actor, params string[] conditions)
    {
        return actor is RulesetCharacter && conditions.Any(actor.HasConditionOfTypeOrSubType);
    }
}
