using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class CountPowerUseInSpecialFeatures
{
    public static CountPowerUseInSpecialFeatures Marker { get; } = new();

    private CountPowerUseInSpecialFeatures()
    {
    }

    internal static void Count(RulesetCharacter character, RulesetUsablePower usablePower)
    {
        var power = usablePower.PowerDefinition;
        if (!power.HasSubFeatureOfType<CountPowerUseInSpecialFeatures>()) { return; }

        var user = GameLocationCharacter.GetFromActor(character);
        if (user == null) { return; }

        var features = user.UsedSpecialFeatures;
        features.TryGetValue(power.Name, out var uses);
        features.AddOrReplace(power.Name, uses + 1);
    }
}
