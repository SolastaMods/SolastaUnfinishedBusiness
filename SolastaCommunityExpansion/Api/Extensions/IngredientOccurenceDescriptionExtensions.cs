using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(IngredientOccurenceDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class IngredientOccurenceDescriptionExtensions
    {
        public static T SetAmount<T>(this T entity, System.Int32 value)
            where T : IngredientOccurenceDescription
        {
            entity.SetField("amount", value);
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : IngredientOccurenceDescription
        {
            entity.SetField("itemDefinition", value);
            return entity;
        }
    }
}