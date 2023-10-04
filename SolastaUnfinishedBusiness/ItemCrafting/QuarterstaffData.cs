using System.Collections.Generic;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class QuarterstaffData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                new List<(ItemDefinition item, ItemDefinition presentation)>
                {
                    (ItemDefinitions.Quarterstaff, ItemDefinitions.QuarterstaffPlus2)
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                ItemDefinitions.Primed_Mace,
                ItemDefinitions.Primed_Longsword,
                ItemDefinitions.Primed_Greatsword,
                ItemDefinitions.Primed_Shortsword,
                ItemDefinitions.Primed_Dagger
            },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                // Same as +1
                new("Acuteness", ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),
                new("Stormblade", ItemDefinitions.Enchanted_Longsword_Stormblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),
                new("Frostburn", ItemDefinitions.Enchanted_Longsword_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),
                new("Lightbringer", ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                new("Dragonblade", ItemDefinitions.Enchanted_Longsword_Dragonblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),
                new("Warden", ItemDefinitions.Enchanted_Longsword_Warden,
                    RecipeDefinitions.Recipe_Enchantment_LongswordWarden),
                new("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                new("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                new("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw)
            }
        };
}
