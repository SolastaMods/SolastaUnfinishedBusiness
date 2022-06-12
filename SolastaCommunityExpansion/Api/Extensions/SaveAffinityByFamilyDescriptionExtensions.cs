using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Api.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(SaveAffinityByFamilyDescription))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class SaveAffinityByFamilyDescriptionExtensions
{
    public static T SetAdvantageType<T>(this T entity, AdvantageType value)
        where T : SaveAffinityByFamilyDescription
    {
        entity.advantageType = value;
        return entity;
    }

    public static T SetFamily<T>(this T entity, String value)
        where T : SaveAffinityByFamilyDescription
    {
        entity.family = value;
        return entity;
    }
}
