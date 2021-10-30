using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;


namespace SolastaContentExpansion.Features
{
    public class RecipeBuilder : BaseDefinitionBuilder<RecipeDefinition>
    {
        public RecipeBuilder(string name, string guid) : base(name, guid)
        {
            Definition.SetField("ingredients", new List<IngredientOccurenceDescription>());
        }

        public RecipeBuilder SetCraftedItem(ItemDefinition craftedItem)
        {
            Definition.SetCraftedItem(craftedItem);
            return this;
        }

        public RecipeBuilder SetCraftedItem(ItemDefinition craftedItem, int stackCount)
        {
            Definition.SetCraftedItem(craftedItem);
            Definition.SetStackCount(stackCount);
            return this;
        }

        public RecipeBuilder SetCraftingCheckData(int craftingHours, int craftingDC, ToolTypeDefinition toolType)
        {
            Definition.SetCraftingHours(craftingHours);
            Definition.SetCraftingDC(craftingDC);
            Definition.SetToolTypeDefinition(toolType);
            return this;
        }

        public RecipeBuilder AddIngredient(IngredientOccurenceDescription ingredient)
        {
            Definition.Ingredients.Add(ingredient);
            return this;
        }

        public RecipeBuilder AddIngredient(ItemDefinition ingredient)
        {
            IngredientOccurenceDescription description = new IngredientOccurenceDescription();
            description.SetItemDefinition(ingredient);
            description.SetAmount(1);
            Definition.Ingredients.Add(description);
            return this;
        }

        public RecipeBuilder AddIngredient(ItemDefinition ingredient, int amount)
        {
            IngredientOccurenceDescription description = new IngredientOccurenceDescription();
            description.SetItemDefinition(ingredient);
            description.SetAmount(amount);
            Definition.Ingredients.Add(description);
            return this;
        }

        public RecipeBuilder SetSpellDefinition(SpellDefinition spellDefinition)
        {
            Definition.SetSpellDefinition(spellDefinition);
            return this;
        }

        public RecipeBuilder SetGuiPresentation(GuiPresentation gui)
        {
            Definition.SetGuiPresentation(gui);
            return this;
        }
    }
}
