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
    [TargetType(typeof(VoiceDefinition))]
    public static partial class VoiceDefinitionExtensions
    {
        public static T SetAmplitude<T>(this T entity, System.Single value)
            where T : VoiceDefinition
        {
            entity.SetField("amplitude", value);
            return entity;
        }

        public static T SetAvailableInCharacterCreation<T>(this T entity, System.Boolean value)
            where T : VoiceDefinition
        {
            entity.SetField("availableInCharacterCreation", value);
            return entity;
        }

        public static T SetIndividualName<T>(this T entity, System.String value)
            where T : VoiceDefinition
        {
            entity.SetField("individualName", value);
            return entity;
        }

        public static T SetSex<T>(this T entity, RuleDefinitions.CreatureSex value)
            where T : VoiceDefinition
        {
            entity.SetField("sex", value);
            return entity;
        }

        public static T SetSpeechSpeed<T>(this T entity, System.Single value)
            where T : VoiceDefinition
        {
            entity.SetField("speechSpeed", value);
            return entity;
        }

        public static T SetWavePath<T>(this T entity, System.String value)
            where T : VoiceDefinition
        {
            entity.SetField("wavePath", value);
            return entity;
        }

        public static T SetWavePrefix<T>(this T entity, System.String value)
            where T : VoiceDefinition
        {
            entity.SetField("wavePrefix", value);
            return entity;
        }

        public static T SetWwiseSuffix<T>(this T entity, System.String value)
            where T : VoiceDefinition
        {
            entity.SetField("wwiseSuffix", value);
            return entity;
        }

        public static T SetWwiseSwitch<T>(this T entity, AK.Wwise.Switch value)
            where T : VoiceDefinition
        {
            entity.SetField("wwiseSwitch", value);
            return entity;
        }
    }
}