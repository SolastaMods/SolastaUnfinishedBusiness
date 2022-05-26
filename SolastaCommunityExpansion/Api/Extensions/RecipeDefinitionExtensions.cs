using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(RecipeDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class RecipeDefinitionExtensions
    {
        public static T AddIngredients<T>(this T entity, params IngredientOccurenceDescription[] value)
            where T : RecipeDefinition
        {
            AddIngredients(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddIngredients<T>(this T entity, IEnumerable<IngredientOccurenceDescription> value)
            where T : RecipeDefinition
        {
            entity.Ingredients.AddRange(value);
            return entity;
        }

        public static T ClearIngredients<T>(this T entity)
            where T : RecipeDefinition
        {
            entity.Ingredients.Clear();
            return entity;
        }

        public static T SetCraftedItem<T>(this T entity, ItemDefinition value)
            where T : RecipeDefinition
        {
            entity.SetField("craftedItem", value);
            return entity;
        }

        public static T SetCraftingDC<T>(this T entity, Int32 value)
            where T : RecipeDefinition
        {
            entity.SetField("craftingDC", value);
            return entity;
        }

        public static T SetCraftingHours<T>(this T entity, Int32 value)
            where T : RecipeDefinition
        {
            entity.SetField("craftingHours", value);
            return entity;
        }

        public static T SetIngredients<T>(this T entity, params IngredientOccurenceDescription[] value)
            where T : RecipeDefinition
        {
            SetIngredients(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetIngredients<T>(this T entity, IEnumerable<IngredientOccurenceDescription> value)
            where T : RecipeDefinition
        {
            entity.Ingredients.SetRange(value);
            return entity;
        }

        public static T SetSpellDefinition<T>(this T entity, SpellDefinition value)
            where T : RecipeDefinition
        {
            entity.SetField("spellDefinition", value);
            return entity;
        }

        public static T SetStackCount<T>(this T entity, Int32 value)
            where T : RecipeDefinition
        {
            entity.SetField("stackCount", value);
            return entity;
        }

        public static T SetToolTypeDefinition<T>(this T entity, ToolTypeDefinition value)
            where T : RecipeDefinition
        {
            entity.SetField("toolTypeDefinition", value);
            return entity;
        }
    }
}
