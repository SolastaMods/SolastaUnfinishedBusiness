using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Api.Extensions;

internal static class CustomizedSubFeatureDefinitions
{
    private static readonly Dictionary<BaseDefinition, List<object>> CustomSubFeatures = new();

    private static List<object> GetOrCreateForKey(BaseDefinition definition)
    {
        if (!CustomSubFeatures.ContainsKey(definition))
        {
            CustomSubFeatures.Add(definition, new List<object>());
        }

        return CustomSubFeatures[definition];
    }

    private static List<object> GetForKey(BaseDefinition definition)
    {
        if (!CustomSubFeatures.ContainsKey(definition))
        {
            return null;
        }

        return CustomSubFeatures[definition];
    }

    internal static T SetCustomSubFeatures<T>(this T definition, params object[] subFeatures) where T : BaseDefinition
    {
        GetOrCreateForKey(definition).SetRange(subFeatures);
        
        return definition;
    }

    internal static List<T> GetAllSubFeaturesOfType<T>(this BaseDefinition definition) where T : class
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

    internal static T GetFirstSubFeatureOfType<T>(this BaseDefinition definition) where T : class
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
