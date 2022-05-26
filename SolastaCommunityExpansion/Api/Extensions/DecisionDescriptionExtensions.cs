using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using TA.AI;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DecisionDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DecisionDescriptionExtensions
    {
        public static T SetActivityType<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("activityType", value);
            return entity;
        }

        public static T SetBoolParameter<T>(this T entity, System.Boolean value)
            where T : DecisionDescription
        {
            entity.SetField("boolParameter", value);
            return entity;
        }

        public static T SetBoolSecParameter<T>(this T entity, System.Boolean value)
            where T : DecisionDescription
        {
            entity.SetField("boolSecParameter", value);
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("description", value);
            return entity;
        }

        public static T SetEnumParameter<T>(this T entity, System.Int32 value)
            where T : DecisionDescription
        {
            entity.SetField("enumParameter", value);
            return entity;
        }

        public static T SetFloatParameter<T>(this T entity, System.Single value)
            where T : DecisionDescription
        {
            entity.SetField("floatParameter", value);
            return entity;
        }

        public static T SetScorer<T>(this T entity, TA.AI.ActivityScorerDefinition value)
            where T : DecisionDescription
        {
            entity.SetField("scorer", value);
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("stringParameter", value);
            return entity;
        }

        public static T SetStringSecParameter<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("stringSecParameter", value);
            return entity;
        }
    }
}