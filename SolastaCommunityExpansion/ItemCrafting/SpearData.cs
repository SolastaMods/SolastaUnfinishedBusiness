using System;
using System.Collections.Generic;
using SolastaModApi;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting
{
    internal static class SpearData
    {
        private static ItemCollection items;

        internal static ItemCollection Items
        {
            get => items ??= new ItemCollection()
            {
                BaseGuid = new Guid("16757d1b-518f-4669-af43-1ddf5d23c223"),
                BaseWeapons = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Spear,
                        },
                PossiblePrimedItemsToReplace = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Primed_Rapier,
                            DatabaseHelper.ItemDefinitions.Primed_Shortsword,
                        },
                MagicToCopy = new List<MagicItemDataHolder>()
                        {
                            new MagicItemDataHolder("BlackViper", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Blackadder,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierBlackAdder),
                            new MagicItemDataHolder("Doomblade", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Doomblade,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierDoomblade),
                            new MagicItemDataHolder("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Of_Acuteness,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierOfAcuteness),
                            new MagicItemDataHolder("Whiteburn", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                            new MagicItemDataHolder("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Lightbringer,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordLightbringer),
                            new MagicItemDataHolder("Sharpness", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_of_Sharpness,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortwordOfSharpness),
                        }
            };
            set => items = value;
        }
    }
}
