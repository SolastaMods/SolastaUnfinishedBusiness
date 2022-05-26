using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static BaseBlueprint;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(PrefabByEnvironmentDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class PrefabByEnvironmentDescriptionExtensions
    {
        public static T SetEnvironment<T>(this T entity, System.String value)
            where T : PrefabByEnvironmentDescription
        {
            entity.SetField("environment", value);
            return entity;
        }

        public static T SetPrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : PrefabByEnvironmentDescription
        {
            entity.SetField("prefabReference", value);
            return entity;
        }
    }
}