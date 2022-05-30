using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionConditionAffinity))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionConditionAffinityExtensions
{
    public static T SetConditionAffinityType<T>(this T entity, ConditionAffinityType value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("conditionAffinityType", value);
        return entity;
    }

    public static T SetConditionType<T>(this T entity, String value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("conditionType", value);
        return entity;
    }

    public static T SetRerollAdvantageType<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("rerollAdvantageType", value);
        return entity;
    }

    public static T SetRerollSaveWhenGained<T>(this T entity, Boolean value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("rerollSaveWhenGained", value);
        return entity;
    }

    public static T SetSavingThrowAdvantageType<T>(this T entity, AdvantageType value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("savingThrowAdvantageType", value);
        return entity;
    }

    public static T SetSavingThrowModifier<T>(this T entity, Int32 value)
        where T : FeatureDefinitionConditionAffinity
    {
        entity.SetField("savingThrowModifier", value);
        return entity;
    }
}
