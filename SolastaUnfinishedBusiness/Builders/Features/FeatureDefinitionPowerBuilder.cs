using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

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

    internal TBuilder DelegatedToAction(bool value = true)
    {
        Definition.delegatedToAction = value;
        return (TBuilder)this;
    }

    internal TBuilder SetUsesFixed(
        ActivationTime activationTime,
        RechargeRate recharge = RechargeRate.AtWill,
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = UsesDetermination.Fixed;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        return (TBuilder)this;
    }

    internal TBuilder SetUsesProficiencyBonus(ActivationTime activationTime,
        RechargeRate rechargeRate = RechargeRate.LongRest)
    {
        Definition.usesDetermination = UsesDetermination.ProficiencyBonus;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = rechargeRate;
        Definition.costPerUse = 1;
        Definition.fixedUsesPerRecharge = 0;
        return (TBuilder)this;
    }

    internal TBuilder SetUsesAbilityBonus(
        ActivationTime activationTime,
        RechargeRate recharge,
        string usesAbilityScoreName)
    {
        Definition.usesDetermination = UsesDetermination.AbilityBonusPlusFixed;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = 1;
        Definition.fixedUsesPerRecharge = 0;
        Definition.usesAbilityScoreName = usesAbilityScoreName;
        return (TBuilder)this;
    }

    internal TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.effectDescription = effect;
        return (TBuilder)this;
    }

    internal TBuilder SetUniqueInstance()
    {
        Definition.uniqueInstance = true;
        return (TBuilder)this;
    }

    internal TBuilder SetAutoActivationPowerTag(
        string autoActivationPowerTag,
        SenseMode.Type autoActivationRequiredTargetSenseType = SenseMode.Type.None,
        string autoActivationRequiredTargetCreatureTag = "None")
    {
        Definition.autoActivationPowerTag = autoActivationPowerTag;
        Definition.autoActivationRequiredTargetSenseType = autoActivationRequiredTargetSenseType;
        Definition.autoActivationRequiredTargetCreatureTag = autoActivationRequiredTargetCreatureTag;
        return (TBuilder)this;
    }

    internal TBuilder SetShowCasting(bool casting)
    {
        Definition.showCasting = casting;
        return (TBuilder)this;
    }

    internal TBuilder SetDisableIfConditionIsOwned(ConditionDefinition conditionDefinition)
    {
        Definition.disableIfConditionIsOwned = conditionDefinition;
        return (TBuilder)this;
    }

    internal TBuilder SetHasCastingFailure(bool value)
    {
        Definition.hasCastingFailure = value;
        return (TBuilder)this;
    }

    internal TBuilder SetOverriddenPower(FeatureDefinitionPower overridenPower)
    {
        Definition.overriddenPower = overridenPower;
        return (TBuilder)this;
    }

    internal TBuilder SetBonusToAttack(
        bool proficiencyBonusToAttack = false,
        bool abilityScoreBonusToAttack = false,
        string abilityScore = AttributeDefinitions.Intelligence,
        PowerAttackHitComputation attackHitComputation = PowerAttackHitComputation.AbilityScore) // this is game default
    {
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.abilityScore = abilityScore;
        Definition.attackHitComputation = attackHitComputation;
        return (TBuilder)this;
    }

    internal TBuilder SetUseSpellAttack()
    {
        return SetBonusToAttack(
            attackHitComputation: (PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack);
    }

    internal TBuilder SetReactionContext(ReactionTriggerContext context)
    {
        Definition.reactionContext = context;
        return (TBuilder)this;
    }

    internal TBuilder SetExplicitAbilityScore(string abilityScore)
    {
        Definition.abilityScoreDetermination = AbilityScoreDetermination.Explicit;
        Definition.abilityScore = abilityScore;
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
