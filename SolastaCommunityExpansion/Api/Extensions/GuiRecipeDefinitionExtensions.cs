using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(GuiRecipeDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GuiRecipeDefinitionExtensions
    {
        public static T AddCompatibleProficiencies<T>(this T entity,  params  System . String [ ]  value)
            where T : GuiRecipeDefinition
        {
            AddCompatibleProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddCompatibleProficiencies<T>(this T entity, IEnumerable<System.String> value)
            where T : GuiRecipeDefinition
        {
            entity.CompatibleProficiencies.AddRange(value);
            return entity;
        }

        public static T AddIngredients<T>(this T entity,  params  IngredientOccurenceDescription [ ]  value)
            where T : GuiRecipeDefinition
        {
            AddIngredients(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddIngredients<T>(this T entity, IEnumerable<IngredientOccurenceDescription> value)
            where T : GuiRecipeDefinition
        {
            entity.Ingredients.AddRange(value);
            return entity;
        }

        public static T ClearCompatibleProficiencies<T>(this T entity)
            where T : GuiRecipeDefinition
        {
            entity.CompatibleProficiencies.Clear();
            return entity;
        }

        public static T ClearIngredients<T>(this T entity)
            where T : GuiRecipeDefinition
        {
            entity.Ingredients.Clear();
            return entity;
        }

        public static T SetCompatibleProficiencies<T>(this T entity,  params  System . String [ ]  value)
            where T : GuiRecipeDefinition
        {
            SetCompatibleProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetCompatibleProficiencies<T>(this T entity, IEnumerable<System.String> value)
            where T : GuiRecipeDefinition
        {
            entity.CompatibleProficiencies.SetRange(value);
            return entity;
        }

        public static T SetIngredients<T>(this T entity,  params  IngredientOccurenceDescription [ ]  value)
            where T : GuiRecipeDefinition
        {
            SetIngredients(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetIngredients<T>(this T entity, IEnumerable<IngredientOccurenceDescription> value)
            where T : GuiRecipeDefinition
        {
            entity.Ingredients.SetRange(value);
            return entity;
        }

        public static T SetRecipeDefinition<T>(this T entity, RecipeDefinition value)
            where T : GuiRecipeDefinition
        {
            entity.SetField("<RecipeDefinition>k__BackingField", value);
            return entity;
        }
    }
}