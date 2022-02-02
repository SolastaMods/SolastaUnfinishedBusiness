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
    [TargetType(typeof(LocationPresentation))]
    public static partial class LocationPresentationExtensions
    {
        public static T SetExploredDescription<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("exploredDescription", value);
            return entity;
        }

        public static T SetExploredTitle<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("exploredTitle", value);
            return entity;
        }

        public static T SetKnownDescription<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("knownDescription", value);
            return entity;
        }

        public static T SetKnownTitle<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("knownTitle", value);
            return entity;
        }

        public static T SetUnchartedDescription<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("unchartedDescription", value);
            return entity;
        }

        public static T SetUnchartedTitle<T>(this T entity, System.String value)
            where T : LocationPresentation
        {
            entity.SetField("unchartedTitle", value);
            return entity;
        }
    }
}