using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(ItemPropertyDescription))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class ItemPropertyDescriptionExtensions
{
    public static ItemPropertyDescription Copy(this ItemPropertyDescription entity)
    {
        return new ItemPropertyDescription(entity);
    }

    public static T SetAppliesOnItemOnly<T>(this T entity, Boolean value)
        where T : ItemPropertyDescription
    {
        entity.SetField("appliesOnItemOnly", value);
        return entity;
    }

    public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
        where T : ItemPropertyDescription
    {
        entity.SetField("conditionDefinition", value);
        return entity;
    }

    public static T SetFeatureDefinition<T>(this T entity, FeatureDefinition value)
        where T : ItemPropertyDescription
    {
        entity.SetField("featureDefinition", value);
        return entity;
    }

    public static T SetKnowledgeAffinity<T>(this T entity, EquipmentDefinitions.KnowledgeAffinity value)
        where T : ItemPropertyDescription
    {
        entity.SetField("knowledgeAffinity", value);
        return entity;
    }

    public static T SetType<T>(this T entity, ItemPropertyDescription.PropertyType value)
        where T : ItemPropertyDescription
    {
        entity.SetField("type", value);
        return entity;
    }
}
