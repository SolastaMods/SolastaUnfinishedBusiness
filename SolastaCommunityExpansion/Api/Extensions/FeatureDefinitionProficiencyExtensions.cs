using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionProficiency))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionProficiencyExtensions
    {
        public static T AddForbiddenItemTags<T>(this T entity, params String[] value)
            where T : FeatureDefinitionProficiency
        {
            AddForbiddenItemTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddForbiddenItemTags<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionProficiency
        {
            entity.ForbiddenItemTags.AddRange(value);
            return entity;
        }

        public static T AddProficiencies<T>(this T entity, params String[] value)
            where T : FeatureDefinitionProficiency
        {
            AddProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionProficiency
        {
            entity.Proficiencies.AddRange(value);
            return entity;
        }

        public static T ClearForbiddenItemTags<T>(this T entity)
            where T : FeatureDefinitionProficiency
        {
            entity.ForbiddenItemTags.Clear();
            return entity;
        }

        public static T ClearProficiencies<T>(this T entity)
            where T : FeatureDefinitionProficiency
        {
            entity.Proficiencies.Clear();
            return entity;
        }

        public static T SetForbiddenItemTags<T>(this T entity, params String[] value)
            where T : FeatureDefinitionProficiency
        {
            SetForbiddenItemTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetForbiddenItemTags<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionProficiency
        {
            entity.ForbiddenItemTags.SetRange(value);
            return entity;
        }

        public static T SetProficiencies<T>(this T entity, params String[] value)
            where T : FeatureDefinitionProficiency
        {
            SetProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetProficiencies<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionProficiency
        {
            entity.Proficiencies.SetRange(value);
            return entity;
        }

        public static T SetProficienciesFormat<T>(this T entity, StringBuilder value)
            where T : FeatureDefinitionProficiency
        {
            entity.SetField("proficienciesFormat", value);
            return entity;
        }

        public static T SetProficiencyType<T>(this T entity, ProficiencyType value)
            where T : FeatureDefinitionProficiency
        {
            entity.SetField("proficiencyType", value);
            return entity;
        }
    }
}
