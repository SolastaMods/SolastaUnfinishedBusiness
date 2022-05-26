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
    [TargetType(typeof(WeightedDecisionDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class WeightedDecisionDescriptionExtensions
    {
        public static T SetCooldown<T>(this T entity, System.Int32 value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("cooldown", value);
            return entity;
        }

        public static T SetDecision<T>(this T entity, DecisionDefinition value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("decision", value);
            return entity;
        }

        public static T SetDynamicCooldown<T>(this T entity, System.Boolean value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("dynamicCooldown", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, System.Single value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}
