using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DiceByRank))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class DiceByRankExtensions
    {
        public static T SetDiceNumber<T>(this T entity, System.Int32 value)
            where T : DiceByRank
        {
            entity.SetField("diceNumber", value);
            return entity;
        }

        public static T SetRank<T>(this T entity, System.Int32 value)
            where T : DiceByRank
        {
            entity.SetField("rank", value);
            return entity;
        }
    }
}
