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
[TargetType(typeof(FeatureDefinitionFeatureSet))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionFeatureSetExtensions
{
    public static T AddFeatureSet<T>(this T entity, params FeatureDefinition[] value)
        where T : FeatureDefinitionFeatureSet
    {
        AddFeatureSet(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddFeatureSet<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.FeatureSet.AddRange(value);
        return entity;
    }

    public static T ClearFeatureSet<T>(this T entity)
        where T : FeatureDefinitionFeatureSet
    {
        entity.FeatureSet.Clear();
        return entity;
    }

    public static T SetDefaultSelection<T>(this T entity, Int32 value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.SetField("defaultSelection", value);
        return entity;
    }

    public static T SetEnumerateInDescription<T>(this T entity, Boolean value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.SetField("enumerateInDescription", value);
        return entity;
    }

    public static T SetFeatureSet<T>(this T entity, params FeatureDefinition[] value)
        where T : FeatureDefinitionFeatureSet
    {
        SetFeatureSet(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetFeatureSet<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.FeatureSet.SetRange(value);
        return entity;
    }

    public static T SetHasRacialAffinity<T>(this T entity, Boolean value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.SetField("hasRacialAffinity", value);
        return entity;
    }

    public static T SetMode<T>(this T entity, FeatureDefinitionFeatureSet.FeatureSetMode value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.SetField("mode", value);
        return entity;
    }

    public static T SetUniqueChoices<T>(this T entity, Boolean value)
        where T : FeatureDefinitionFeatureSet
    {
        entity.SetField("uniqueChoices", value);
        return entity;
    }
}
