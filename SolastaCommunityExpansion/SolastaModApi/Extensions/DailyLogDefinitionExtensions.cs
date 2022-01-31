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
    [TargetType(typeof(DailyLogDefinition))]
    public static partial class DailyLogDefinitionExtensions
    {
        public static T SetHeaderHeight<T>(this T entity, System.Int32 value)
            where T : DailyLogDefinition
        {
            entity.SetField("headerHeight", value);
            return entity;
        }

        public static T SetSubItemHeight<T>(this T entity, System.Int32 value)
            where T : DailyLogDefinition
        {
            entity.SetField("subItemHeight", value);
            return entity;
        }

        public static T SetSubItemSpacing<T>(this T entity, System.Int32 value)
            where T : DailyLogDefinition
        {
            entity.SetField("subItemSpacing", value);
            return entity;
        }

        public static T SetTableBottomMargin<T>(this T entity, System.Int32 value)
            where T : DailyLogDefinition
        {
            entity.SetField("tableBottomMargin", value);
            return entity;
        }
    }
}