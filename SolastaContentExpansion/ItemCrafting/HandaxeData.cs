using SolastaModApi;
using System;
using System.Collections.Generic;
using static SolastaContentExpansion.ItemCrafting.ItemCollection;

namespace SolastaContentExpansion.ItemCrafting
{
    class HandaxeData
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
                DatabaseHelper.ItemDefinitions.Handaxe,
            },
                        PossiblePrimedItemsToReplace = new List<ItemDefinition>()
            {
                DatabaseHelper.ItemDefinitions.Primed_Dagger,
            },
                        MagicToCopy = new List<MagicItemDataHolder>()
            {
                // Same as +1
                new MagicItemDataHolder("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerOfAcuteness),
                // Same as +2
                new MagicItemDataHolder("Sharpness", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_of_Sharpness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerOfSharpness),
                new MagicItemDataHolder("Souldrinker", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                new MagicItemDataHolder("Frostburn", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Frostburn,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),
            }
                    };
                }
                return items;
            }
            set => items = value;
        }
    }
}
