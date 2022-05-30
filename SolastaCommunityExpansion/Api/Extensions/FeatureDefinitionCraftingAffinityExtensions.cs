using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionCraftingAffinity))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionCraftingAffinityExtensions
{
    public static T AddAffinityGroups<T>(this T entity,
        params FeatureDefinitionCraftingAffinity.CraftingAffinityGroup[] value)
        where T : FeatureDefinitionCraftingAffinity
    {
        AddAffinityGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddAffinityGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionCraftingAffinity.CraftingAffinityGroup> value)
        where T : FeatureDefinitionCraftingAffinity
    {
        entity.AffinityGroups.AddRange(value);
        return entity;
    }

    public static T ClearAffinityGroups<T>(this T entity)
        where T : FeatureDefinitionCraftingAffinity
    {
        entity.AffinityGroups.Clear();
        return entity;
    }

    public static T SetAffinityGroups<T>(this T entity,
        params FeatureDefinitionCraftingAffinity.CraftingAffinityGroup[] value)
        where T : FeatureDefinitionCraftingAffinity
    {
        SetAffinityGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetAffinityGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionCraftingAffinity.CraftingAffinityGroup> value)
        where T : FeatureDefinitionCraftingAffinity
    {
        entity.AffinityGroups.SetRange(value);
        return entity;
    }
}
