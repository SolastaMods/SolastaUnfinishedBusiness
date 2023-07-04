using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionCombatAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionCombatAffinity, FeatureDefinitionCombatAffinityBuilder>
{
    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifierDieType(RuleDefinitions.DieType dieType)
    {
        Definition.myAttackModifierValueDetermination = RuleDefinitions.CombatAffinityValueDetermination.Die;
        Definition.myAttackModifierDieType = dieType;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifier(
        ExtraCombatAffinityValueDetermination determination,
        RuleDefinitions.AttackModifierSign modifierSign = RuleDefinitions.AttackModifierSign.Add)
    {
        Definition.myAttackModifierValueDetermination = (RuleDefinitions.CombatAffinityValueDetermination)determination;
        Definition.myAttackModifierSign = modifierSign;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifierSign(
        RuleDefinitions.AttackModifierSign modifierSign)
    {
        Definition.myAttackModifierSign = modifierSign;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetAttackOfOpportunityImmunity(
        bool attackOfOpportunityImmunity)
    {
        Definition.attackOfOpportunityImmunity = attackOfOpportunityImmunity;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackAdvantage(RuleDefinitions.AdvantageType advantage)
    {
        Definition.myAttackAdvantage = advantage;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetAttackOfOpportunityOnMeAdvantage(
        RuleDefinitions.AdvantageType advantage)
    {
        Definition.attackOfOpportunityOnMeAdvantageType = advantage;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetAttackOnMeAdvantage(
        RuleDefinitions.AdvantageType advantage,
        int attackOnMeCountLimit = -1)
    {
        Definition.attackOnMeAdvantage = advantage;
        Definition.attackOnMeCountLimit = attackOnMeCountLimit;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetIgnoreCover()
    {
        Definition.ignoreCover = true;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetPermanentCover(RuleDefinitions.CoverType permanentCover)
    {
        Definition.permanentCover = permanentCover;
        return this;
    }

#if false
    internal FeatureDefinitionCombatAffinityBuilder SetInitiativeAffinity(RuleDefinitions.AdvantageType affinity)
    {
        Definition.initiativeAffinity = affinity;
        return this;
    }
#endif

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(RuleDefinitions.SituationalContext context,
        ConditionDefinition requiredCondition = null)
    {
        Definition.situationalContext = context;
        Definition.requiredCondition = requiredCondition;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(ExtraSituationalContext context)
    {
        Definition.situationalContext = (RuleDefinitions.SituationalContext)context;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionCombatAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionCombatAffinityBuilder(FeatureDefinitionCombatAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
