using System;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    /**
     * Note if you want to use a modifier for the power pool later you should set uses determination
     * to fixed or ability bonus plus fixed.
     */
    public class FeatureDefinitionPowerPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerPoolBuilder>
    {
        protected override void Initialise()
        {
            base.Initialise();

            if (IsNew)
            {
                // This is just an activation time that should not be shown in the UI.
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.Permanent);

                // Math for usage gets weird if this isn't 1.
                Definition.SetCostPerUse(1);
            }
        }

        internal override void Validate()
        {
            base.Validate();

            Preconditions.AreEqual(Definition.CostPerUse, 1, $"{GetType().Name}[{Definition.Name}].CostPerUse must be set to 1.");
        }

        public FeatureDefinitionPowerPoolBuilder Configure(int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination, string usesAbilityScoreName,
            RuleDefinitions.RechargeRate recharge)
        {
            Definition.SetFixedUsesPerRecharge(usesPerRecharge);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            Definition.SetRechargeRate(recharge);
            Definition.SetOverriddenPower(Definition);

            return This();
        }

        #region Constructors
        protected FeatureDefinitionPowerPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
