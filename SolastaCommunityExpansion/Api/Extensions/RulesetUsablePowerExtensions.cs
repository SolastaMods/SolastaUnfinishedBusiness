using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(RulesetUsablePower))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class RulesetUsablePowerExtensions
{
    public static T SetMaxUses<T>(this T entity, Int32 value)
        where T : RulesetUsablePower
    {
        entity.SetField("maxUses", value);
        return entity;
    }

    public static T SetOriginClass<T>(this T entity, CharacterClassDefinition value)
        where T : RulesetUsablePower
    {
        entity.SetField("originClass", value);
        return entity;
    }

    public static T SetOriginRace<T>(this T entity, CharacterRaceDefinition value)
        where T : RulesetUsablePower
    {
        entity.SetField("originRace", value);
        return entity;
    }

    public static T SetPowerDefinition<T>(this T entity, FeatureDefinitionPower value)
        where T : RulesetUsablePower
    {
        entity.SetField("powerDefinition", value);
        return entity;
    }

    public static T SetRemainingUses<T>(this T entity, Int32 value)
        where T : RulesetUsablePower
    {
        entity.SetField("remainingUses", value);
        return entity;
    }

    public static T SetSaveDC<T>(this T entity, Int32 value)
        where T : RulesetUsablePower
    {
        entity.SaveDC = value;
        return entity;
    }

    public static T SetSpentPoints<T>(this T entity, Int32 value)
        where T : RulesetUsablePower
    {
        entity.SetField("spentPoints", value);
        return entity;
    }

    public static T SetUsesAttribute<T>(this T entity, RulesetAttribute value)
        where T : RulesetUsablePower
    {
        entity.UsesAttribute = value;
        return entity;
    }
}
