using System;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

public class FeatureDefinitionPowerBuilder
    : FeatureDefinitionPowerBuilder<FeatureDefinitionPower, FeatureDefinitionPowerBuilder>
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
        Definition.effectDescription ??= new EffectDescription();
    }

    public TBuilder Configure(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack,
        bool abilityScoreBonusToAttack,
        string abilityScore,
        EffectDescription effectDescription,
        bool uniqueInstance = false)
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
        Definition.effectDescription = effectDescription.Copy();
        Definition.uniqueInstance = uniqueInstance;
        return This();
    }

    public TBuilder SetEffectDescription(EffectDescription effect)
    {
        Definition.effectDescription = effect;
        return This();
    }

    public TBuilder SetExplicitAbilityScore(string ability)
    {
        Definition.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.Explicit;
        Definition.abilityScore = ability;
        return This();
    }

    public TBuilder SetSpellcastingAbilityScore()
    {
        Definition.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.SpellcastingAbility;
        return This();
    }

    public TBuilder SetAttackAbilityToHit(bool abilityScoreBonusToAttack = true, bool proficiencyBonusToAttack = false)
    {
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.AbilityScore;
        Definition.abilityScoreBonusToAttack = abilityScoreBonusToAttack;
        Definition.proficiencyBonusToAttack = proficiencyBonusToAttack;
        return This();
    }

    public TBuilder SetAttackFixedToHit(int bonus)
    {
        Definition.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.Fixed;
        Definition.fixedAttackHit = bonus;
        return This();
    }

    public TBuilder SetActivationTime(RuleDefinitions.ActivationTime time)
    {
        Definition.activationTime = time;
        return This();
    }

    public TBuilder SetHasCastingFailure(bool hasCastingFailure)
    {
        Definition.hasCastingFailure = hasCastingFailure;
        return This();
    }

    public TBuilder SetRechargeRate(RuleDefinitions.RechargeRate rate)
    {
        Definition.rechargeRate = rate;

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

    public TBuilder SetUniqueInstance(bool unique = true)
    {
        Definition.uniqueInstance = unique;
        return This();
    }

    public TBuilder SetAbilityScoreDetermination(
        RuleDefinitions.AbilityScoreDetermination abilityScoreNameDetermination)
    {
        Definition.abilityScoreDetermination = abilityScoreNameDetermination;
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
