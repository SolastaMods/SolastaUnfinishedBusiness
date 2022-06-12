using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Api.Extensions;

[TargetType(typeof(DamageForm))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class DamageFormExtensions
{
    public static T SetBonusDamage<T>(this T entity, Int32 value)
        where T : DamageForm
    {
        entity.BonusDamage = value;
        return entity;
    }

    public static T SetDamageType<T>(this T entity, String value)
        where T : DamageForm
    {
        entity.DamageType = value;
        return entity;
    }

    public static T SetDiceNumber<T>(this T entity, Int32 value)
        where T : DamageForm
    {
        entity.DiceNumber = value;
        return entity;
    }

    internal static T SetDieType<T>(this T entity, DieType value)
        where T : DamageForm
    {
        entity.DieType = value;
        return entity;
    }
}
