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
    [TargetType(typeof(GuiPresentation)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GuiPresentationExtensions
    {
        public static T SetColor<T>(this T entity, UnityEngine.Color value)
            where T : GuiPresentation
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.Description = value;
            return entity;
        }

        public static T SetHidden<T>(this T entity, System.Boolean value)
            where T : GuiPresentation
        {
            entity.SetField("hidden", value);
            return entity;
        }

        public static T SetSortOrder<T>(this T entity, System.Int32 value)
            where T : GuiPresentation
        {
            entity.SetField("sortOrder", value);
            return entity;
        }

        public static T SetSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : GuiPresentation
        {
            entity.SetField("spriteReference", value);
            return entity;
        }

        public static T SetSymbolChar<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.SetField("symbolChar", value);
            return entity;
        }

        public static T SetTitle<T>(this T entity, System.String value)
            where T : GuiPresentation
        {
            entity.Title = value;
            return entity;
        }

        public static T SetUnusedInSolastaCOTM<T>(this T entity, System.Boolean value)
            where T : GuiPresentation
        {
            entity.SetField("unusedInSolastaCOTM", value);
            return entity;
        }
    }
}