using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(GuiPresentation))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class GuiPresentationExtensions
    {
        public static GuiPresentation Copy(this GuiPresentation entity)
        {
            return new GuiPresentation(entity);
        }

        public static T SetColor<T>(this T entity, UnityEngine.Color value)
            where T : GuiPresentation
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.Description = value;
            return entity;
        }

        public static T SetHidden<T>(this T entity, System.Boolean value)
            where T : GuiPresentation
        {
            entity.SetField("hidden", value);
            return entity;
        }

        public static T SetSortOrder<T>(this T entity, System.Int32 value)
            where T : GuiPresentation
        {
            entity.SetField("sortOrder", value);
            return entity;
        }

        public static T SetSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : GuiPresentation
        {
            entity.SetField("spriteReference", value);
            return entity;
        }

        public static T SetSymbolChar<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.SetField("symbolChar", value);
            return entity;
        }

        public static T SetTitle<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.Title = value;
            return entity;
        }

        public static T SetUnusedInSolastaCOTM<T>(this T entity, System.Boolean value)
            where T : GuiPresentation
        {
            entity.SetField("unusedInSolastaCOTM", value);
            return entity;
        }

        public static T SetUsedInValleyDLC<T>(this T entity, System.Boolean value)
            where T : GuiPresentation
        {
            entity.SetField("usedInValleyDLC", value);
            return entity;
        }
    }
}
