using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionAttributeModifier))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionAttributeModifierExtensions
{
    public static T SetModifiedAttribute<T>(this T entity, String value)
        where T : FeatureDefinitionAttributeModifier
    {
        entity.SetField("modifiedAttribute", value);
        return entity;
    }

    public static T SetModifierAbilityScore<T>(this T entity, String value)
        where T : FeatureDefinitionAttributeModifier
    {
        entity.SetField("modifierAbilityScore", value);
        return entity;
    }

    public static T SetModifierType2<T>(this T entity,
        FeatureDefinitionAttributeModifier.AttributeModifierOperation value)
        where T : FeatureDefinitionAttributeModifier
    {
        entity.SetField("modifierType2", value);
        return entity;
    }

    public static T SetModifierValue<T>(this T entity, Int32 value)
        where T : FeatureDefinitionAttributeModifier
    {
        entity.SetField("modifierValue", value);
        return entity;
    }

    public static T SetSituationalContext<T>(this T entity, SituationalContext value)
        where T : FeatureDefinitionAttributeModifier
    {
        entity.SetField("situationalContext", value);
        return entity;
    }
}
