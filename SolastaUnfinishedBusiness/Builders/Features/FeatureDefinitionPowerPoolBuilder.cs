using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

// note if you want to use a modifier for the power pool later you should set uses determination to fixed or ability bonus plus fixed
[UsedImplicitly]
internal class FeatureDefinitionPowerPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower,
    FeatureDefinitionPowerPoolBuilder>
{
    internal override void Validate()
    {
        base.Validate();

        Preconditions.AreEqual(Definition.CostPerUse, 1,
            $"{GetType().Name}[{Definition.Name}].CostPerUse must be set to 1.");
    }

    internal FeatureDefinitionPowerPoolBuilder Configure(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.RechargeRate recharge)
    {
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        Definition.usesDetermination = usesDetermination;
        Definition.usesAbilityScoreName = usesAbilityScoreName;
        Definition.rechargeRate = recharge;
        Definition.overriddenPower = Definition;
        return this;
    }

    internal FeatureDefinitionPowerPoolBuilder SetUsesProficiency()
    {
        Definition.proficiencyBonusToAttack = true;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionPowerPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    #endregion
}
