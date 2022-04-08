using System;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPowerSharedPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerSharedPool, FeatureDefinitionPowerSharedPoolBuilder>
    {
        protected override void Initialise()
        {
            base.Initialise();

            // We set uses determination to fixed because the code handling updates needs that.
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
        }

        internal override void Validate()
        {
            base.Validate();

            Preconditions.IsNotNull(Definition.SharedPool, $"FeatureDefinitionPowerSharedPoolBuilder[{Definition.Name}].SharedPool is null.");
            Preconditions.AreEqual(Definition.UsesDetermination, RuleDefinitions.UsesDetermination.Fixed, $"FeatureDefinitionPowerSharedPoolBuilder[{Definition.Name}].UsesDetermination must be set to Fixed.");
        }

        public FeatureDefinitionPowerSharedPoolBuilder Configure(FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore, EffectDescription effectDescription, bool uniqueInstance)
        {
            Preconditions.IsNotNull(poolPower, $"FeatureDefinitionPowerSharedPoolBuilder[{Definition.Name}] poolPower is null.");

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

        public FeatureDefinitionPowerSharedPoolBuilder(string name, string guid,
            FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore, EffectDescription effectDescription,
            GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
        {
            Preconditions.IsNotNull(poolPower, $"FeatureDefinitionPowerSharedPoolBuilder[{name}] poolPower is null.");

            Configure(poolPower, recharge, activationTime, costPerUse, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription, uniqueInstance);

            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionPowerSharedPoolBuilder(string name,
            FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge, RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore, EffectDescription effectDescription,
            GuiPresentation guiPresentation, bool uniqueInstance) : base(name, CENamespaceGuid)
        {
            Preconditions.IsNotNull(poolPower, $"FeatureDefinitionPowerSharedPoolBuilder[{name}] poolPower is null.");

            Configure(poolPower, recharge, activationTime, costPerUse, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription, uniqueInstance);

            Definition.SetGuiPresentation(guiPresentation);
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
