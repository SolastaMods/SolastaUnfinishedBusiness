using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPowerBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerBuilder>
    {
        #region Constructors
        protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original) : base(original)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }

    public abstract class FeatureDefinitionPowerBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionPower
        where TBuilder : FeatureDefinitionPowerBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionPowerBuilder(TDefinition original) : base(original)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPowerBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPowerBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPowerBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // Over specific method?
        // TODO: split into smaller methods
        public TBuilder Configure(
            int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination, string usesAbilityScoreName,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, bool uniqueInstance)
        {
            Definition.SetFixedUsesPerRecharge(usesPerRecharge);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            Definition.SetActivationTime(activationTime);
            Definition.SetCostPerUse(costPerUse);
            Definition.SetRechargeRate(recharge);
            Definition.SetProficiencyBonusToAttack(proficiencyBonusToAttack);
            Definition.SetAbilityScoreBonusToAttack(abilityScoreBonusToAttack);
            Definition.SetAbilityScore(abilityScore);
            Definition.SetEffectDescription(effectDescription);
            Definition.SetUniqueInstance(uniqueInstance);
            return This();
        }

        public TBuilder SetEffectDescription(EffectDescription effect)
        {
            Definition.SetEffectDescription(effect);
            return This();
        }

        public TBuilder DelegatedToAction()
        {
            Definition.SetDelegatedToAction(true);
            return This();
        }

        public TBuilder SurrogateToSpell(SpellDefinition spell)
        {
            Definition.SetSurrogateToSpell(spell);
            return This();
        }

        public TBuilder SetActivation(RuleDefinitions.ActivationTime time, int costPerUse)
        {
            Definition.SetActivationTime(time);
            Definition.SetCostPerUse(costPerUse);
            return This();
        }

        // TODO: combine with above with default?
        public TBuilder SetActivationTime(RuleDefinitions.ActivationTime value)
        {
            Definition.SetActivationTime(value);
            return This();
        }

        public TBuilder SetReaction(RuleDefinitions.ReactionTriggerContext context, string name)
        {
            Definition.SetReactionContext(context);
            Definition.SetReactionName(name);
            return This();
        }

        public TBuilder SetCastingFailure(RuleDefinitions.CastingSuccessComputation computation)
        {
            Definition.SetHasCastingFailure(true);
            Definition.SetCastingSuccessComputation(computation);
            return This();
        }

        public TBuilder CanUseInDialog()
        {
            Definition.SetCanUseInDialog(true);
            return This();
        }

        public TBuilder SetDisablingCondition(ConditionDefinition condition)
        {
            Definition.SetDisableIfConditionIsOwned(condition);
            return This();
        }

        public TBuilder SetRechargeRate(RuleDefinitions.RechargeRate rate)
        {
            Definition.SetRechargeRate(rate);
            return This();
        }
        
        public TBuilder SetSpellCastingFeature(FeatureDefinitionCastSpell spellFeature)
        {
            Definition.SetSpellcastingFeature(spellFeature);
            return This();
        }

        public TBuilder SetUsesFixed(int fixedUses)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            return This();
        }

        public TBuilder SetUsesProficiency()
        {
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            return This();
        }

        public TBuilder SetUsesAbility(int fixedUses, string attribute)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesAbilityScoreName(attribute);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed);
            return This();
        }

        public TBuilder UseSpellCastingModifier()
        {
            Definition.SetAbilityScoreDetermination(RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility);
            return This();
        }

        public TBuilder SetAttackModifierAbility(bool ability, bool proficiency, string attribute)
        {
            Definition.SetAbilityScore(attribute);
            Definition.SetAbilityScoreBonusToAttack(ability);
            Definition.SetProficiencyBonusToAttack(proficiency);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.AbilityScore);
            return This();
        }

        public TBuilder SetAttackModifierStatic(int attackModifier)
        {
            Definition.SetFixedAttackHit(attackModifier);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.Fixed);
            return This();
        }

        public TBuilder SetUniqueInstance(bool uniqueInstance)
        {
            Definition.SetUniqueInstance(uniqueInstance);
            return This();
        }

        public TBuilder SetShowCasting(bool casting)
        {
            Definition.SetShowCasting(casting);
            return This();
        }

        public TBuilder SetOverriddenPower(FeatureDefinitionPower overridenPower)
        {
            Definition.SetOverriddenPower(overridenPower);
            return This();
        }

        public TBuilder SetShortTitle(string title)
        {
            Definition.SetShortTitleOverride(title);
            return This();
        }

        public TBuilder SetAbility(string ability)
        {
            Definition.SetAbilityScore(ability);
            return This();
        }
    }
}
