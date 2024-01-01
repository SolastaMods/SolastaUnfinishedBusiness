using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CustomizedSubFeatureDefinitions
{
    private static readonly Dictionary<BaseDefinition, List<object>> CustomSubFeatures = new();

    private static List<object> GetOrCreateForKey([NotNull] BaseDefinition definition)
    {
        if (CustomSubFeatures.TryGetValue(definition, out var value))
        {
            return value;
        }

        value = [];
        CustomSubFeatures.Add(definition, value);

        return value;
    }

    [CanBeNull]
    // ReSharper disable once ReturnTypeCanBeEnumerable.Local
    private static List<object> GetForKey([NotNull] BaseDefinition definition)
    {
        return !CustomSubFeatures.TryGetValue(definition, out var value) ? null : value;
    }

    internal static void AddCustomSubFeatures<T>(
        [NotNull] this T definition,
        [NotNull] params object[] subFeatures)
        where T : BaseDefinition
    {
        GetOrCreateForKey(definition).AddRange(subFeatures);
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
