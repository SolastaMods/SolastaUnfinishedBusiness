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
        UpdateUses(actor, result);
        UpdateSaveDc(actor, result);
        UpdatePoolUses(actor, result);

        return result;
    }

    private static void UpdateUses(RulesetCharacter actor, RulesetUsablePower usablePower)
    {
        if (actor == null) { return; }

        var powerDefinition = usablePower.powerDefinition;
        if (powerDefinition.RechargeRate == RuleDefinitions.RechargeRate.ChannelDivinity)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.ChannelDivinityNumber);
        }
        else if (powerDefinition.RechargeRate == RuleDefinitions.RechargeRate.HealingPool)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.HealingPool);
        }
        else if (powerDefinition.RechargeRate == RuleDefinitions.RechargeRate.SorceryPoints)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.SorceryPoints);
        }
        else if (powerDefinition.UsesDetermination == RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed)
        {
            usablePower.UsesAttribute = actor.GetAttribute(powerDefinition.UsesAbilityScoreName);
        }
        else if (powerDefinition.UsesDetermination == RuleDefinitions.UsesDetermination.ProficiencyBonus)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.ProficiencyBonus);
        }
        else if (powerDefinition.RechargeRate == RuleDefinitions.RechargeRate.KiPoints)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.KiPoints);
        }
        else if (powerDefinition.RechargeRate == RuleDefinitions.RechargeRate.BardicInspiration)
        {
            usablePower.UsesAttribute = actor.GetAttribute(AttributeDefinitions.BardicInspirationNumber);
        }

        usablePower.Recharge();
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
