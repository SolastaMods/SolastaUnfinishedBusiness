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
    [TargetType(typeof(CharacterSizeDefinition))]
    public static partial class CharacterSizeDefinitionExtensions
    {
        public static T SetBestiaryScaleFactor<T>(this T entity, System.Single value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("bestiaryScaleFactor", value);
            return entity;
        }

        public static T SetCarryingSize<T>(this T entity, RuleDefinitions.CreatureSize value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("carryingSize", value);
            return entity;
        }

        public static T SetMaxExtent<T>(this T entity, TA.int3 value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("maxExtent", value);
            return entity;
        }

        public static T SetMinExtent<T>(this T entity, TA.int3 value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("minExtent", value);
            return entity;
        }

        public static T SetVisionHeightFactor<T>(this T entity, System.Single value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("visionHeightFactor", value);
            return entity;
        }

        public static T SetWieldingSize<T>(this T entity, RuleDefinitions.CreatureSize value)
            where T : CharacterSizeDefinition
        {
            entity.SetField("wieldingSize", value);
            return entity;
        }
    }
}