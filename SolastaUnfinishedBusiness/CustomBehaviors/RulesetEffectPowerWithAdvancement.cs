using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class RulesetEffectPowerWithAdvancement : RulesetEffectPower
{
    internal RulesetEffectPowerWithAdvancement(
        int extraCharges,
        RulesetCharacter user,
        RulesetUsablePower usablePower,
        RulesetItemDevice originItem = null,
        RulesetDeviceFunction usableDeviceFunction = null) : base(user, usablePower, originItem, usableDeviceFunction)
    {
        var effectLevel = 1 + extraCharges;

        if (extraCharges > 0)
        {
            var provider = usablePower.PowerDefinition.GetFirstSubFeatureOfType<ICustomOverchargeProvider>();
            if (provider != null)
            {
                var steps = provider.OverchargeSteps(Global.CurrentGuiCharacter);
                effectLevel = 1 + CustomOverchargeProvider.GetAdvancementFromOvercharge(extraCharges, steps);
            }
        }

        EffectLevel = effectLevel;
        ExtraCharges = extraCharges;

        RemainingRounds = PowerDefinition.EffectDescription.ComputeRoundsDuration(effectLevel);
    }

    public override int EffectLevel { get; }
    internal int ExtraCharges { get; }

    public override int ComputeTargetParameter()
    {
        var targetParameter = base.ComputeTargetParameter();

        if (!EffectDescription.HasAdditionalSlotAdvancement)
        {
            return targetParameter;
        }

        var num = EffectLevel - 1;
        var targetType = EffectDescription.TargetType;

        if (targetType == RuleDefinitions.TargetType.Position || EffectDescription.IsAoE)
        {
            targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalTargetCellsBySlotDelta(num);
        }
        else
        {
            switch (targetType)
            {
                case RuleDefinitions.TargetType.Individuals:
                case RuleDefinitions.TargetType.IndividualsUnique:
                    targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalTargetsBySlotDelta(num);
                    break;
                case RuleDefinitions.TargetType.ArcFromIndividual:
                    targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalSubtargetsBySlotDelta(num);
                    break;
            }
        }

        return targetParameter;
    }

    internal static bool GetAdvancementData(CharacterActionUsePower action)
    {
        if (action.ActingCharacter.RulesetCharacter is RulesetCharacterEffectProxy)
        {
            return true;
        }

        if (action.activePower.PowerDefinition.SurrogateToSpell != null)
        {
            return true;
        }

        var effectDescription = action.activePower.EffectDescription;

        if (action.activePower is not RulesetEffectPowerWithAdvancement
            || !effectDescription.HasAdditionalSlotAdvancement)
        {
            return true;
        }

        var deltaLevel = action.activePower.EffectLevel - 1;
        var advancement = effectDescription.EffectAdvancement;

        action.AddDice = advancement.ComputeAdditionalDiceBySlotDelta(deltaLevel);
        action.AddHP = advancement.ComputeAdditionalHPBySlotDelta(deltaLevel);
        action.AddTempHP = advancement.ComputeAdditionalTempHPBySlotDelta(deltaLevel);

        return false;
    }

    internal static bool InstantiateActiveDeviceFunction(
        RulesetImplementationManagerLocation manager,
        ref RulesetEffect result,
        RulesetCharacter user,
        RulesetItemDevice device,
        RulesetDeviceFunction deviceFunction,
        int addedCharges,
        bool delayRegistration)
    {
        var functionDescription = deviceFunction.DeviceFunctionDescription;

        if (functionDescription.Type == DeviceFunctionDescription.FunctionType.Spell || addedCharges == 0)
        {
            return true;
        }

        var usablePower = new RulesetUsablePower(functionDescription.FeatureDefinitionPower, null, null);

        result = new RulesetEffectPowerWithAdvancement(addedCharges, user, usablePower, device, deviceFunction);
        manager.HandleEffectRegistration(result, delayRegistration);

        return false;
    }
}
