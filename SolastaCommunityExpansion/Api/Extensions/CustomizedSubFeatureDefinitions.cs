using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Api.Extensions;

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

    [NotNull]
    internal static T SetCustomSubFeatures<T>([NotNull] this T definition, params object[] subFeatures)
        where T : BaseDefinition
    {
        GetOrCreateForKey(definition).SetRange(subFeatures);

        return definition;
    }

    [CanBeNull]
    internal static List<T> GetAllSubFeaturesOfType<T>([CanBeNull] this BaseDefinition definition) where T : class
    {
        if (definition == null)
        {
            return null;
        }

        var results = new List<T>();

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
