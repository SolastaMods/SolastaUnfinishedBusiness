using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemOccurence)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ItemOccurenceExtensions
    {
        public static ItemOccurence Copy(this ItemOccurence entity)
        {
            return new ItemOccurence(entity);
        }

        public static T SetAdditiveModifier<T>(this T entity, System.Int32 value)
            where T : ItemOccurence
        {
            entity.AdditiveModifier = value;
            return entity;
        }

        public static T SetDiceNumber<T>(this T entity, System.Int32 value)
            where T : ItemOccurence
        {
            entity.DiceNumber = value;
            return entity;
        }

        public static T SetDiceType<T>(this T entity, RuleDefinitions.DieType value)
            where T : ItemOccurence
        {
            entity.DiceType = value;
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : ItemOccurence
        {
            entity.ItemDefinition = value;
            return entity;
        }

        public static T SetItemMode<T>(this T entity, ItemOccurence.SelectionMode value)
            where T : ItemOccurence
        {
            entity.ItemMode = value;
            return entity;
        }

        public static T SetTreasureTableDefinition<T>(this T entity, TreasureTableDefinition value)
            where T : ItemOccurence
        {
            entity.SetField("treasureTableDefinition", value);
            return entity;
        }
    }
}