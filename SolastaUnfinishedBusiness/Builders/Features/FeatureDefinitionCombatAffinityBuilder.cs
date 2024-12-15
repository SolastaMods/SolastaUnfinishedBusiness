using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionCombatAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionCombatAffinity, FeatureDefinitionCombatAffinityBuilder>
{
    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifierDieType(DieType dieType)
    {
        Definition.myAttackModifierValueDetermination = CombatAffinityValueDetermination.Die;
        Definition.myAttackModifierDieType = dieType;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifier(
        ExtraCombatAffinityValueDetermination determination,
        AttackModifierSign modifierSign = AttackModifierSign.Add)
    {
        Definition.myAttackModifierValueDetermination = (CombatAffinityValueDetermination)determination;
        Definition.myAttackModifierSign = modifierSign;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetInitiativeAffinity(
        AdvantageType initiativeAffinity)
    {
        Definition.initiativeAffinity = initiativeAffinity;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackModifierSign(
        AttackModifierSign modifierSign)
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

    internal FeatureDefinitionCombatAffinityBuilder SetMyAttackAdvantage(AdvantageType advantage)
    {
        Definition.myAttackAdvantage = advantage;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetAttackOfOpportunityOnMeAdvantage(
        AdvantageType advantage)
    {
        Definition.attackOfOpportunityOnMeAdvantageType = advantage;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetAttackOnMeAdvantage(
        AdvantageType advantage,
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

    internal FeatureDefinitionCombatAffinityBuilder SetPermanentCover(CoverType permanentCover)
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

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(SituationalContext context,
        ConditionDefinition requiredCondition = null)
    {
        Definition.situationalContext = context;
        Definition.requiredCondition = requiredCondition;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder DisableAutoFormatDescription()
    {
        Definition.autoFormatDescription = false;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetSituationalContext(ExtraSituationalContext context)
    {
        Definition.situationalContext = (SituationalContext)context;
        return this;
    }

    internal FeatureDefinitionCombatAffinityBuilder SetOtherCharacterFamilyRestrictions(
        params string[] otherCharacterFamilyRestrictions)
    {
        Definition.otherCharacterFamilyRestrictions.SetRange(otherCharacterFamilyRestrictions);
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
