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
    [TargetType(typeof(EntityDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EntityDescriptionExtensions
    {
        public static T SetAction<T>(this T entity, EntityDescription.DescriptionAction value)
            where T : EntityDescription
        {
            entity.Action = value;
            return entity;
        }

        public static T SetCover<T>(this T entity, RuleDefinitions.CoverType value)
            where T : EntityDescription
        {
            entity.Cover = value;
            return entity;
        }

        public static T SetFailure<T>(this T entity, System.String value)
            where T : EntityDescription
        {
            entity.Failure = value;
            return entity;
        }

        public static T SetHeader<T>(this T entity, System.String value)
            where T : EntityDescription
        {
            entity.Header = value;
            return entity;
        }
    }
}