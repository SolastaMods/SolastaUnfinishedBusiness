using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FocusItemDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FocusItemDescriptionExtensions
    {
        public static FocusItemDescription Copy(this FocusItemDescription entity)
        {
            return new FocusItemDescription(entity);
        }

        public static T SetFocusType<T>(this T entity, EquipmentDefinitions.FocusType value)
            where T : FocusItemDescription
        {
            entity.SetField("focusType", value);
            return entity;
        }

        public static T SetShownAsFocus<T>(this T entity, System.Boolean value)
            where T : FocusItemDescription
        {
            entity.SetField("shownAsFocus", value);
            return entity;
        }
    }
}