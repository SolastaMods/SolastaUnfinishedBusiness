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
        //BUGFIX: check required for smooth mod update to game version 1.5.x
        if (Main.Enabled)
        {
            return FeaturesByType<FeatureDefinition>(item)
                .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
                .ToList();
        }

        return new List<T>();
    }

    internal static bool HasSubFeatureOfType<T>(this RulesetItem item) where T : class
    {
        //BUGFIX: check required for smooth mod update to game version 1.5.x
        if (Main.Enabled)
        {
            return FeaturesByType<FeatureDefinition>(item)
                .SelectMany(f => f.GetAllSubFeaturesOfType<T>())
                .FirstOrDefault() != null;
        }

        return false;
    }

    internal static bool NeedsIdentification(this RulesetItem item)
    {
        var definition = item.itemDefinition;
        return definition.Magical && definition.RequiresIdentification && !item.Identified;
    }
}
