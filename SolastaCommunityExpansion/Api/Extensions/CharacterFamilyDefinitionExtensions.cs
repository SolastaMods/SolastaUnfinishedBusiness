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
[TargetType(typeof(CharacterFamilyDefinition))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class CharacterFamilyDefinitionExtensions
{
    public static T AddFeatures<T>(this T entity, params FeatureDefinition[] value)
        where T : CharacterFamilyDefinition
    {
        AddFeatures(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : CharacterFamilyDefinition
    {
        entity.Features.AddRange(value);
        return entity;
    }

    public static T ClearFeatures<T>(this T entity)
        where T : CharacterFamilyDefinition
    {
        entity.Features.Clear();
        return entity;
    }

    public static T SetExtraplanar<T>(this T entity, Boolean value)
        where T : CharacterFamilyDefinition
    {
        entity.SetField("extraplanar", value);
        return entity;
    }

    public static T SetFeatures<T>(this T entity, params FeatureDefinition[] value)
        where T : CharacterFamilyDefinition
    {
        SetFeatures(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : CharacterFamilyDefinition
    {
        entity.Features.SetRange(value);
        return entity;
    }
}
