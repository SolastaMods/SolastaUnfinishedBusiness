
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

        public FeatureDefinitionPowerSharedPoolBuilder SetEffect(EffectDescription effect)
        {
            Definition.SetEffectDescription(effect);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder DelegatedToAction()
        {
            Definition.SetDelegatedToAction(true);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SurrogateToSpell(SpellDefinition spell)
        {
            Definition.SetSurrogateToSpell(spell);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetActivation(RuleDefinitions.ActivationTime time, int costPerUse)
        {
            Definition.SetActivationTime(time);
            Definition.SetCostPerUse(costPerUse);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetReaction(RuleDefinitions.ReactionTriggerContext context, string name)
        {
            Definition.SetReactionContext(context);
            Definition.SetReactionName(name);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetCastingFailure(RuleDefinitions.CastingSuccessComputation computation)
        {
            Definition.SetHasCastingFailure(true);
            Definition.SetCastingSuccessComputation(computation);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder CanUseInDialog()
        {
            Definition.SetCanUseInDialog(true);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetDisablingCondition(ConditionDefinition condition)
        {
            Definition.SetDisableIfConditionIsOwned(condition);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetRecharge(RuleDefinitions.RechargeRate rate)
        {
            Definition.SetRechargeRate(rate);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetSpellCastingFeature(FeatureDefinitionCastSpell spellFeature)
        {
            Definition.SetSpellcastingFeature(spellFeature);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetUsesFixed(int fixedUses)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetUsesProficiency()
        {
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetUsesAbility(int fixedUses, string attribute)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesAbilityScoreName(attribute);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder UseSpellCastingModifier()
        {
            Definition.SetAbilityScoreDetermination(RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetAttackModifierAbility(bool ability, bool proficiency, string attribute)
        {
            Definition.SetAbilityScore(attribute);
            Definition.SetAbilityScoreBonusToAttack(ability);
            Definition.SetProficiencyBonusToAttack(proficiency);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.AbilityScore);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetAttackModifierStatic(int attackModifier)
        {
            Definition.SetFixedAttackHit(attackModifier);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.Fixed);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetUniqueInstance(bool uniqueInstance)
        {
            Definition.SetUniqueInstance(uniqueInstance);
            return this;
        }


        public FeatureDefinitionPowerSharedPoolBuilder SetShowCasting(bool casting)
        {
            Definition.SetShowCasting(casting);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder AddOverriddenPower(FeatureDefinitionPower overridenPower)
        {
            Definition.SetOverriddenPower(overridenPower);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetShortTitle(string title)
        {
            Definition.SetShortTitleOverride(title);
            return this;
        }

        public FeatureDefinitionPowerSharedPoolBuilder SetAbility(string ability)
        {
            Definition.SetAbilityScore(ability);
            return this;
        }
    }
}
