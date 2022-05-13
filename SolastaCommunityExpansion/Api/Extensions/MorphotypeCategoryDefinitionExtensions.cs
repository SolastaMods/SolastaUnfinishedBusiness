using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(MorphotypeCategoryDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MorphotypeCategoryDefinitionExtensions
    {
        public static T SetChoiceForm<T>(this T entity, MorphotypeCategoryDefinition.Form value)
            where T : MorphotypeCategoryDefinition
        {
            entity.SetField("choiceForm", value);
            return entity;
        }

        public static T SetDependency<T>(this T entity, MorphotypeElementDefinition.ElementCategory value)
            where T : MorphotypeCategoryDefinition
        {
            entity.SetField("dependency", value);
            return entity;
        }

        public static T SetDependencyNoneEntry<T>(this T entity, System.String value)
            where T : MorphotypeCategoryDefinition
        {
            entity.SetField("dependencyNoneEntry", value);
            return entity;
        }

        public static T SetHasDependency<T>(this T entity, System.Boolean value)
            where T : MorphotypeCategoryDefinition
        {
            entity.SetField("hasDependency", value);
            return entity;
        }
    }
}