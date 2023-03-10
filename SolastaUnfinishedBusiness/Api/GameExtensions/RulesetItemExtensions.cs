using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class RulesetItemExtensions
{
    [NotNull]
    private static List<T> FeaturesByType<T>([CanBeNull] RulesetItem item) where T : class
    {
        var list = new List<FeatureDefinition>();

        item?.EnumerateFeaturesToBrowse<T>(list);

        return list
            .OfType<T>()
            .ToList();
    }

    [NotNull]
    internal static List<T> GetSubFeaturesByType<T>(this RulesetItem item) where T : class
    {
        return FeaturesByType<FeatureDefinition>(item)
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .ToList();
    }

    internal static bool HasSubFeatureOfType<T>(this RulesetItem item) where T : class
    {
        return FeaturesByType<FeatureDefinition>(item)
            .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
            .FirstOrDefault() != null;
    }

    internal static bool NeedsIdentification(this RulesetItem item)
    {
        var definition = item.itemDefinition;
        return definition.Magical && definition.RequiresIdentification && !item.Identified;
    }
}
