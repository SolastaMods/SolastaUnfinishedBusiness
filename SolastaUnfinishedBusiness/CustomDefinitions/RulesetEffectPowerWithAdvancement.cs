using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class RulesetEffectPowerWithAdvancement : RulesetEffectPower
{
    private int _effectLevel;

    [UsedImplicitly]
    public RulesetEffectPowerWithAdvancement()
    {
    }

    private RulesetEffectPowerWithAdvancement(
        int extraCharges,
        RulesetCharacter user,
        RulesetUsablePower usablePower,
        RulesetItemDevice originItem = null,
        RulesetDeviceFunction usableDeviceFunction = null) : base(user, usablePower, originItem, usableDeviceFunction)
    {
        ExtraCharges = extraCharges;
        UpdateEffectLevel();
        RemainingRounds = PowerDefinition.EffectDescription.ComputeRoundsDuration(_effectLevel);
    }

    public override int EffectLevel => _effectLevel;

    internal int ExtraCharges { get; private set; }

    private void UpdateEffectLevel()
    {
        _effectLevel = 1 + ExtraCharges;

        if (usablePower == null)
        {
            return;
        }

        if (ExtraCharges <= 0)
        {
            return;
        }

        var provider = usablePower.PowerDefinition.GetFirstSubFeatureOfType<ICustomOverchargeProvider>();

        if (provider == null)
        {
            return;
        }

        var steps = provider.OverchargeSteps(user);

        _effectLevel = 1 + CustomOverchargeProvider.GetAdvancementFromOvercharge(ExtraCharges, steps);
    }

    public override int ComputeTargetParameter()
    {
        var targetParameter = base.ComputeTargetParameter();

        if (!EffectDescription.HasAdditionalSlotAdvancement)
        {
            return targetParameter;
        }

        var num = EffectLevel - 1;
        var targetType = EffectDescription.TargetType;

        if (targetType == TargetType.Position || EffectDescription.IsAoE)
        {
            targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalTargetCellsBySlotDelta(num);
        }
        else
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (targetType)
            {
                case TargetType.Individuals:
                case TargetType.IndividualsUnique:
                    targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalTargetsBySlotDelta(num);
                    break;
                case TargetType.ArcFromIndividual:
                    targetParameter += EffectDescription.EffectAdvancement.ComputeAdditionalSubtargetsBySlotDelta(num);
                    break;
            }
        }

        return targetParameter;
    }

    public override void SerializeAttributes(IAttributesSerializer serializer, IVersionProvider versionProvider)
    {
        base.SerializeAttributes(serializer, versionProvider);
        ExtraCharges = serializer.SerializeAttribute("ExtraCharges", ExtraCharges);
    }

    public override void SerializeElements(IElementsSerializer serializer, IVersionProvider versionProvider)
    {
        base.SerializeElements(serializer, versionProvider);
        UpdateEffectLevel();
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
