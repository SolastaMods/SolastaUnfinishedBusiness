using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    [TargetType(typeof(TreasureOption))]
    public static partial class TreasureOptionsExtension
    {
        public static T SetOdds<T>(this T entity, int value)
           where T : TreasureOption
        {
            entity.SetField("odds", value);
            return entity;
        }
        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : TreasureOption
        {
            entity.SetField("itemDefinition", value);
            return entity;
        }
        public static T SetAmount<T>(this T entity, int value)
            where T : TreasureOption
        {
            entity.SetField("amount", value);
            return entity;
        }
    }
}
