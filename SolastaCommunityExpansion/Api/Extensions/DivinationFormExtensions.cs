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
    [TargetType(typeof(DivinationForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class DivinationFormExtensions
    {
        public static T AddCreatureFamilies<T>(this T entity, params CharacterFamilyDefinition[] value)
            where T : DivinationForm
        {
            AddCreatureFamilies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddCreatureFamilies<T>(this T entity, IEnumerable<CharacterFamilyDefinition> value)
            where T : DivinationForm
        {
            entity.CreatureFamilies.AddRange(value);
            return entity;
        }

        public static T AddRevealedTags<T>(this T entity, params String[] value)
            where T : DivinationForm
        {
            AddRevealedTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRevealedTags<T>(this T entity, IEnumerable<String> value)
            where T : DivinationForm
        {
            entity.RevealedTags.AddRange(value);
            return entity;
        }

        public static T ClearCreatureFamilies<T>(this T entity)
            where T : DivinationForm
        {
            entity.CreatureFamilies.Clear();
            return entity;
        }

        public static T ClearRevealedTags<T>(this T entity)
            where T : DivinationForm
        {
            entity.RevealedTags.Clear();
            return entity;
        }

        public static DivinationForm Copy(this DivinationForm entity)
        {
            var copy = new DivinationForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetCreatureFamilies<T>(this T entity, params CharacterFamilyDefinition[] value)
            where T : DivinationForm
        {
            SetCreatureFamilies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetCreatureFamilies<T>(this T entity, IEnumerable<CharacterFamilyDefinition> value)
            where T : DivinationForm
        {
            entity.CreatureFamilies.SetRange(value);
            return entity;
        }

        public static T SetDivinationType<T>(this T entity, DivinationForm.Type value)
            where T : DivinationForm
        {
            entity.SetField("divinationType", value);
            return entity;
        }

        public static T SetRangeCells<T>(this T entity, Int32 value)
            where T : DivinationForm
        {
            entity.SetField("rangeCells", value);
            return entity;
        }

        public static T SetRevealedTags<T>(this T entity, params String[] value)
            where T : DivinationForm
        {
            SetRevealedTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRevealedTags<T>(this T entity, IEnumerable<String> value)
            where T : DivinationForm
        {
            entity.RevealedTags.SetRange(value);
            return entity;
        }
    }
}
