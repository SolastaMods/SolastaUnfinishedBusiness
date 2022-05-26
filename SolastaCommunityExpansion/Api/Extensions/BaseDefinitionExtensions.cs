using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(BaseDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class BaseDefinitionExtensions
    {
        public static T SetCachedName<T>(this T entity, System.String value)
            where T : BaseDefinition
        {
            entity.SetField("cachedName", value);
            return entity;
        }

        public static T SetContentCopyright<T>(this T entity, BaseDefinition.Copyright value)
            where T : BaseDefinition
        {
            entity.SetField("contentCopyright", value);
            return entity;
        }

        public static T SetContentPack<T>(this T entity, GamingPlatformDefinitions.ContentPack value)
            where T : BaseDefinition
        {
            entity.SetField("contentPack", value);
            return entity;
        }

        public static T SetGuid<T>(this T entity, System.String value)
            where T : BaseDefinition
        {
            entity.SetField("guid", value);
            return entity;
        }

        public static T SetGuiPresentation<T>(this T entity, GuiPresentation value)
            where T : BaseDefinition
        {
            entity.GuiPresentation = value;
            return entity;
        }
    }
}