using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(HitAffinityByTag))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class HitAffinityByTagExtensions
    {
        public static HitAffinityByTag Copy(this HitAffinityByTag entity)
        {
            var copy = new HitAffinityByTag();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAdvantageType<T>(this T entity, AdvantageType value)
            where T : HitAffinityByTag
        {
            entity.SetField("advantageType", value);
            return entity;
        }

        public static T SetTag<T>(this T entity, String value)
            where T : HitAffinityByTag
        {
            entity.SetField("tag", value);
            return entity;
        }
    }
}
