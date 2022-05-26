using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionCharacterPresentation))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionCharacterPresentationExtensions
    {
        public static T SetForcedBeard<T>(this T entity, String value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("forcedBeard", value);
            return entity;
        }

        public static T SetKeepExistingBeardList<T>(this T entity, String[] value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("keepExistingBeardList", value);
            return entity;
        }

        public static T SetOccurencePercentage<T>(this T entity, Int32 value)
            where T : FeatureDefinitionCharacterPresentation
        {
            entity.SetField("occurencePercentage", value);
            return entity;
        }
    }
}
