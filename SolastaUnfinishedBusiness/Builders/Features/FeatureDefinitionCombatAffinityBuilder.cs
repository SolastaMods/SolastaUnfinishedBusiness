using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

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

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifierSign(
        RuleDefinitions.AttackModifierSign modifierSign)
    {
        Definition.myAttackModifierSign = modifierSign;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackAdvantage(RuleDefinitions.AdvantageType advantage)
    {
        Definition.myAttackAdvantage = advantage;
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

#if false
    internal FeatureDefinitionCombatAffinityBuilder SetInitiativeAffinity(RuleDefinitions.AdvantageType affinity)
    {
        Definition.initiativeAffinity = affinity;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(RuleDefinitions.SituationalContext context)
    {
        Definition.situationalContext = context;
        return this;
    }
#endif

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(RuleDefinitions.SituationalContext context)
    {
        Definition.situationalContext = context;
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
