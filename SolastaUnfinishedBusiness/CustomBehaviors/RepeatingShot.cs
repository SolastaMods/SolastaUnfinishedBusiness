using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RepeatingShot
{
    public static RepeatingShot Instance { get; } = new();

    private RepeatingShot()
    {
    }

    internal static void ModifyTags(RulesetItem item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (!HasRepeatingShot(item))
        {
            return;
        }

        tags.Remove(TagsDefinitions.WeaponTagLoading);
        tags.Remove(TagsDefinitions.WeaponTagAmmunition);
    }

    internal static bool HasRepeatingShot(RulesetItem item)
    {
        return item != null && item.HasSubFeatureOfType<RepeatingShot>();
    }
}
