using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(TopologyForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class TopologyFormExtensions
    {
        public static TopologyForm Copy(this TopologyForm entity)
        {
            var copy = new TopologyForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetChangeType<T>(this T entity, TopologyForm.Type value)
            where T : TopologyForm
        {
            entity.SetField("changeType", value);
            return entity;
        }

        public static T SetImpactsFlyingCharacters<T>(this T entity, System.Boolean value)
            where T : TopologyForm
        {
            entity.SetField("impactsFlyingCharacters", value);
            return entity;
        }
    }
}
