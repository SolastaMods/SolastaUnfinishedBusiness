using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionAncestry))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionAncestryExtensions
{
    public static T SetDamageType<T>(this T entity, String value)
        where T : FeatureDefinitionAncestry
    {
        entity.SetField("damageType", value);
        return entity;
    }
}
