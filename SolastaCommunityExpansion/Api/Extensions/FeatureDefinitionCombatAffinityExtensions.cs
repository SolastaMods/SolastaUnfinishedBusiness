using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionCombatAffinity))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionCombatAffinityExtensions
{
    public static T AddNullifiedBySenses<T>(this T entity, params SenseMode.Type[] value)
        where T : FeatureDefinitionCombatAffinity
    {
        AddNullifiedBySenses(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddNullifiedBySenses<T>(this T entity, IEnumerable<SenseMode.Type> value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.NullifiedBySenses.AddRange(value);
        return entity;
    }

    public static T ClearNullifiedBySenses<T>(this T entity)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.NullifiedBySenses.Clear();
        return entity;
    }

    public static T SetAttackOfOpportunityImmunity<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("attackOfOpportunityImmunity", value);
        return entity;
    }

    public static T SetAttackOfOpportunityOnMeAdvantageType<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("attackOfOpportunityOnMeAdvantageType", value);
        return entity;
    }

    public static T SetAttackOnMeAdvantage<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("attackOnMeAdvantage", value);
        return entity;
    }

    public static T SetAttackOnMeCountLimit<T>(this T entity, Int32 value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("attackOnMeCountLimit", value);
        return entity;
    }

    public static T SetAutoCritical<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("autoCritical", value);
        return entity;
    }

    public static T SetCanRageToOvercomeSurprise<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("canRageToOvercomeSurprise", value);
        return entity;
    }

    public static T SetCriticalHitImmunity<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("criticalHitImmunity", value);
        return entity;
    }

    public static T SetIgnoreCover<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("ignoreCover", value);
        return entity;
    }

    public static T SetIgnoreRangeAdvantage<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("ignoreRangeAdvantage", value);
        return entity;
    }

    public static T SetInitiativeAffinity<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("initiativeAffinity", value);
        return entity;
    }

    public static T SetMultiAttackAffinity<T>(this T entity, Boolean value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("multiAttackAffinity", value);
        return entity;
    }

    public static T SetMultiAttackDefenseValue<T>(this T entity, Int32 value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("multiAttackDefenseValue", value);
        return entity;
    }

    public static T SetMyAttackAdvantage<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackAdvantage", value);
        return entity;
    }

    public static T SetMyAttackAffinityFilter<T>(this T entity, AttackAffinityFilter value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackAffinityFilter", value);
        return entity;
    }

    public static T SetMyAttackDamageMultiplier<T>(this T entity, Single value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackDamageMultiplier", value);
        return entity;
    }

    public static T SetMyAttackModifierDiceNumber<T>(this T entity, Int32 value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackModifierDiceNumber", value);
        return entity;
    }

    public static T SetMyAttackModifierDieType<T>(this T entity, DieType value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackModifierDieType", value);
        return entity;
    }

    public static T SetMyAttackModifierSign<T>(this T entity, AttackModifierSign value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackModifierSign", value);
        return entity;
    }

    public static T SetMyAttackModifierValueDetermination<T>(this T entity, CombatAddinityValueDetermination value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("myAttackModifierValueDetermination", value);
        return entity;
    }

    public static T SetNullifiedBySenses<T>(this T entity, params SenseMode.Type[] value)
        where T : FeatureDefinitionCombatAffinity
    {
        SetNullifiedBySenses(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetNullifiedBySenses<T>(this T entity, IEnumerable<SenseMode.Type> value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.NullifiedBySenses.SetRange(value);
        return entity;
    }

    public static T SetRequiredCondition<T>(this T entity, ConditionDefinition value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("requiredCondition", value);
        return entity;
    }

    public static T SetSituationalContext<T>(this T entity, SituationalContext value)
        where T : FeatureDefinitionCombatAffinity
    {
        entity.SetField("situationalContext", value);
        return entity;
    }
}
