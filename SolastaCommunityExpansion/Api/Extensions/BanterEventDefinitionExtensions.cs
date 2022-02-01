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
    [TargetType(typeof(BanterEventDefinition))]
    public static partial class BanterEventDefinitionExtensions
    {
        public static T SetCanUseWhileCautious<T>(this T entity, System.Boolean value)
            where T : BanterEventDefinition
        {
            entity.SetField("canUseWhileCautious", value);
            return entity;
        }

        public static T SetEventProbability<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("eventProbability", value);
            return entity;
        }

        public static T SetEventTrigger<T>(this T entity, System.String value)
            where T : BanterEventDefinition
        {
            entity.SetField("eventTrigger", value);
            return entity;
        }

        public static T SetPlaybackDelay<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("playbackDelay", value);
            return entity;
        }

        public static T SetSearchKey<T>(this T entity, System.String value)
            where T : BanterEventDefinition
        {
            entity.SetField("searchKey", value);
            return entity;
        }

        public static T SetSelfProbability<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("selfProbability", value);
            return entity;
        }
    }
}