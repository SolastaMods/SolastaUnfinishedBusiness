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
    [TargetType(typeof(LightSourceItemDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class LightSourceItemDescriptionExtensions
    {
        public static LightSourceItemDescription Copy(this LightSourceItemDescription entity)
        {
            return new LightSourceItemDescription(entity);
        }

        public static T SetActiveSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : LightSourceItemDescription
        {
            entity.SetField("activeSpriteReference", value);
            return entity;
        }

        public static T SetBrightRange<T>(this T entity, System.Int32 value)
            where T : LightSourceItemDescription
        {
            entity.SetField("brightRange", value);
            return entity;
        }

        public static T SetColor<T>(this T entity, UnityEngine.Color value)
            where T : LightSourceItemDescription
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetDimAdditionalRange<T>(this T entity, System.Int32 value)
            where T : LightSourceItemDescription
        {
            entity.SetField("dimAdditionalRange", value);
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : LightSourceItemDescription
        {
            entity.SetField("durationType", value);
            return entity;
        }

        public static T SetDurationValue<T>(this T entity, System.Int32 value)
            where T : LightSourceItemDescription
        {
            entity.SetField("durationValue", value);
            return entity;
        }

        public static T SetLightSourceType<T>(this T entity, RuleDefinitions.LightSourceType value)
            where T : LightSourceItemDescription
        {
            entity.SetField("lightSourceType", value);
            return entity;
        }
    }
}