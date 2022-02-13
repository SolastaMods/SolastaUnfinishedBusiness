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
    [TargetType(typeof(ConsiderationDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ConsiderationDescriptionExtensions
    {
        public static T SetBoolParameter<T>(this T entity, System.Boolean value)
            where T : ConsiderationDescription
        {
            entity.SetField("boolParameter", value);
            return entity;
        }

        public static T SetBoolSecParameter<T>(this T entity, System.Boolean value)
            where T : ConsiderationDescription
        {
            entity.SetField("boolSecParameter", value);
            return entity;
        }

        public static T SetBoolTerParameter<T>(this T entity, System.Boolean value)
            where T : ConsiderationDescription
        {
            entity.SetField("boolTerParameter", value);
            return entity;
        }

        public static T SetByteParameter<T>(this T entity, System.Byte value)
            where T : ConsiderationDescription
        {
            entity.SetField("byteParameter", value);
            return entity;
        }

        public static T SetConsiderationType<T>(this T entity, System.String value)
            where T : ConsiderationDescription
        {
            entity.SetField("considerationType", value);
            return entity;
        }

        public static T SetCurve<T>(this T entity, UnityEngine.AnimationCurve value)
            where T : ConsiderationDescription
        {
            entity.SetField("curve", value);
            return entity;
        }

        public static T SetFloatParameter<T>(this T entity, System.Single value)
            where T : ConsiderationDescription
        {
            entity.SetField("floatParameter", value);
            return entity;
        }

        public static T SetIntParameter<T>(this T entity, System.Int32 value)
            where T : ConsiderationDescription
        {
            entity.SetField("intParameter", value);
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : ConsiderationDescription
        {
            entity.SetField("stringParameter", value);
            return entity;
        }
    }
}