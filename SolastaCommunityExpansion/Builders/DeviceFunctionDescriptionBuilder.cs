using System;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Builders;

public class DeviceFunctionDescriptionBuilder
{
    private readonly DeviceFunctionDescription description;

    public DeviceFunctionDescriptionBuilder()
    {
        description = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.BeltOfRegeneration
            .UsableDeviceDescription.DeviceFunctions[0])
        {
            parentUsage = EquipmentDefinitions.ItemUsage.ByFunction,
            useAffinity = DeviceFunctionDescription.FunctionUseAffinity.AtWill,
            useAmount = 1,
            rechargeRate = RuleDefinitions.RechargeRate.Dawn,
            durationType = RuleDefinitions.DurationType.Instantaneous,
            canOverchargeSpell = false,
            type = DeviceFunctionDescription.FunctionType.Power,
            spellDefinition = null,
            featureDefinitionPower = null
        };
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
        switch (description.Type)
        {
            case DeviceFunctionDescription.FunctionType.Power when description.FeatureDefinitionPower == null:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty FeatureDefinitionPower!");
            case DeviceFunctionDescription.FunctionType.Spell when description.SpellDefinition == null:
                throw new ArgumentException("DeviceFunctionDescriptionBuilder empty SpellDefinition!");
        }
    }

    public DeviceFunctionDescription Build()
    {
        Validate();
        return description;
    }
}
