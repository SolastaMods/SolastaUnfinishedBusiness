using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionSubclassChoice))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionSubclassChoiceExtensions
{
    public static T AddSubclasses<T>(this T entity, params String[] value)
        where T : FeatureDefinitionSubclassChoice
    {
        AddSubclasses(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddSubclasses<T>(this T entity, IEnumerable<String> value)
        where T : FeatureDefinitionSubclassChoice
    {
        entity.Subclasses.AddRange(value);
        return entity;
    }

    public static T ClearSubclasses<T>(this T entity)
        where T : FeatureDefinitionSubclassChoice
    {
        entity.Subclasses.Clear();
        return entity;
    }

    public static T SetFilterByDeity<T>(this T entity, Boolean value)
        where T : FeatureDefinitionSubclassChoice
    {
        entity.SetField("filterByDeity", value);
        return entity;
    }

    public static T SetSubclasses<T>(this T entity, params String[] value)
        where T : FeatureDefinitionSubclassChoice
    {
        SetSubclasses(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetSubclasses<T>(this T entity, IEnumerable<String> value)
        where T : FeatureDefinitionSubclassChoice
    {
        entity.Subclasses.SetRange(value);
        return entity;
    }

    public static T SetSubclassSuffix<T>(this T entity, String value)
        where T : FeatureDefinitionSubclassChoice
    {
        entity.SetField("subclassSuffix", value);
        return entity;
    }
}
