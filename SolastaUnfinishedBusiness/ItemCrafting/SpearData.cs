using System.Collections.Generic;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class SpearData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems = new List<(ItemDefinition item, ItemDefinition presentation)>
            {
                (ItemDefinitions.Spear, ItemDefinitions.SpearPlus2)
            },
            PossiblePrimedItemsToReplace =
                new List<ItemDefinition> { ItemDefinitions.Primed_Rapier, ItemDefinitions.Primed_Shortsword },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("BlackViper", ItemDefinitions.Enchanted_Rapier_Blackadder,
                    RecipeDefinitions.Recipe_Enchantment_RapierBlackAdder),
                new("Doomblade", ItemDefinitions.Enchanted_Rapier_Doomblade,
                    RecipeDefinitions.Recipe_Enchantment_RapierDoomblade),
                new("Acuteness", ItemDefinitions.Enchanted_Rapier_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_RapierOfAcuteness),
                new("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                new("Lightbringer", ItemDefinitions.Enchanted_Shortsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordLightbringer),
                new("Sharpness", ItemDefinitions.Enchanted_Shortsword_of_Sharpness,
                    RecipeDefinitions.Recipe_Enchantment_ShortwordOfSharpness),
                new("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                new("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw)
            }
        };
}
