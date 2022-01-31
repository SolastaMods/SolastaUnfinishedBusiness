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
    [TargetType(typeof(DisolveParameters))]
    public static partial class DisolveParametersExtensions
    {
        public static T SetColor<T>(this T entity, UnityEngine.Color value)
            where T : DisolveParameters
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetColorWidth<T>(this T entity, System.Single value)
            where T : DisolveParameters
        {
            entity.SetField("colorWidth", value);
            return entity;
        }

        public static T SetCurve<T>(this T entity, UnityEngine.AnimationCurve value)
            where T : DisolveParameters
        {
            entity.SetField("curve", value);
            return entity;
        }

        public static T SetDuration<T>(this T entity, System.Single value)
            where T : DisolveParameters
        {
            entity.SetField("duration", value);
            return entity;
        }

        public static T SetHueScale<T>(this T entity, System.Single value)
            where T : DisolveParameters
        {
            entity.SetField("hueScale", value);
            return entity;
        }

        public static T SetNoiseScale<T>(this T entity, System.Single value)
            where T : DisolveParameters
        {
            entity.SetField("noiseScale", value);
            return entity;
        }

        public static T SetStartAfterDeathAnimation<T>(this T entity, System.Boolean value)
            where T : DisolveParameters
        {
            entity.SetField("startAfterDeathAnimation", value);
            return entity;
        }

        public static T SetVertexOffset<T>(this T entity, System.Single value)
            where T : DisolveParameters
        {
            entity.SetField("vertexOffset", value);
            return entity;
        }
    }
}