using System;
using System.CodeDom.Compiler;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Api.Extensions;

#if DEBUG

[TargetType(typeof(ShapeOptionDescription))]
#endif
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class ShapeOptionDescriptionExtensions
{
    public static T SetRequiredLevel<T>(this T entity, Int32 value)
        where T : ShapeOptionDescription
    {
        entity.requiredLevel = value;
        return entity;
    }

    public static T SetSubstituteMonster<T>(this T entity, MonsterDefinition value)
        where T : ShapeOptionDescription
    {
        entity.substituteMonster = value;
        return entity;
    }
}
