using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(CounterForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class CounterFormExtensions
    {
        public static CounterForm Copy(this CounterForm entity)
        {
            var copy = new CounterForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAddProficiencyBonus<T>(this T entity, Boolean value)
            where T : CounterForm
        {
            entity.SetField("addProficiencyBonus", value);
            return entity;
        }

        public static T SetAddSpellCastingAbility<T>(this T entity, Boolean value)
            where T : CounterForm
        {
            entity.SetField("addSpellCastingAbility", value);
            return entity;
        }

        public static T SetAutomaticSpellLevel<T>(this T entity, Int32 value)
            where T : CounterForm
        {
            entity.SetField("automaticSpellLevel", value);
            return entity;
        }

        public static T SetCheckBaseDC<T>(this T entity, Int32 value)
            where T : CounterForm
        {
            entity.SetField("checkBaseDC", value);
            return entity;
        }

        public static T SetType<T>(this T entity, CounterForm.CounterType value)
            where T : CounterForm
        {
            entity.SetField("type", value);
            return entity;
        }
    }
}
