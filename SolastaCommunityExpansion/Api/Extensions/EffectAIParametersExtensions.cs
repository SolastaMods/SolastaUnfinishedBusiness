using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(EffectAIParameters))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class EffectAIParametersExtensions
    {
        public static EffectAIParameters Copy(this EffectAIParameters entity)
        {
            var copy = new EffectAIParameters();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAoeScoreMultiplier<T>(this T entity, Single value)
            where T : EffectAIParameters
        {
            entity.SetField("aoeScoreMultiplier", value);
            return entity;
        }

        public static T SetCooldownForBattle<T>(this T entity, Int32 value)
            where T : EffectAIParameters
        {
            entity.SetField("cooldownForBattle", value);
            return entity;
        }

        public static T SetCooldownForCaster<T>(this T entity, Int32 value)
            where T : EffectAIParameters
        {
            entity.SetField("cooldownForCaster", value);
            return entity;
        }

        public static T SetDynamicCooldown<T>(this T entity, Boolean value)
            where T : EffectAIParameters
        {
            entity.SetField("dynamicCooldown", value);
            return entity;
        }
    }
}
