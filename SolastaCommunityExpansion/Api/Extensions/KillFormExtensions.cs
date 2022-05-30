using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(KillForm))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class KillFormExtensions
{
    public static KillForm Copy(this KillForm entity)
    {
        var copy = new KillForm();
        copy.Copy(entity);
        return copy;
    }

    public static T SetChallengeRating<T>(this T entity, Single value)
        where T : KillForm
    {
        entity.SetField("challengeRating", value);
        return entity;
    }

    public static T SetHitPoints<T>(this T entity, Int32 value)
        where T : KillForm
    {
        entity.SetField("hitPoints", value);
        return entity;
    }

    public static T SetKillCondition<T>(this T entity, KillCondition value)
        where T : KillForm
    {
        entity.SetField("killCondition", value);
        return entity;
    }
}
