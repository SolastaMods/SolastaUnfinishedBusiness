using System;
using System.Collections.Generic;

namespace SolastaContentExpansion.ItemCrafting
{
    
    class ItemCollection
    {
        public struct MagicItemDataHolder
        {
            public string Name;
            public ItemDefinition Item;
            public RecipeDefinition Recipe;

            public MagicItemDataHolder(string name, ItemDefinition item, RecipeDefinition recipe)
            {
                this.Name = name;
                this.Item = item;
                this.Recipe = recipe;
            }
        }

        public Guid BaseGuid;
        public List<ItemDefinition> BaseWeapons;
        public List<MagicItemDataHolder> MagicToCopy;
        public List<ItemDefinition> PossiblePrimedItemsToReplace;
        public int NumProduced = 1;
    }
}
