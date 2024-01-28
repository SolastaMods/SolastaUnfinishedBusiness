using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

public class CountPowerUseInSpecialFeatures
{
    private CountPowerUseInSpecialFeatures()
    {
    }

    public static CountPowerUseInSpecialFeatures Marker { get; } = new();

    internal static void Count(RulesetCharacter character, RulesetUsablePower usablePower)
    {
        var power = usablePower.PowerDefinition;

        if (!power.HasSubFeatureOfType<CountPowerUseInSpecialFeatures>())
        {
            return;
        }

        var user = GameLocationCharacter.GetFromActor(character);

        if (user == null)
        {
            return;
        }

        var features = user.UsedSpecialFeatures;

        features.TryGetValue(power.Name, out var uses);
        features.AddOrReplace(power.Name, uses + 1);
    }
}
