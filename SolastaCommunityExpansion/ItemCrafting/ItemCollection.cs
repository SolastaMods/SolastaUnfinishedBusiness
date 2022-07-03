using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.ItemCrafting;

internal sealed class ItemCollection
{
    public Guid BaseGuid;
    public List<ItemDefinition> BaseWeapons;
    public List<MagicItemDataHolder> MagicToCopy;
    public int NumProduced = 1;
    public List<ItemDefinition> PossiblePrimedItemsToReplace;

    public struct MagicItemDataHolder
    {
        public readonly string Name;
        public readonly ItemDefinition Item;
        public readonly RecipeDefinition Recipe;

        public MagicItemDataHolder(string name, ItemDefinition item, RecipeDefinition recipe)
        {
            Name = name;
            Item = item;
            Recipe = recipe;
        }
    }
}
