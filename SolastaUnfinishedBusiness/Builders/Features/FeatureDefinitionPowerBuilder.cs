using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
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

    internal TBuilder SetUsesProficiencyBonus(
        ActivationTime activationTime,
        RechargeRate recharge = RechargeRate.AtWill,
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = UsesDetermination.ProficiencyBonus;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        return (TBuilder)this;
    }

    internal TBuilder SetUsesAbilityBonus(
        ActivationTime activationTime,
        RechargeRate recharge = RechargeRate.AtWill,
        string usesAbilityScoreName = "",
        int costPerUse = 1,
        int usesPerRecharge = 1)
    {
        Definition.usesDetermination = UsesDetermination.AbilityBonusPlusFixed;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        Definition.usesAbilityScoreName = usesAbilityScoreName == string.Empty
            ? AttributeDefinitions.Charisma // game default
            : usesAbilityScoreName;
        return (TBuilder)this;
    }

    internal TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.effectDescription = effect;
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

    internal TBuilder SetIsPowerPool()
    {
        Definition.overriddenPower = Definition;
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
