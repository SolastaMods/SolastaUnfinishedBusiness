using System;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features;

public class
    FeatureDefinitionPowerBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPower,
        FeatureDefinitionPowerBuilder>
{
    #region Constructors

    protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

public abstract class
    FeatureDefinitionPowerBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionPower
    where TBuilder : FeatureDefinitionPowerBuilder<TDefinition, TBuilder>
{
    protected override void Initialise()
    {
        base.Initialise();

        if (Definition.effectDescription == null)
        {
            // The game throws an exception if there is no effect description.
            Definition.effectDescription = new EffectDescription();
        }
    }

    public TBuilder Configure(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
        EffectDescription effectDescription)
    {
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        Definition.usesDetermination = usesDetermination;
        Definition.usesAbilityScoreName = usesAbilityScoreName;
        Definition.activationTime = activationTime;
        Definition.costPerUse = costPerUse;
        Definition.rechargeRate = recharge;
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.abilityScore = abilityScore;
        Definition.effectDescription = effectDescription.DeepCopy();

        return This();
    }

    public TBuilder Configure(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
        EffectDescription effectDescription, FeatureDefinitionPower overridenPower)
    {
        Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse,
            recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription);

        Definition.overriddenPower = overridenPower;

        return This();
    }

    public TBuilder Configure(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName, RuleDefinitions.ActivationTime activationTime, int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
        EffectDescription effectDescription, bool uniqueInstance)
    {
        Configure(usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse,
            recharge, proficiencyBonusToAttack, abilityScoreBonusToAttack, abilityScore, effectDescription);

        Definition.uniqueInstance = uniqueInstance;

        return This();
    }

    public TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.effectDescription = effect.DeepCopy();
        return This();
    }

    public TBuilder DelegatedToAction()
    {
        Definition.delegatedToAction = true;
        return This();
    }

    public TBuilder SurrogateToSpell(SpellDefinition spell)
    {
        Definition.surrogateToSpell = spell;
        return This();
    }

    public TBuilder SetActivation(RuleDefinitions.ActivationTime time, int costPerUse)
    {
        Definition.activationTime = time;
        Definition.costPerUse = costPerUse;
        return This();
    }

    // TODO: combine with above with default?
    public TBuilder SetActivationTime(RuleDefinitions.ActivationTime value)
    {
        Definition.activationTime = value;
        return This();
    }

    public TBuilder SetReaction(RuleDefinitions.ReactionTriggerContext context, string name)
    {
        Definition.reactionContext = context;
        Definition.reactionName = name;
        return This();
    }

    public TBuilder SetCastingFailure(RuleDefinitions.CastingSuccessComputation computation)
    {
        Definition.hasCastingFailure = true;
        Definition.castingSuccessComputation = computation;
        return This();
    }

    public TBuilder CanUseInDialog()
    {
        Definition.canUseInDialog = true;
        return This();
    }

    public TBuilder SetDisablingCondition(ConditionDefinition condition)
    {
        Definition.disableIfConditionIsOwned = condition;
        return This();
    }

    public TBuilder SetRechargeRate(RuleDefinitions.RechargeRate rate)
    {
        Definition.rechargeRate = rate;

        return This();
    }

    public TBuilder SetShortTitleOverride(string titleOverride)
    {
        Definition.shortTitleOverride = titleOverride;
        return This();
    }

    public TBuilder SetSpellCastingFeature(FeatureDefinitionCastSpell spellFeature)
    {
        Definition.spellcastingFeature = spellFeature;
        return This();
    }

    public TBuilder SetUsesFixed(int fixedUses)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        return This();
    }

    public TBuilder SetFixedUsesPerRecharge(int fixedUses)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        return This();
    }

    public TBuilder SetCostPerUse(int costPerUse)
    {
        Definition.costPerUse = costPerUse;
        return This();
    }

    public TBuilder SetUsesProficiency()
    {
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.ProficiencyBonus;
        return This();
    }

    public TBuilder SetAbilityScore(string abilityScoreName)
    {
        Definition.abilityScore = abilityScoreName;
        return This();
    }

    public TBuilder SetUsesAbilityScoreName(string abilityScoreName)
    {
        Definition.usesAbilityScoreName = abilityScoreName;
        return This();
    }

    public TBuilder SetUsesAbility(int fixedUses, string attribute)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        Definition.usesAbilityScoreName = attribute;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed;
        return This();
    }

    public TBuilder UseSpellCastingModifier()
    {
        Definition.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility;
        return This();
    }

    public TBuilder SetAttackModifierAbility(bool ability, bool proficiency, string attribute)
    {
        Definition.abilityScore = attribute;
        Definition.abilityScoreBonusToAttack = ability;
        Definition.proficiencyBonusToAttack = proficiency;
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.AbilityScore;
        return This();
    }

    public TBuilder SetAttackModifierStatic(int attackModifier)
    {
        Definition.fixedAttackHit = attackModifier;
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.Fixed;
        return This();
    }

    public TBuilder SetUniqueInstance(bool uniqueInstance)
    {
        Definition.uniqueInstance = uniqueInstance;
        return This();
    }

    public TBuilder SetShowCasting(bool casting)
    {
        Definition.showCasting = casting;
        return This();
    }

    public TBuilder SetOverriddenPower(FeatureDefinitionPower overridenPower)
    {
        Definition.overriddenPower = overridenPower;
        return This();
    }

    public TBuilder SetShortTitle(string title)
    {
        Definition.shortTitleOverride = title;
        return This();
    }

    public TBuilder SetAbility(string ability)
    {
        Definition.abilityScore = ability;
        return This();
    }

    #region Constructors

    protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(TDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
