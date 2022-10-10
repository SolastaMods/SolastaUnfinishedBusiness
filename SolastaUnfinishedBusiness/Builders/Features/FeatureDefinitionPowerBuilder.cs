using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPowerBuilder
    : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerBuilder>
{
    #region Constructors

    protected FeatureDefinitionPowerBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionPowerBuilder(FeatureDefinitionPower original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
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

    internal TBuilder Configure(
        RuleDefinitions.UsesDetermination usesDetermination,
        RuleDefinitions.ActivationTime activationTime,
        RuleDefinitions.RechargeRate recharge,
        EffectDescription effectDescription,
        bool uniqueInstance = false,
        int costPerUse = 1,
        int usesPerRecharge = 1,
        string usesAbilityScoreName = "",
        bool proficiencyBonusToAttack = false,
        bool abilityScoreBonusToAttack = false,
        string abilityScore = "")
    {
        Definition.usesDetermination = usesDetermination;
        Definition.activationTime = activationTime;
        Definition.rechargeRate = recharge;
        Definition.effectDescription = effectDescription.Copy();
        Definition.uniqueInstance = uniqueInstance;
        Definition.costPerUse = costPerUse;
        Definition.fixedUsesPerRecharge = usesPerRecharge;
        Definition.usesAbilityScoreName = usesAbilityScoreName;
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

#if false
    internal TBuilder SetSpellcastingAbilityScore()
    {
        Definition.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility;
        return (TBuilder)this;
    }
#endif

    internal TBuilder SetAttackAbilityToHit(
        bool abilityScoreBonusToAttack = true,
        bool proficiencyBonusToAttack = false)
    {
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.AbilityScore;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        return (TBuilder)this;
    }

#if false
    internal TBuilder SetAttackFixedToHit(int bonus)
    {
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.Fixed;
        Definition.fixedAttackHit = bonus;
        return (TBuilder)this;
    }
#endif

    internal TBuilder SetActivationTime(RuleDefinitions.ActivationTime time)
    {
        Definition.activationTime = time;
        return (TBuilder)this;
    }

    internal TBuilder SetReactionContext(RuleDefinitions.ReactionTriggerContext context)
    {
        Definition.reactionContext = context;
        return (TBuilder)this;
    }

#if false
    internal TBuilder SetHasCastingFailure(bool hasCastingFailure)
    {
        Definition.hasCastingFailure = hasCastingFailure;
        return (TBuilder)this;
    }
#endif

    internal TBuilder SetRechargeRate(RuleDefinitions.RechargeRate rate)
    {
        Definition.rechargeRate = rate;
        return (TBuilder)this;
    }

    internal TBuilder SetUsesFixed(int fixedUses)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
        return (TBuilder)this;
    }

    internal TBuilder SetFixedUsesPerRecharge(int fixedUses)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        return (TBuilder)this;
    }

    internal TBuilder SetCostPerUse(int costPerUse)
    {
        Definition.costPerUse = costPerUse;
        return (TBuilder)this;
    }

    internal TBuilder SetUniqueInstance(bool unique = true)
    {
        Definition.uniqueInstance = unique;
        return (TBuilder)this;
    }

    internal TBuilder SetAbilityScoreDetermination(
        RuleDefinitions.AbilityScoreDetermination abilityScoreNameDetermination)
    {
        Definition.abilityScoreDetermination = abilityScoreNameDetermination;
        return (TBuilder)this;
    }

#if false
    internal TBuilder SetUsesAbilityScoreName(string abilityScoreName)
    {
        Definition.usesAbilityScoreName = abilityScoreName;
        return (TBuilder)this;
    }
#endif

    internal TBuilder SetUsesAbility(int fixedUses, string attribute)
    {
        Definition.fixedUsesPerRecharge = fixedUses;
        Definition.usesAbilityScoreName = attribute;
        Definition.usesDetermination = RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed;
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
