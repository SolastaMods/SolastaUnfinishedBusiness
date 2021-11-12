
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IPowerSharedPool
    {
        FeatureDefinitionPower GetUsagePoolPower();
    }

    /**
     * Features using a shared pool should have UsesDetermination == Fixed.
     */
    public class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower, IPowerSharedPool
    {
        public FeatureDefinitionPower SharedPool;

        public FeatureDefinitionPower GetUsagePoolPower()
        {
            return SharedPool;
        }
    }

    public interface IPowerPoolModifier
    {
        FeatureDefinitionPower GetUsagePoolPower();
        int PoolChangeAmount();
    }

    public class FeatureDefinitionPowerPoolModifier : FeatureDefinitionPower, IPowerPoolModifier
    {
        public FeatureDefinitionPower PoolPower;
        public int poolChangeAmount;

        public FeatureDefinitionPower GetUsagePoolPower()
        {
            return PoolPower;
        }

        public int PoolChangeAmount()
        {
            return poolChangeAmount;
        }
    }

    /**
     * Note if you want to use a modifier for the power pool later you should set uses determination
     * to fixed or ability bonus plus fixed.
     */
    public class FeatureDefinitionPowerPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        public FeatureDefinitionPowerPoolBuilder(string name, string guid, int usesPerRecharge,
            RuleDefinitions.UsesDetermination usesDetermination, string usesAbilityScoreName,
            RuleDefinitions.RechargeRate recharge, GuiPresentation guiPresentation) : base(name,guid)
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

            if (Main.IsModHelpersLoaded())
            {
                // use gui hidden if mod helpers is installed- this also hides the power from level up screens which is not ideal
                guiPresentation.SetHidden(true);
            } else
            {
                // Note Holic's ModHelpers removes all overriden powers.
                // Setting to hidden means it never shows in the UI including on level up screens.
                // By not hidding and not overriding the feature users will see it in the list of available powers.
                // That is not ideal, but seems like a decent compromise for now.
                //Definition.SetOverriddenPower(Definition);
            }
            Definition.SetGuiPresentation(guiPresentation);
        }
    }

    public class FeatureDefinitionPowerPoolModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionPowerPoolModifier>
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

    public class FeatureDefinitionPowerSharedPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPowerSharedPool>
    {
        public FeatureDefinitionPowerSharedPoolBuilder(string name, string guid, FeatureDefinitionPower poolPower,
            RuleDefinitions.RechargeRate recharge,
            RuleDefinitions.ActivationTime activationTime, int costPerUse,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
        {
            // We set uses determination to fixed because the code handling updates needs that.
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
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

        public FeatureDefinitionPowerSharedPoolBuilder AddOverriddenPower(FeatureDefinitionPower overridenPower)
        {
            Definition.SetOverriddenPower(overridenPower);
            return this;
        }
    }
}
