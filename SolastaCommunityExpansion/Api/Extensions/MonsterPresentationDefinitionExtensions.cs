using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(MonsterPresentationDefinition))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class MonsterPresentationDefinitionExtensions
{
    public static T SetCustomMaterials<T>(this T entity, AssetReference[] value)
        where T : MonsterPresentationDefinition
    {
        entity.SetField("customMaterials", value);
        return entity;
    }

    public static T SetModelScale<T>(this T entity, Single value)
        where T : MonsterPresentationDefinition
    {
        entity.SetField("modelScale", value);
        return entity;
    }

    public static T SetPrefabReference<T>(this T entity, AssetReference value)
        where T : MonsterPresentationDefinition
    {
        entity.SetField("prefabReference", value);
        return entity;
    }

    public static T SetSex<T>(this T entity, CreatureSex value)
        where T : MonsterPresentationDefinition
    {
        entity.SetField("sex", value);
        return entity;
    }

    public static T SetUseCustomMaterials<T>(this T entity, Boolean value)
        where T : MonsterPresentationDefinition
    {
        entity.SetField("useCustomMaterials", value);
        return entity;
    }
}
