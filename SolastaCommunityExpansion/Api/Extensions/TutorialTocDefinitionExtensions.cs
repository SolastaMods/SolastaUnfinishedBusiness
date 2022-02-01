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
    [TargetType(typeof(TutorialTocDefinition))]
    public static partial class TutorialTocDefinitionExtensions
    {
        public static T SetSectionLineHeight<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("sectionLineHeight", value);
            return entity;
        }

        public static T SetSubsectionHeaderHeight<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("subsectionHeaderHeight", value);
            return entity;
        }

        public static T SetSubsectionIndentWidth<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("subsectionIndentWidth", value);
            return entity;
        }

        public static T SetSubsectionLineHeight<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("subsectionLineHeight", value);
            return entity;
        }

        public static T SetSubsectionLineSpacing<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("subsectionLineSpacing", value);
            return entity;
        }

        public static T SetSubsectionTrailingHeight<T>(this T entity, System.Single value)
            where T : TutorialTocDefinition
        {
            entity.SetField("subsectionTrailingHeight", value);
            return entity;
        }
    }
}