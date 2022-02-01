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
    [TargetType(typeof(TravelEventDefinition))]
    public static partial class TravelEventDefinitionExtensions
    {
        public static T SetCooldownValue<T>(this T entity, System.Int32 value)
            where T : TravelEventDefinition
        {
            entity.SetField("cooldownValue", value);
            return entity;
        }

        public static T SetDumpsAdventureLog<T>(this T entity, System.Boolean value)
            where T : TravelEventDefinition
        {
            entity.SetField("dumpsAdventureLog", value);
            return entity;
        }

        public static T SetEventPeriodicity<T>(this T entity, TravelDefinitions.EventPeriodicity value)
            where T : TravelEventDefinition
        {
            entity.SetField("eventPeriodicity", value);
            return entity;
        }

        public static T SetEventType<T>(this T entity, TravelDefinitions.EventType value)
            where T : TravelEventDefinition
        {
            entity.SetField("eventType", value);
            return entity;
        }

        public static T SetRecordedInAdventureLog<T>(this T entity, System.Boolean value)
            where T : TravelEventDefinition
        {
            entity.SetField("recordedInAdventureLog", value);
            return entity;
        }
    }
}