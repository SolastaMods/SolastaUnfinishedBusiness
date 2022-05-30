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
[TargetType(typeof(FeatureDefinitionSavingThrowAffinity))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionSavingThrowAffinityExtensions
{
    public static T AddAffinityGroups<T>(this T entity,
        params FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup[] value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        AddAffinityGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddAffinityGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup> value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.AffinityGroups.AddRange(value);
        return entity;
    }

    public static T ClearAffinityGroups<T>(this T entity)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.AffinityGroups.Clear();
        return entity;
    }

    public static T SetAffinityGroups<T>(this T entity,
        params FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup[] value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        SetAffinityGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetAffinityGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionSavingThrowAffinity.SavingThrowAffinityGroup> value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.AffinityGroups.SetRange(value);
        return entity;
    }

    public static T SetCanBorrowLuck<T>(this T entity, Boolean value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.SetField("canBorrowLuck", value);
        return entity;
    }

    public static T SetIndomitableSavingThrows<T>(this T entity, Int32 value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.SetField("indomitableSavingThrows", value);
        return entity;
    }

    public static T SetPriorityAbilityScore<T>(this T entity, String value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.SetField("priorityAbilityScore", value);
        return entity;
    }

    public static T SetUseControllerSavingThrows<T>(this T entity, Boolean value)
        where T : FeatureDefinitionSavingThrowAffinity
    {
        entity.SetField("useControllerSavingThrows", value);
        return entity;
    }
}
