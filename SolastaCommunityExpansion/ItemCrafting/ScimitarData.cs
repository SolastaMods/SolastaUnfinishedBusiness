using SolastaModApi;
using System;
using System.Collections.Generic;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting
{
    internal static class ScimitarData
    {
        private static ItemCollection items;

        internal static ItemCollection Items
        {
            get
            {
                if (items == null)
                {
                    items = new ItemCollection()
                    {
                        BaseGuid = new Guid("16757d1b-518f-4669-af43-1ddf5d23c223"),
                        BaseWeapons = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Scimitar,
                        },
                        PossiblePrimedItemsToReplace = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Primed_Longsword,
                            DatabaseHelper.ItemDefinitions.Primed_Greatsword,
                            DatabaseHelper.ItemDefinitions.Primed_Shortsword,
                            DatabaseHelper.ItemDefinitions.Primed_Dagger,
                        },
                        MagicToCopy = new List<MagicItemDataHolder>()
                        {
                            new MagicItemDataHolder("Stormblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Stormblade,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),
                            new MagicItemDataHolder("Frostburn", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Frostburn,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),
                            new MagicItemDataHolder("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                            new MagicItemDataHolder("Dragonblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Dragonblade,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),
                            new MagicItemDataHolder("Warden", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Warden,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordWarden),
                            new MagicItemDataHolder("Whiteburn", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                            new MagicItemDataHolder("Souldrinker", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Souldrinker,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                        }
                    };
                }
                return items;
            }
            set => items = value;
        }
    }
}
