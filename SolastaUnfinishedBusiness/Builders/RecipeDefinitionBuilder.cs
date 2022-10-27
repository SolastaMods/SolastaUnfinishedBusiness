using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class RecipeDefinitionBuilder : DefinitionBuilder<RecipeDefinition, RecipeDefinitionBuilder>
{
    internal RecipeDefinitionBuilder SetCraftedItem(ItemDefinition craftedItem, int stackCount = 1)
    {
        Definition.craftedItem = craftedItem;
        Definition.stackCount = stackCount;
        return this;
    }

    internal RecipeDefinitionBuilder SetCraftingCheckData(
        int craftingHours,
        int craftingDc,
        ToolTypeDefinition toolType)
    {
        Definition.craftingHours = craftingHours;
        Definition.craftingDC = craftingDc;
        Definition.toolTypeDefinition = toolType;
        return this;
    }

    internal RecipeDefinitionBuilder AddIngredients(params ItemDefinition[] ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            Definition.Ingredients.Add(new IngredientOccurenceDescription { itemDefinition = ingredient, amount = 1 });
        }

        return this;
    }

    #region Constructors

    protected RecipeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected RecipeDefinitionBuilder(RecipeDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
