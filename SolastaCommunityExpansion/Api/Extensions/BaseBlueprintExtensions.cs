using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(BaseBlueprint))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class BaseBlueprintExtensions
    {
        public static T AddPrefabsByEnvironment<T>(this T entity,
            params BaseBlueprint.PrefabByEnvironmentDescription[] value)
            where T : BaseBlueprint
        {
            AddPrefabsByEnvironment(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPrefabsByEnvironment<T>(this T entity,
            IEnumerable<BaseBlueprint.PrefabByEnvironmentDescription> value)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.AddRange(value);
            return entity;
        }

        public static T ClearPrefabsByEnvironment<T>(this T entity)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.Clear();
            return entity;
        }

        public static T SetCategory<T>(this T entity, String value)
            where T : BaseBlueprint
        {
            entity.SetField("category", value);
            return entity;
        }

        public static T SetDimensions<T>(this T entity, Vector2Int value)
            where T : BaseBlueprint
        {
            entity.SetField("dimensions", value);
            return entity;
        }

        public static T SetPrefabsByEnvironment<T>(this T entity,
            params BaseBlueprint.PrefabByEnvironmentDescription[] value)
            where T : BaseBlueprint
        {
            SetPrefabsByEnvironment(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPrefabsByEnvironment<T>(this T entity,
            IEnumerable<BaseBlueprint.PrefabByEnvironmentDescription> value)
            where T : BaseBlueprint
        {
            entity.PrefabsByEnvironment.SetRange(value);
            return entity;
        }
    }
}
