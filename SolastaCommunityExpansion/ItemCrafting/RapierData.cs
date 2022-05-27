using System;
using System.Collections.Generic;
using SolastaModApi;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting
{
    internal static class RapierData
    {
        private static ItemCollection items;

        internal static ItemCollection Items
        {
            get => items ??= new ItemCollection
            {
                BaseGuid = new Guid("16757d1b-518f-4669-af43-1ddf5d23c223"),
                BaseWeapons = new List<ItemDefinition> {DatabaseHelper.ItemDefinitions.Rapier},
                PossiblePrimedItemsToReplace = new List<ItemDefinition>
                {
                    DatabaseHelper.ItemDefinitions.Primed_Longsword,
                    DatabaseHelper.ItemDefinitions.Primed_Greatsword,
                    DatabaseHelper.ItemDefinitions.Primed_Shortsword,
                    DatabaseHelper.ItemDefinitions.Primed_Dagger,
                    DatabaseHelper.ItemDefinitions.Primed_Morningstar
                },
                MagicToCopy = new List<MagicItemDataHolder>
                {
                    new("Stormblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Stormblade,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),
                    new("Frostburn", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Frostburn,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),
                    new("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                    new("Dragonblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Dragonblade,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),
                    new("Warden", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Warden,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordWarden),
                    new("Whiteburn", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                    new("Souldrinker", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Souldrinker,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                    new("Power", DatabaseHelper.ItemDefinitions.Enchanted_Morningstar_Of_Power,
                        DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MorningstarOfPower)
                }
            };
            set => items = value;
        }
    }
}
