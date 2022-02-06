using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DocumentDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DocumentDescriptionExtensions
    {
        public static T SetDestroyAfterReading<T>(this T entity, System.Boolean value)
            where T : DocumentDescription
        {
            entity.SetField("destroyAfterReading", value);
            return entity;
        }

        public static T SetFormat<T>(this T entity, DocumentDescription.FormatType value)
            where T : DocumentDescription
        {
            entity.SetField("format", value);
            return entity;
        }

        public static T SetLanguage<T>(this T entity, System.String value)
            where T : DocumentDescription
        {
            entity.SetField("language", value);
            return entity;
        }

        public static T SetLocationDefinition<T>(this T entity, LocationDefinition value)
            where T : DocumentDescription
        {
            entity.SetField("locationDefinition", value);
            return entity;
        }

        public static T SetLocationKnowledgeLevel<T>(this T entity, GameCampaignDefinitions.NodeKnowledge value)
            where T : DocumentDescription
        {
            entity.SetField("locationKnowledgeLevel", value);
            return entity;
        }

        public static T SetLoreType<T>(this T entity, RuleDefinitions.LoreType value)
            where T : DocumentDescription
        {
            entity.SetField("loreType", value);
            return entity;
        }

        public static T SetRecipeDefinition<T>(this T entity, RecipeDefinition value)
            where T : DocumentDescription
        {
            entity.SetField("recipeDefinition", value);
            return entity;
        }

        public static T SetScript<T>(this T entity, DocumentDescription.ScriptType value)
            where T : DocumentDescription
        {
            entity.SetField("script", value);
            return entity;
        }
    }
}