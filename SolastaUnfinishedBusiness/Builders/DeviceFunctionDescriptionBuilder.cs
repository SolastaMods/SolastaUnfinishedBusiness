using System;
using SolastaUnfinishedBusiness.Api;
using static DeviceFunctionDescription;

namespace SolastaUnfinishedBusiness.Builders;

internal class DeviceFunctionDescriptionBuilder
{
    private readonly DeviceFunctionDescription description;

    internal DeviceFunctionDescriptionBuilder()
    {
        description = new DeviceFunctionDescription(
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
        description.type = FunctionType.Power;
        description.featureDefinitionPower = power;
        description.canOverchargeSpell = canOvercharge;
        return this;
    }

    internal DeviceFunctionDescriptionBuilder SetSpell(SpellDefinition spell, bool canOverchargeSpell = false)
    {
        description.type = FunctionType.Spell;
        description.spellDefinition = spell;
        description.canOverchargeSpell = canOverchargeSpell;
        return this;
    }

    internal DeviceFunctionDescriptionBuilder SetUsage(
        EquipmentDefinitions.ItemUsage parentUsage = EquipmentDefinitions.ItemUsage.ByFunction,
        FunctionUseAffinity useAffinity = FunctionUseAffinity.AtWill,
        int useAmount = 1)
    {
        description.parentUsage = parentUsage;
        description.useAffinity = useAffinity;
        description.useAmount = useAmount;
        return this;
    }

    private void Validate()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (description.Type)
        {
            case FunctionType.Power when description.FeatureDefinitionPower == null:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty FeatureDefinitionPower!");
            case FunctionType.Spell when description.SpellDefinition == null:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty SpellDefinition!");
        }
    }

    internal DeviceFunctionDescription Build()
    {
        Validate();
        return description;
    }
}
