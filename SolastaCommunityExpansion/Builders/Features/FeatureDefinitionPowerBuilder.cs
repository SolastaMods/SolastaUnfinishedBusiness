using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        public FeatureDefinitionPowerBuilder(string name, string guid, int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName,
            RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
            bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
            EffectDescription effectDescription, GuiPresentation guiPresentation, bool uniqueInstance) : base(name, guid)
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
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetUniqueInstance(uniqueInstance);
        }

        public FeatureDefinitionPowerBuilder(string name, string guid,
            GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionPowerBuilder SetEffect(EffectDescription effect)
        {
            Definition.SetEffectDescription(effect);
            return this;
        }

        public FeatureDefinitionPowerBuilder DelegatedToAction()
        {
            Definition.SetDelegatedToAction(true);
            return this;
        }

        public FeatureDefinitionPowerBuilder SurrogateToSpell(SpellDefinition spell)
        {
            Definition.SetSurrogateToSpell(spell);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetActivation(RuleDefinitions.ActivationTime time, int costPerUse)
        {
            Definition.SetActivationTime(time);
            Definition.SetCostPerUse(costPerUse);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetReaction(RuleDefinitions.ReactionTriggerContext context, string name)
        {
            Definition.SetReactionContext(context);
            Definition.SetReactionName(name);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetCastingFailure(RuleDefinitions.CastingSuccessComputation computation)
        {
            Definition.SetHasCastingFailure(true);
            Definition.SetCastingSuccessComputation(computation);
            return this;
        }

        public FeatureDefinitionPowerBuilder CanUseInDialog()
        {
            Definition.SetCanUseInDialog(true);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetDisablingCondition(ConditionDefinition condition)
        {
            Definition.SetDisableIfConditionIsOwned(condition);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetRecharge(RuleDefinitions.RechargeRate rate)
        {
            Definition.SetRechargeRate(rate);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetSpellCastingFeature(FeatureDefinitionCastSpell spellFeature)
        {
            Definition.SetSpellcastingFeature(spellFeature);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetUsesFixed(int fixedUses)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetUsesProficiency()
        {
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.ProficiencyBonus);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetUsesAbility(int fixedUses, string attribute)
        {
            Definition.SetFixedUsesPerRecharge(fixedUses);
            Definition.SetUsesAbilityScoreName(attribute);
            Definition.SetUsesDetermination(RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed);
            return this;
        }

        public FeatureDefinitionPowerBuilder UseSpellCastingModifier()
        {
            Definition.SetAbilityScoreDetermination(RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetAttackModifierAbility(bool ability, bool proficiency, string attribute)
        {
            Definition.SetAbilityScore(attribute);
            Definition.SetAbilityScoreBonusToAttack(ability);
            Definition.SetProficiencyBonusToAttack(proficiency);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.AbilityScore);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetAttackModifierStatic(int attackModifier)
        {
            Definition.SetFixedAttackHit(attackModifier);
            Definition.SetAttackHitComputation(RuleDefinitions.PowerAttackHitComputation.Fixed);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetUniqueInstance(bool uniqueInstance)
        {
            Definition.SetUniqueInstance(uniqueInstance);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetShowCasting(bool casting)
        {
            Definition.SetShowCasting(casting);
            return this;
        }

        public FeatureDefinitionPowerBuilder AddOverriddenPower(FeatureDefinitionPower overridenPower)
        {
            Definition.SetOverriddenPower(overridenPower);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetShortTitle(string title)
        {
            Definition.SetShortTitleOverride(title);
            return this;
        }

        public FeatureDefinitionPowerBuilder SetAbility(string ability)
        {
            Definition.SetAbilityScore(ability);
            return this;
        }
    }
}
