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
    [TargetType(typeof(FeatureDefinitionFightingStyleChoice)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionFightingStyleChoiceExtensions
    {
        public static T AddFightingStyles<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionFightingStyleChoice
        {
            AddFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFightingStyles<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionFightingStyleChoice
        {
            entity.FightingStyles.AddRange(value);
            return entity;
        }

        public static T ClearFightingStyles<T>(this T entity)
            where T : FeatureDefinitionFightingStyleChoice
        {
            entity.FightingStyles.Clear();
            return entity;
        }

        public static T SetFightingStyles<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionFightingStyleChoice
        {
            SetFightingStyles(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFightingStyles<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionFightingStyleChoice
        {
            entity.FightingStyles.SetRange(value);
            return entity;
        }
    }
}