using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(SaveAffinityByFamilyDescription))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class SaveAffinityByFamilyDescriptionExtensions
{
    public static SaveAffinityByFamilyDescription Copy(this SaveAffinityByFamilyDescription entity)
    {
        var copy = new SaveAffinityByFamilyDescription();
        copy.Copy(entity);
        return copy;
    }

    public static T SetAdvantageType<T>(this T entity, AdvantageType value)
        where T : SaveAffinityByFamilyDescription
    {
        entity.SetField("advantageType", value);
        return entity;
    }

    public static T SetFamily<T>(this T entity, String value)
        where T : SaveAffinityByFamilyDescription
    {
        entity.SetField("family", value);
        return entity;
    }
}
