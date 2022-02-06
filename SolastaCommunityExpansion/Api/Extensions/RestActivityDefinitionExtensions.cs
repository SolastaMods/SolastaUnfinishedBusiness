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
    [TargetType(typeof(RestActivityDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RestActivityDefinitionExtensions
    {
        public static T SetCondition<T>(this T entity, RestActivityDefinition.ActivityCondition value)
            where T : RestActivityDefinition
        {
            entity.SetField("condition", value);
            return entity;
        }

        public static T SetFunctor<T>(this T entity, System.String value)
            where T : RestActivityDefinition
        {
            entity.SetField("functor", value);
            return entity;
        }

        public static T SetRestStage<T>(this T entity, RestDefinitions.RestStage value)
            where T : RestActivityDefinition
        {
            entity.SetField("restStage", value);
            return entity;
        }

        public static T SetRestType<T>(this T entity, RuleDefinitions.RestType value)
            where T : RestActivityDefinition
        {
            entity.SetField("restType", value);
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : RestActivityDefinition
        {
            entity.SetField("stringParameter", value);
            return entity;
        }
    }
}