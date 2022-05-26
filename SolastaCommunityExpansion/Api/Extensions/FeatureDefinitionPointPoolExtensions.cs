using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionPointPool))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionPointPoolExtensions
    {
        public static T AddRestrictedChoices<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionPointPool
        {
            AddRestrictedChoices(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRestrictedChoices<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionPointPool
        {
            entity.RestrictedChoices.AddRange(value);
            return entity;
        }

        public static T ClearRestrictedChoices<T>(this T entity)
            where T : FeatureDefinitionPointPool
        {
            entity.RestrictedChoices.Clear();
            return entity;
        }

        public static T SetPoolAmount<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionPointPool
        {
            entity.SetField("poolAmount", value);
            return entity;
        }

        public static T SetPoolType<T>(this T entity, HeroDefinitions.PointsPoolType value)
            where T : FeatureDefinitionPointPool
        {
            entity.SetField("poolType", value);
            return entity;
        }

        public static T SetRestrictedChoices<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionPointPool
        {
            SetRestrictedChoices(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRestrictedChoices<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionPointPool
        {
            entity.RestrictedChoices.SetRange(value);
            return entity;
        }

        public static T SetUniqueChoices<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPointPool
        {
            entity.SetField("uniqueChoices", value);
            return entity;
        }
    }
}
