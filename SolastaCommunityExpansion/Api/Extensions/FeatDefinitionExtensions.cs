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
[TargetType(typeof(FeatDefinition))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatDefinitionExtensions
{
    public static T AddCompatibleClassesPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        AddCompatibleClassesPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddCompatibleClassesPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.CompatibleClassesPrerequisite.AddRange(value);
        return entity;
    }

    public static T AddCompatibleRacesPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        AddCompatibleRacesPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddCompatibleRacesPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.CompatibleRacesPrerequisite.AddRange(value);
        return entity;
    }

    public static T AddFeatures<T>(this T entity, params FeatureDefinition[] value)
        where T : FeatDefinition
    {
        AddFeatures(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : FeatDefinition
    {
        entity.Features.AddRange(value);
        return entity;
    }

    public static T AddKnownFeatsPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        AddKnownFeatsPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddKnownFeatsPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.KnownFeatsPrerequisite.AddRange(value);
        return entity;
    }

    public static T ClearCompatibleClassesPrerequisite<T>(this T entity)
        where T : FeatDefinition
    {
        entity.CompatibleClassesPrerequisite.Clear();
        return entity;
    }

    public static T ClearCompatibleRacesPrerequisite<T>(this T entity)
        where T : FeatDefinition
    {
        entity.CompatibleRacesPrerequisite.Clear();
        return entity;
    }

    public static T ClearFeatures<T>(this T entity)
        where T : FeatDefinition
    {
        entity.Features.Clear();
        return entity;
    }

    public static T ClearKnownFeatsPrerequisite<T>(this T entity)
        where T : FeatDefinition
    {
        entity.KnownFeatsPrerequisite.Clear();
        return entity;
    }

    public static T SetArmorProficiencyCategory<T>(this T entity, String value)
        where T : FeatDefinition
    {
        entity.SetField("armorProficiencyCategory", value);
        return entity;
    }

    public static T SetArmorProficiencyPrerequisite<T>(this T entity, Boolean value)
        where T : FeatDefinition
    {
        entity.SetField("armorProficiencyPrerequisite", value);
        return entity;
    }

    public static T SetCompatibleClassesPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        SetCompatibleClassesPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetCompatibleClassesPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.CompatibleClassesPrerequisite.SetRange(value);
        return entity;
    }

    public static T SetCompatibleRacesPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        SetCompatibleRacesPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetCompatibleRacesPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.CompatibleRacesPrerequisite.SetRange(value);
        return entity;
    }

    public static T SetFeatures<T>(this T entity, params FeatureDefinition[] value)
        where T : FeatDefinition
    {
        SetFeatures(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
        where T : FeatDefinition
    {
        entity.Features.SetRange(value);
        return entity;
    }

    public static T SetKnownFeatsPrerequisite<T>(this T entity, params String[] value)
        where T : FeatDefinition
    {
        SetKnownFeatsPrerequisite(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetKnownFeatsPrerequisite<T>(this T entity, IEnumerable<String> value)
        where T : FeatDefinition
    {
        entity.KnownFeatsPrerequisite.SetRange(value);
        return entity;
    }

    public static T SetMinimalAbilityScoreName<T>(this T entity, String value)
        where T : FeatDefinition
    {
        entity.SetField("minimalAbilityScoreName", value);
        return entity;
    }

    public static T SetMinimalAbilityScorePrerequisite<T>(this T entity, Boolean value)
        where T : FeatDefinition
    {
        entity.SetField("minimalAbilityScorePrerequisite", value);
        return entity;
    }

    public static T SetMinimalAbilityScoreValue<T>(this T entity, Int32 value)
        where T : FeatDefinition
    {
        entity.SetField("minimalAbilityScoreValue", value);
        return entity;
    }

    public static T SetMustCastSpellsPrerequisite<T>(this T entity, Boolean value)
        where T : FeatDefinition
    {
        entity.SetField("mustCastSpellsPrerequisite", value);
        return entity;
    }
}
