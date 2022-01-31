using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
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
    [TargetType(typeof(DecisionDescription))]
    public static partial class DecisionDescriptionExtensions
    {
        public static T SetActivityType<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("activityType", value);
            return entity;
        }

        public static T SetBoolParameter<T>(this T entity, System.Boolean value)
            where T : DecisionDescription
        {
            entity.SetField("boolParameter", value);
            return entity;
        }

        public static T SetBoolSecParameter<T>(this T entity, System.Boolean value)
            where T : DecisionDescription
        {
            entity.SetField("boolSecParameter", value);
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("description", value);
            return entity;
        }

        public static T SetEnumParameter<T>(this T entity, System.Int32 value)
            where T : DecisionDescription
        {
            entity.SetField("enumParameter", value);
            return entity;
        }

        public static T SetFloatParameter<T>(this T entity, System.Single value)
            where T : DecisionDescription
        {
            entity.SetField("floatParameter", value);
            return entity;
        }

        public static T SetScorer<T>(this T entity, TA.AI.ActivityScorerDefinition value)
            where T : DecisionDescription
        {
            entity.SetField("scorer", value);
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("stringParameter", value);
            return entity;
        }

        public static T SetStringSecParameter<T>(this T entity, System.String value)
            where T : DecisionDescription
        {
            entity.SetField("stringSecParameter", value);
            return entity;
        }
    }
}