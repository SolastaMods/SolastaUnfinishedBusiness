using System;

namespace SolastaCommunityExpansion.Builders;

public class RecipeDefinitionBuilder : DefinitionBuilder<RecipeDefinition, RecipeDefinitionBuilder>
{
    public RecipeDefinitionBuilder SetCraftedItem(ItemDefinition craftedItem)
    {
        Definition.craftedItem = craftedItem;
        return this;
    }

    public RecipeDefinitionBuilder SetCraftedItem(ItemDefinition craftedItem, int stackCount)
    {
        Definition.craftedItem = craftedItem;
        Definition.stackCount = stackCount;
        return this;
    }

    public RecipeDefinitionBuilder SetCraftingCheckData(int craftingHours, int craftingDC,
        ToolTypeDefinition toolType)
    {
        Definition.craftingHours = craftingHours;
        Definition.craftingDC = craftingDC;
        Definition.toolTypeDefinition = toolType;
        return this;
    }

    public RecipeDefinitionBuilder AddIngredient(IngredientOccurenceDescription ingredient)
    {
        Definition.Ingredients.Add(ingredient);
        return this;
    }

    public RecipeDefinitionBuilder AddIngredient(ItemDefinition ingredient)
    {
        var description = new IngredientOccurenceDescription {itemDefinition = ingredient, amount = 1};
        Definition.Ingredients.Add(description);
        return this;
    }

#if false
    public RecipeDefinitionBuilder AddIngredient(ItemDefinition ingredient, int amount)
    {
        var description = new IngredientOccurenceDescription {itemDefinition = ingredient, amount = amount};

        Definition.Ingredients.Add(description);
        return this;
    }
#endif

    public RecipeDefinitionBuilder AddIngredients(params ItemDefinition[] ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }

        return this;
    }

#if false
    public RecipeDefinitionBuilder SetSpellDefinition(SpellDefinition spellDefinition)
    {
        Definition.spellDefinition = spellDefinition;
        return this;
    }
#endif

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
