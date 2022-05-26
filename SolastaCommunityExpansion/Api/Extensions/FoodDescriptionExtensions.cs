using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FoodDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FoodDescriptionExtensions
    {
        public static FoodDescription Copy(this FoodDescription entity)
        {
            return new FoodDescription(entity);
        }

        public static T SetNutritiveCapacity<T>(this T entity, Int32 value)
            where T : FoodDescription
        {
            entity.SetField("nutritiveCapacity", value);
            return entity;
        }

        public static T SetPerishable<T>(this T entity, Boolean value)
            where T : FoodDescription
        {
            entity.SetField("perishable", value);
            return entity;
        }
    }
}
