using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.CustomSpecificBehaviors;

public class RepeatingShot
{
    private RepeatingShot()
    {
    }

    public static RepeatingShot Instance { get; } = new();

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
