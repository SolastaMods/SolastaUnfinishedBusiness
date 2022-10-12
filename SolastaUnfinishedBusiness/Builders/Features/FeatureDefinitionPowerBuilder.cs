using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPowerBuilder
    : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerBuilder>
{
    #region Constructors

    protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}

internal abstract class
    FeatureDefinitionPowerBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionPower
    where TBuilder : FeatureDefinitionPowerBuilder<TDefinition, TBuilder>
{
    protected override void Initialise()
    {
        base.Initialise();
        Definition.effectDescription ??= new EffectDescription();
    }

    internal TBuilder SetUsesFixed(
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate recharge,
        EffectDescription effectDescription = null,
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;

        if (effectDescription != null)
        {
            Definition.effectDescription.Copy(effectDescription);
        }

        return (TBuilder)this;
    }

    internal TBuilder SetUsesAbilityBonus(
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate recharge,
        string usesAbilityScoreName,
        EffectDescription effectDescription = null,
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        Definition.usesAbilityScoreName = usesAbilityScoreName;

        if (effectDescription != null)
        {
            Definition.effectDescription.Copy(effectDescription);
        }

        return (TBuilder)this;
    }

    internal TBuilder SetUsesProficiencyBonus(
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate recharge,
        EffectDescription effectDescription = null,
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.ProficiencyBonus;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;

        if (effectDescription != null)
        {
            Definition.effectDescription.Copy(effectDescription);
        }

        return (TBuilder)this;
    }

    internal TBuilder SetBonusToAttack(
        bool proficiencyBonusToAttack = false,
        bool abilityScoreBonusToAttack = false,
        string abilityScore = AttributeDefinitions.Intelligence) // this is game default
    {
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.abilityScore = abilityScore;
        return (TBuilder)this;
    }

    internal TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.effectDescription = effect;
        return (TBuilder)this;
    }

    internal TBuilder SetExplicitAbilityScore(string ability)
    {
        Definition.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.Explicit;
        Definition.abilityScore = ability;
        return (TBuilder)this;
    }

    internal TBuilder SetAttackAbilityToHit(
        bool abilityScoreBonusToAttack = true,
        bool proficiencyBonusToAttack = false)
    {
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.AbilityScore;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        return (TBuilder)this;
    }

    internal TBuilder SetReactionContext(RuleDefinitions.ReactionTriggerContext context)
    {
        Definition.reactionContext = context;
        return (TBuilder)this;
    }

    internal TBuilder SetUniqueInstance(bool unique = true)
    {
        Definition.uniqueInstance = unique;
        return (TBuilder)this;
    }

    internal TBuilder SetShowCasting(bool casting)
    {
        Definition.showCasting = casting;
        return (TBuilder)this;
    }

    internal TBuilder SetOverriddenPower(FeatureDefinitionPower overridenPower)
    {
        Definition.overriddenPower = overridenPower;
        return (TBuilder)this;
    }

    #region Constructors

    protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    #endregion
}
