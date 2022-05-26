using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(LightSourceForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class LightSourceFormExtensions
    {
        public static LightSourceForm Copy(this LightSourceForm entity)
        {
            var copy = new LightSourceForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetApplyToSelf<T>(this T entity, Boolean value)
            where T : LightSourceForm
        {
            entity.SetField("applyToSelf", value);
            return entity;
        }

        public static T SetBrightRange<T>(this T entity, Int32 value)
            where T : LightSourceForm
        {
            entity.SetField("brightRange", value);
            return entity;
        }

        public static T SetColor<T>(this T entity, Color value)
            where T : LightSourceForm
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetDimAdditionalRange<T>(this T entity, Int32 value)
            where T : LightSourceForm
        {
            entity.SetField("dimAdditionalRange", value);
            return entity;
        }

        public static T SetGraphicsPrefabReference<T>(this T entity, AssetReference value)
            where T : LightSourceForm
        {
            entity.SetField("graphicsPrefabReference", value);
            return entity;
        }

        public static T SetLightSourceType<T>(this T entity, LightSourceType value)
            where T : LightSourceForm
        {
            entity.SetField("lightSourceType", value);
            return entity;
        }
    }
}
