using System;

namespace SolastaCommunityExpansion.Api.Extensions;

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
