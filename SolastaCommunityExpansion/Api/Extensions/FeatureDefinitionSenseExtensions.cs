using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionSense)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionSenseExtensions
    {
        public static T SetRevealsHiddenObjects<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionSense
        {
            entity.SetField("revealsHiddenObjects", value);
            return entity;
        }

        public static T SetSenseRange<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionSense
        {
            entity.SetField("senseRange", value);
            return entity;
        }

        public static T SetSenseType<T>(this T entity, SenseMode.Type value)
            where T : FeatureDefinitionSense
        {
            entity.SetField("senseType", value);
            return entity;
        }

        public static T SetStealthBreakerRange<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionSense
        {
            entity.SetField("stealthBreakerRange", value);
            return entity;
        }
    }
}