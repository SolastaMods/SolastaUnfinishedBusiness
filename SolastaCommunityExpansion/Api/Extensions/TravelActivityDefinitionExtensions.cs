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
    [TargetType(typeof(TravelActivityDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TravelActivityDefinitionExtensions
    {
        public static T SetFatigueImpactPerHour<T>(this T entity, System.Single value)
            where T : TravelActivityDefinition
        {
            entity.SetField("fatigueImpactPerHour", value);
            return entity;
        }

        public static T SetIsDynamic<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("isDynamic", value);
            return entity;
        }

        public static T SetNavigationActivity<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("navigationActivity", value);
            return entity;
        }

        public static T SetRestActivity<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("restActivity", value);
            return entity;
        }

        public static T SetStandardDurationHours<T>(this T entity, System.Int32 value)
            where T : TravelActivityDefinition
        {
            entity.SetField("standardDurationHours", value);
            return entity;
        }

        public static T SetStandardStartHour<T>(this T entity, System.Int32 value)
            where T : TravelActivityDefinition
        {
            entity.SetField("standardStartHour", value);
            return entity;
        }
    }
}