using System;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Builders;

public class DeviceFunctionDescriptionBuilder
{
    private readonly DeviceFunctionDescription description;

    public DeviceFunctionDescriptionBuilder()
    {
        description = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.BeltOfRegeneration
            .UsableDeviceDescription.DeviceFunctions[0]);

        description.parentUsage = EquipmentDefinitions.ItemUsage.ByFunction;
        description.useAffinity = DeviceFunctionDescription.FunctionUseAffinity.AtWill;
        description.useAmount = 1;
        description.rechargeRate = RuleDefinitions.RechargeRate.Dawn;
        description.durationType = RuleDefinitions.DurationType.Instantaneous;
        description.canOverchargeSpell = false;
        description.type = DeviceFunctionDescription.FunctionType.Power;
        description.spellDefinition = null;
        description.featureDefinitionPower = null;
    }

    public DeviceFunctionDescriptionBuilder SetPower(FeatureDefinitionPower power)
    {
        description.type = DeviceFunctionDescription.FunctionType.Power;
        description.featureDefinitionPower = power;
        return this;
    }

    public DeviceFunctionDescriptionBuilder SetSpell(SpellDefinition spell, bool canOverchargeSpell = false)
    {
        description.type = DeviceFunctionDescription.FunctionType.Spell;
        description.spellDefinition = spell;
        description.canOverchargeSpell = canOverchargeSpell;
        return this;
    }

    private void Validate()
    {
        if (description.Type == DeviceFunctionDescription.FunctionType.Power
            && description.FeatureDefinitionPower == null)
        {
            throw new ArgumentException("DeviceFunctionDescriptionBuilder empty FeatureDefinitionPower!");
        }

        if (description.Type == DeviceFunctionDescription.FunctionType.Spell
            && description.SpellDefinition == null)
        {
            throw new ArgumentException("DeviceFunctionDescriptionBuilder empty SpellDefinition!");
        }
    }

    public DeviceFunctionDescription Build()
    {
        Validate();
        return description;
    }
}
