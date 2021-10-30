using SolastaModApi;
using System;
using System.Collections.Generic;
using static SolastaContentExpansion.ItemCrafting.ItemCollection;

namespace SolastaContentExpansion.ItemCrafting
{
    class BashingWeaponsData
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
                DatabaseHelper.ItemDefinitions.Club,
                DatabaseHelper.ItemDefinitions.Maul,
                DatabaseHelper.ItemDefinitions.Warhammer,
            },
                        PossiblePrimedItemsToReplace = new List<ItemDefinition>()
            {
                DatabaseHelper.ItemDefinitions.Primed_Morningstar,
                DatabaseHelper.ItemDefinitions.Primed_Mace,
                DatabaseHelper.ItemDefinitions.Primed_Greatsword,
                DatabaseHelper.ItemDefinitions.Primed_Battleaxe,
            },
                        MagicToCopy = new List<MagicItemDataHolder>()
            {
                 // Same as +1
                new MagicItemDataHolder("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),
                new MagicItemDataHolder("Bearclaw", DatabaseHelper.ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),
                new MagicItemDataHolder("Power", DatabaseHelper.ItemDefinitions.Enchanted_Morningstar_Of_Power,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MorningstarOfPower),
                new MagicItemDataHolder("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                new MagicItemDataHolder("Punisher", DatabaseHelper.ItemDefinitions.Enchanted_Battleaxe_Punisher,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_BattleaxePunisher),
            }
                    };
                }
                return items;
            }
            set => items = value;
        }
    }
}
