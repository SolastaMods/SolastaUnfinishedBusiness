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
    [TargetType(typeof(BehaviorModeDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class BehaviorModeDescriptionExtensions
    {
        public static T SetBehaviour<T>(this T entity, RandomBehaviour value)
            where T : BehaviorModeDescription
        {
            entity.SetField("behaviour", value);
            return entity;
        }

        public static T SetCondition<T>(this T entity, ConditionDefinition value)
            where T : BehaviorModeDescription
        {
            entity.SetField("condition", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, Int32 value)
            where T : BehaviorModeDescription
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}
