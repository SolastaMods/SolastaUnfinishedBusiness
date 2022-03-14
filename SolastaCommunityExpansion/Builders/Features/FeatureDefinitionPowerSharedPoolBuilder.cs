using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    /**
     * Note if you want to use a modifier for the power pool later you should set uses determination
     * to fixed or ability bonus plus fixed.
     */
    public class FeatureDefinitionPowerPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerPoolBuilder>
    {
        public FeatureDefinitionPowerPoolBuilder(string name, string guid, int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination, string usesAbilityScoreName,
            RuleDefinitions.RechargeRate recharge, GuiPresentation guiPresentation) : base(name, guid)
        {
            Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, recharge);

            Definition.SetGuiPresentation(guiPresentation);
        }

        // TODO: move this to FeatureDefinitionPowerBuilder as ConfigurePowerPool()?
        public FeatureDefinitionPowerPoolBuilder Configure(int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination, string usesAbilityScoreName,
            RuleDefinitions.RechargeRate recharge)
        {
            Definition.SetFixedUsesPerRecharge(usesPerRecharge);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            // This is just an activation time that won't allow activation in the UI.
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Permanent);
            // Math for usage gets weird if this isn't 1.
            Definition.SetCostPerUse(1);
            Definition.SetRechargeRate(recharge);
            // The game throws an exception if there is no effect description.
            Definition.SetEffectDescription(new EffectDescription());
            Definition.SetOverriddenPower(Definition);

            return This();
        }

        #region Constructors
        public FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original) : base(original)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerPoolBuilder(FeatureDefinitionPower original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }

    public class FeatureDefinitionPowerSharedPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerSharedPool, FeatureDefinitionPowerSharedPoolBuilder>
    {
        public FeatureDefinitionPowerSharedPoolBuilder(string name, string guid, FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge,
            RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
        {
            Configure(poolPower, recharge, activationTime, costPerUse, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription, uniqueInstance);

            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionPowerSharedPoolBuilder Configure(FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore, EffectDescription effectDescription, bool uniqueInstance)
        {
            // We set uses determination to fixed because the code handling updates needs that.
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            // Recharge rate probably shouldn't be in here, but for now leave it be because there is already usage outside of this mod.
            Definition.SetRechargeRate(recharge);
            Definition.SetActivationTime(activationTime);
            Definition.SetCostPerUse(costPerUse);
            Definition.SetProficiencyBonusToAttack(proficiencyBonusToAttack);
            Definition.SetAbilityScoreBonusToAttack(abilityScoreBonusToAttack);
            Definition.SetAbilityScore(abilityScore);
            Definition.SetEffectDescription(effectDescription);
            Definition.SetUniqueInstance(uniqueInstance);
            Definition.SharedPool = poolPower;

            return This();
        }

        #region Constructors
        public FeatureDefinitionPowerSharedPoolBuilder(FeatureDefinitionPowerSharedPool original) : base(original)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(FeatureDefinitionPowerSharedPool original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(FeatureDefinitionPowerSharedPool original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerSharedPoolBuilder(FeatureDefinitionPowerSharedPool original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
