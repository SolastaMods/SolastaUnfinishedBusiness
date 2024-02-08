using System.Linq;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

/**
 * Adds all powers from condition definition's feature list to character's usable powers when condition is applied
 * and removes them when condition is removed.
 */
public class AddUsablePowersFromCondition : IOnConditionAddedOrRemoved
{
    internal static AddUsablePowersFromCondition Marker { get; } = new();

    public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        //assuming mod won't have any case where same power is added twice to a condition
        var powers = target.UsablePowers
            .Select(x => x.PowerDefinition)
            .ToList(); // avoid change enumerator

        foreach (var power in rulesetCondition.ConditionDefinition.Features
                     .OfType<FeatureDefinitionPower>()
                     .Where(x => !powers.Contains(x)))
        {
            target.UsablePowers.Add(PowerProvider.Get(power, target));
        }
    }

    public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        var powers = rulesetCondition.ConditionDefinition.Features
            .OfType<FeatureDefinitionPower>();

        foreach (var usablePower in target.UsablePowers
                     .Where(x => powers.Contains(x.PowerDefinition))
                     .ToList())
        {
            var effectPower = target.PowersUsedByMe
                .FirstOrDefault(x => x.UsablePower == usablePower);

            if (effectPower != null)
            {
                target.TerminatePower(effectPower);
            }
            
            target.UsablePowers.Remove(usablePower);
        }
    }
}
