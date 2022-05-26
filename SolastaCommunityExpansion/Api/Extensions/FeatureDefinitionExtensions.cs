using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionExtensions
    {
        public static T SetUserFeature<T>(this T entity, System.Boolean value)
            where T : FeatureDefinition
        {
            entity.SetField("<UserFeature>k__BackingField", value);
            return entity;
        }
    }
}
