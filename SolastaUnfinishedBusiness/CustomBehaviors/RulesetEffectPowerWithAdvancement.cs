namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RulesetEffectPowerWithAdvancement : RulesetEffectPower
{
    public RulesetEffectPowerWithAdvancement(
        int advancement,
        RulesetCharacter user,
        RulesetUsablePower usablePower,
        RulesetItemDevice originItem = null,
        RulesetDeviceFunction usableDeviceFunction = null) : base(user, usablePower, originItem, usableDeviceFunction)
    {
        EffectLevel = 1 + advancement;
        RemainingRounds = PowerDefinition.EffectDescription.ComputeRoundsDuration(1 + advancement);
    }

    public override int EffectLevel { get; }

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

    public static bool GetAdvancementData(CharacterActionUsePower action)
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

    public static bool InstantiateActiveDeviceFunction(RulesetImplementationManagerLocation manager,
        ref RulesetEffect result, RulesetCharacter user, RulesetItemDevice device,
        RulesetDeviceFunction deviceFunction, int addedCharges, bool delayRegistration)
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
