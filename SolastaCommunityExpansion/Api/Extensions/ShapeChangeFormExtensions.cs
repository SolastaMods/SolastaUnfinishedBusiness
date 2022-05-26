using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ShapeChangeForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class ShapeChangeFormExtensions
    {
        public static T AddShapeOptions<T>(this T entity, params ShapeOptionDescription[] value)
            where T : ShapeChangeForm
        {
            AddShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddShapeOptions<T>(this T entity, IEnumerable<ShapeOptionDescription> value)
            where T : ShapeChangeForm
        {
            entity.ShapeOptions.AddRange(value);
            return entity;
        }

        public static T ClearShapeOptions<T>(this T entity)
            where T : ShapeChangeForm
        {
            entity.ShapeOptions.Clear();
            return entity;
        }

        public static ShapeChangeForm Copy(this ShapeChangeForm entity)
        {
            var copy = new ShapeChangeForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetKeepMentalAbilityScores<T>(this T entity, Boolean value)
            where T : ShapeChangeForm
        {
            entity.SetField("keepMentalAbilityScores", value);
            return entity;
        }

        public static T SetShapeChangeType<T>(this T entity, ShapeChangeForm.Type value)
            where T : ShapeChangeForm
        {
            entity.SetField("shapeChangeType", value);
            return entity;
        }

        public static T SetShapeOptions<T>(this T entity, params ShapeOptionDescription[] value)
            where T : ShapeChangeForm
        {
            SetShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetShapeOptions<T>(this T entity, IEnumerable<ShapeOptionDescription> value)
            where T : ShapeChangeForm
        {
            entity.ShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetSpecialSubstituteCondition<T>(this T entity, ConditionDefinition value)
            where T : ShapeChangeForm
        {
            entity.SetField("specialSubstituteCondition", value);
            return entity;
        }
    }
}
