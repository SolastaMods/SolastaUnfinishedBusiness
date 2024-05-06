using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class BashingWeaponsData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
            [
                (ItemDefinitions.Club, null),
                (ItemDefinitions.Maul, ItemDefinitions.MaulPlus2),
                (ItemDefinitions.Warhammer, ItemDefinitions.WarhammerPlus2)
            ],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Acuteness", ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),

                new ItemCollection.MagicItemDataHolder("Power", ItemDefinitions.Enchanted_Morningstar_Of_Power,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarOfPower),

                new ItemCollection.MagicItemDataHolder("Lightbringer",
                    ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),

                new ItemCollection.MagicItemDataHolder("Punisher", ItemDefinitions.Enchanted_Battleaxe_Punisher,
                    RecipeDefinitions.Recipe_Enchantment_BattleaxePunisher),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),

                new ItemCollection.MagicItemDataHolder("Stormblade", ItemDefinitions.Enchanted_Longsword_Stormblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn)
            ]
        };
}
