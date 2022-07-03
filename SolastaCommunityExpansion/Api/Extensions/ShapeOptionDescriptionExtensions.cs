using System;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class ShapeOptionDescriptionExtensions
{
    [NotNull]
    public static T SetRequiredLevel<T>([NotNull] this T entity, Int32 value)
        where T : ShapeOptionDescription
    {
        entity.requiredLevel = value;
        return entity;
    }

    [NotNull]
    public static T SetSubstituteMonster<T>([NotNull] this T entity, MonsterDefinition value)
        where T : ShapeOptionDescription
    {
        entity.substituteMonster = value;
        return entity;
    }
}
