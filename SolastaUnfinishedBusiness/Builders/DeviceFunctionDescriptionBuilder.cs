using System;
using SolastaUnfinishedBusiness.Api;
using static DeviceFunctionDescription;

namespace SolastaUnfinishedBusiness.Builders;

internal class DeviceFunctionDescriptionBuilder
{
    private readonly DeviceFunctionDescription _description;

    internal DeviceFunctionDescriptionBuilder()
    {
        _description = new DeviceFunctionDescription(
            DatabaseHelper.ItemDefinitions.BeltOfRegeneration.UsableDeviceDescription.DeviceFunctions[0])
        {
            parentUsage = EquipmentDefinitions.ItemUsage.ByFunction,
            useAffinity = FunctionUseAffinity.AtWill,
            useAmount = 1,
            rechargeRate = RuleDefinitions.RechargeRate.Dawn,
            durationType = RuleDefinitions.DurationType.Instantaneous,
            canOverchargeSpell = false,
            type = FunctionType.Power,
            spellDefinition = null,
            featureDefinitionPower = null
        };
    }

    internal DeviceFunctionDescriptionBuilder SetPower(FeatureDefinitionPower power, bool canOvercharge = false)
    {
        _description.type = FunctionType.Power;
        _description.featureDefinitionPower = power;
        _description.canOverchargeSpell = canOvercharge;
        return this;
    }

    internal DeviceFunctionDescriptionBuilder SetSpell(SpellDefinition spell, bool canOverchargeSpell = false)
    {
        _description.type = FunctionType.Spell;
        _description.spellDefinition = spell;
        _description.canOverchargeSpell = canOverchargeSpell;
        return this;
    }

    internal DeviceFunctionDescriptionBuilder SetUsage(
        EquipmentDefinitions.ItemUsage parentUsage = EquipmentDefinitions.ItemUsage.ByFunction,
        FunctionUseAffinity useAffinity = FunctionUseAffinity.AtWill,
        int useAmount = 1)
    {
        _description.parentUsage = parentUsage;
        _description.useAffinity = useAffinity;
        _description.useAmount = useAmount;
        return this;
    }

    private void Validate()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_description.Type)
        {
            case FunctionType.Power when !_description.FeatureDefinitionPower:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty FeatureDefinitionPower!");
            case FunctionType.Spell when !_description.SpellDefinition:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty SpellDefinition!");
        }
    }

    internal DeviceFunctionDescription Build()
    {
        Validate();
        return _description;
    }
}
