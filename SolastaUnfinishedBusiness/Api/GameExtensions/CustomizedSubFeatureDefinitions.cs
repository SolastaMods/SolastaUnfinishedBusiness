using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CustomizedSubFeatureDefinitions
{
    private static readonly Dictionary<BaseDefinition, List<object>> CustomSubFeatures = new();

    private static List<object> GetOrCreateForKey([NotNull] BaseDefinition definition)
    {
        if (!CustomSubFeatures.ContainsKey(definition))
        {
            CustomSubFeatures.Add(definition, new List<object>());
        }

        return CustomSubFeatures[definition];
    }

    [CanBeNull]
    private static IEnumerable<object> GetForKey([NotNull] BaseDefinition definition)
    {
        return !CustomSubFeatures.ContainsKey(definition) ? null : CustomSubFeatures[definition];
    }

    internal static void AddCustomSubFeatures<T>(
        [NotNull] this T definition,
        [NotNull] params object[] subFeatures)
        where T : BaseDefinition
    {
        GetOrCreateForKey(definition).AddRange(subFeatures);
    }

    internal static void RemoveCustomSubFeatures<T>(
        [NotNull] this T definition,
        [NotNull] params object[] subFeatures)
        where T : BaseDefinition
    {
        if (!CustomSubFeatures.ContainsKey(definition))
        {
            return;
        }

        foreach (var subFeature in subFeatures)
        {
            CustomSubFeatures[definition].Remove(subFeature);
        }
    }

    [NotNull]
    internal static List<T> GetAllSubFeaturesOfType<T>([CanBeNull] this BaseDefinition definition) where T : class
    {
        var results = new List<T>();

        if (definition == null)
        {
            return results;
        }

        if (definition is T custom)
        {
            results.Add(custom);
        }

        var subFeatures = GetForKey(definition)?.OfType<T>();

        if (subFeatures != null)
        {
            results.AddRange(subFeatures);
        }

        return results;
    }

    [CanBeNull]
    internal static T GetFirstSubFeatureOfType<T>([CanBeNull] this BaseDefinition definition) where T : class
    {
        if (definition == null)
        {
            return null;
        }

        if (definition is T custom)
        {
            return custom;
        }

        return GetForKey(definition)?.OfType<T>().FirstOrDefault();
    }

    internal static bool HasSubFeatureOfType<T>(this BaseDefinition definition) where T : class
    {
        return definition.GetFirstSubFeatureOfType<T>() != null;
    }
}
