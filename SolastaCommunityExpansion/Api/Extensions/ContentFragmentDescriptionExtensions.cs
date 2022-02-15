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
    [TargetType(typeof(ContentFragmentDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ContentFragmentDescriptionExtensions
    {
        public static ContentFragmentDescription Copy(this ContentFragmentDescription entity)
        {
            return new ContentFragmentDescription(entity);
        }

        public static T SetSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : ContentFragmentDescription
        {
            entity.SpriteReference = value;
            return entity;
        }

        public static T SetText<T>(this T entity, System.String value)
            where T : ContentFragmentDescription
        {
            entity.Text = value;
            return entity;
        }

        public static T SetType<T>(this T entity, ContentFragmentDescription.FragmentType value)
            where T : ContentFragmentDescription
        {
            entity.Type = value;
            return entity;
        }
    }
}