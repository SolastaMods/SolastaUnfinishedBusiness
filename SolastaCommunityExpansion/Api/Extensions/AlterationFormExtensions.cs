using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(AlterationForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class AlterationFormExtensions
    {
        public static AlterationForm Copy(this AlterationForm entity)
        {
            var copy = new AlterationForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAbilityScore<T>(this T entity, String value)
            where T : AlterationForm
        {
            entity.SetField("abilityScore", value);
            return entity;
        }

        public static T SetAlterationType<T>(this T entity, AlterationForm.Type value)
            where T : AlterationForm
        {
            entity.SetField("alterationType", value);
            return entity;
        }

        public static T SetFeastDurationHours<T>(this T entity, Int32 value)
            where T : AlterationForm
        {
            entity.SetField("feastDurationHours", value);
            return entity;
        }

        public static T SetMaximumIncrease<T>(this T entity, Int32 value)
            where T : AlterationForm
        {
            entity.SetField("maximumIncrease", value);
            return entity;
        }

        public static T SetValueIncrease<T>(this T entity, Int32 value)
            where T : AlterationForm
        {
            entity.SetField("valueIncrease", value);
            return entity;
        }
    }
}
