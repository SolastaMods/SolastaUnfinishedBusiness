using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class RecipeDefinitionBuilder : DefinitionBuilder<RecipeDefinition, RecipeDefinitionBuilder>
{
    internal RecipeDefinitionBuilder SetCraftedItem(ItemDefinition craftedItem)
    {
        Definition.craftedItem = craftedItem;
        return this;
    }

    internal RecipeDefinitionBuilder SetCraftedItem(ItemDefinition craftedItem, int stackCount)
    {
        Definition.craftedItem = craftedItem;
        Definition.stackCount = stackCount;
        return this;
    }

    internal RecipeDefinitionBuilder SetCraftingCheckData(int craftingHours, int craftingDC,
        ToolTypeDefinition toolType)
    {
        Definition.craftingHours = craftingHours;
        Definition.craftingDC = craftingDC;
        Definition.toolTypeDefinition = toolType;
        return this;
    }

    internal RecipeDefinitionBuilder AddIngredient(IngredientOccurenceDescription ingredient)
    {
        Definition.Ingredients.Add(ingredient);
        return this;
    }

    internal RecipeDefinitionBuilder AddIngredient(ItemDefinition ingredient)
    {
        var description = new IngredientOccurenceDescription { itemDefinition = ingredient, amount = 1 };
        Definition.Ingredients.Add(description);
        return this;
    }

    internal RecipeDefinitionBuilder AddIngredients(params ItemDefinition[] ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }

        return this;
    }

    #region Constructors

    protected RecipeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected RecipeDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected RecipeDefinitionBuilder(RecipeDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected RecipeDefinitionBuilder(RecipeDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
