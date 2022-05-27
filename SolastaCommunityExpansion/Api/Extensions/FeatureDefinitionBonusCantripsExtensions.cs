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
    [TargetType(typeof(FeatureDefinitionBonusCantrips))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionBonusCantripsExtensions
    {
        public static T AddBonusCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : FeatureDefinitionBonusCantrips
        {
            AddBonusCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddBonusCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : FeatureDefinitionBonusCantrips
        {
            entity.BonusCantrips.AddRange(value);
            return entity;
        }

        public static T ClearBonusCantrips<T>(this T entity)
            where T : FeatureDefinitionBonusCantrips
        {
            entity.BonusCantrips.Clear();
            return entity;
        }

        public static T SetBonusCantrips<T>(this T entity, params SpellDefinition[] value)
            where T : FeatureDefinitionBonusCantrips
        {
            SetBonusCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetBonusCantrips<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : FeatureDefinitionBonusCantrips
        {
            entity.BonusCantrips.SetRange(value);
            return entity;
        }
    }
}
