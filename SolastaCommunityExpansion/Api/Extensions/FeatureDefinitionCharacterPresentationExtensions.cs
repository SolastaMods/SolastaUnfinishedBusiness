using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionCharacterPresentation)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionCharacterPresentationExtensions
    {
        public static T SetForcedBeard<T>(this T entity, System.String value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("forcedBeard", value);
            return entity;
        }

        public static T SetKeepExistingBeardList<T>(this T entity, System.String[] value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("keepExistingBeardList", value);
            return entity;
        }

        public static T SetOccurencePercentage<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("occurencePercentage", value);
            return entity;
        }
    }
}