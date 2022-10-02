using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class ShapeOptionDescriptionExtensions
{
    [NotNull]
    internal static T SetRequiredLevel<T>([NotNull] this T entity, Int32 value)
        where T : ShapeOptionDescription
    {
        entity.requiredLevel = value;
        return entity;
    }

    [NotNull]
    internal static T SetSubstituteMonster<T>([NotNull] this T entity, MonsterDefinition value)
        where T : ShapeOptionDescription
    {
        entity.substituteMonster = value;
        return entity;
    }
}
