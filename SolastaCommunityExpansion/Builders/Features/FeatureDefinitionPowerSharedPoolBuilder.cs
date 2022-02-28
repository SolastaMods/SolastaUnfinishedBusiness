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
            Definition.SetGuiPresentation(guiPresentation);
        }
    }

    /**
     * Note this is based on FeatureDefinitionPower so that you can take advantage of power usage calculations
     * like proficiency or ability score usage. However in order to do that the game needs to add a power to
     * the hero and only one power for a given name+guid is added. Which means if you want to add a +1 modifier
     * at 4 different character levels you need to create 4 different FeatureDefinitionPowerPoolModifier.
     */
    public class FeatureDefinitionPowerPoolModifierBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerPoolModifier, FeatureDefinitionPowerPoolModifierBuilder>
    {
        public FeatureDefinitionPowerPoolModifierBuilder(string name, string guid,
            int powerPoolModifier, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName, FeatureDefinitionPower poolPower, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetFixedUsesPerRecharge(powerPoolModifier);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            // This is just an activation time that should not be shown in the UI.
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Permanent);
            // Math for usage gets weird if this isn't 1.
            Definition.SetCostPerUse(1);
            // The game throws an exception if there is no effect description.
            Definition.SetEffectDescription(new EffectDescription());
            Definition.SetOverriddenPower(Definition);
            Definition.SetGuiPresentation(guiPresentation);

            Definition.PoolPower = poolPower;
        }
    }

    public class FeatureDefinitionPowerSharedPoolBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerSharedPool, FeatureDefinitionPowerSharedPoolBuilder>
    {
        public FeatureDefinitionPowerSharedPoolBuilder(string name, string guid, FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge,
            RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
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
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetUniqueInstance(uniqueInstance);
            Definition.SharedPool = poolPower;
        }

        public FeatureDefinitionPowerSharedPoolBuilder(string name, string guid,
            FeatureDefinitionPower poolPower, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SharedPool = poolPower;
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
