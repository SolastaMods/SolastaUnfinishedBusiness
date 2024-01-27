using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;

namespace SolastaUnfinishedBusiness.Behaviors;

internal static class PowerProvider
{
    // private static readonly Dictionary<(FeatureDefinitionPower, RulesetCharacter), RulesetUsablePower>
    //     UsablePowers = [];

    [NotNull]
    internal static RulesetUsablePower Get(FeatureDefinitionPower power, [CanBeNull] RulesetCharacter actor = null)
    {
        var result = (RulesetUsablePower)null;

        if (actor != null)
        {
            result = actor.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);
        }

        if (result != null)
        {
            return result;
        }

        // if (UsablePowers.TryGetValue((power, actor), out var usablePower))
        // {
        //     result = usablePower;
        // }
        // else
        {
            result = new RulesetUsablePower(power, null, null);
            //UsablePowers.Add((power, actor), result);
        }

        //Update properties to match actor
        UpdateSaveDc(actor, result);
        UpdatePoolUses(actor, result);

        return result;
    }

    private static void UpdatePoolUses([CanBeNull] RulesetCharacter character, RulesetUsablePower usablePower)
    {
        if (character == null)
        {
            return;
        }

        var pool = PowerBundle.GetPoolPower(usablePower, character);

        if (pool == null || pool == usablePower)
        {
            return;
        }

        var powerCost = usablePower.PowerDefinition.CostPerUse;
        var maxUsesForPool = character.GetMaxUsesOfPower(usablePower);

        usablePower.maxUses = maxUsesForPool / powerCost;
        usablePower.remainingUses = pool.RemainingUses / powerCost;
    }

    internal static void UpdateSaveDc(
        [CanBeNull] RulesetCharacter actor,
        [NotNull] RulesetUsablePower usablePower,
        CharacterClassDefinition classDefinition = null)
    {
        var power = usablePower.PowerDefinition;
        var effectDescription = power.EffectDescription;

        if (actor == null || !effectDescription.HasSavingThrow)
        {
            return;
        }

        if (classDefinition == null)
        {
            classDefinition = actor.FindClassHoldingFeature(power);
        }

        usablePower.saveDC =
            EffectHelpers.CalculateSaveDc(actor, effectDescription, classDefinition, usablePower.saveDC);
    }
}
