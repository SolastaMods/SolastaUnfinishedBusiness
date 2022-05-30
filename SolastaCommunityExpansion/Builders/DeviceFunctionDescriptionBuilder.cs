using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders;

public class DeviceFunctionDescriptionBuilder
{
    private readonly DeviceFunctionDescription description;

    public DeviceFunctionDescriptionBuilder()
    {
        description = new DeviceFunctionDescription(DatabaseHelper.ItemDefinitions.BeltOfRegeneration
            .UsableDeviceDescription.DeviceFunctions[0]);

        description.SetParentUsage(EquipmentDefinitions.ItemUsage.ByFunction);
        description.SetUseAffinity(DeviceFunctionDescription.FunctionUseAffinity.AtWill);
        description.SetUseAmount(1);
        description.SetRechargeRate(RuleDefinitions.RechargeRate.Dawn);
        description.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
        description.SetCanOverchargeSpell(false);
        description.SetType(DeviceFunctionDescription.FunctionType.Power);
        description.SetSpellDefinition(null);
        description.SetFeatureDefinitionPower(null);
    }

    public DeviceFunctionDescriptionBuilder SetPower(FeatureDefinitionPower power)
    {
        description.SetType(DeviceFunctionDescription.FunctionType.Power);
        description.SetFeatureDefinitionPower(power);
        return this;
    }

    public DeviceFunctionDescriptionBuilder SetSpell(SpellDefinition spell, bool canOverchargeSpell = false)
    {
        description.SetType(DeviceFunctionDescription.FunctionType.Spell);
        description.SetSpellDefinition(spell);
        description.SetCanOverchargeSpell(canOverchargeSpell);
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
