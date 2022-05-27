using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(TreasureOption))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class TreasureOptionExtensions
    {
        public static T SetAmount<T>(this T entity, Int32 value)
            where T : TreasureOption
        {
            entity.SetField("amount", value);
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : TreasureOption
        {
            entity.SetField("itemDefinition", value);
            return entity;
        }

        public static T SetOdds<T>(this T entity, Int32 value)
            where T : TreasureOption
        {
            entity.SetField("odds", value);
            return entity;
        }
    }
}
