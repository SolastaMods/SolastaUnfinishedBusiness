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
    [TargetType(typeof(RulesetLightSource)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetLightSourceExtensions
    {
        public static T SetAssetGUID<T>(this T entity, System.String value)
            where T : RulesetLightSource
        {
            entity.SetField("assetGUID", value);
            return entity;
        }

        public static T SetBrightRange<T>(this T entity, System.Single value)
            where T : RulesetLightSource
        {
            entity.SetField("brightRange", value);
            return entity;
        }

        public static T SetColor<T>(this T entity, UnityEngine.Color value)
            where T : RulesetLightSource
        {
            entity.SetField("color", value);
            return entity;
        }

        public static T SetDayCycleType<T>(this T entity, RuleDefinitions.LightSourceDayCycleType value)
            where T : RulesetLightSource
        {
            entity.SetField("dayCycleType", value);
            return entity;
        }

        public static T SetDimRange<T>(this T entity, System.Single value)
            where T : RulesetLightSource
        {
            entity.SetField("dimRange", value);
            return entity;
        }

        public static T SetIsDayCycleActive<T>(this T entity, System.Boolean value)
            where T : RulesetLightSource
        {
            entity.IsDayCycleActive = value;
            return entity;
        }

        public static T SetIsObscured<T>(this T entity, System.Boolean value)
            where T : RulesetLightSource
        {
            entity.IsObscured = value;
            return entity;
        }

        public static T SetIsSpot<T>(this T entity, System.Boolean value)
            where T : RulesetLightSource
        {
            entity.SetField("isSpot", value);
            return entity;
        }

        public static T SetLightSourceDayCycleActiveChanged<T>(this T entity, RulesetLightSource.LightSourceDayCycleActiveChangedHandler value)
            where T : RulesetLightSource
        {
            entity.SetField("<LightSourceDayCycleActiveChanged>k__BackingField", value);
            return entity;
        }

        public static T SetLightSourceExtinguished<T>(this T entity, RulesetLightSource.LightSourceExtinguishedHandler value)
            where T : RulesetLightSource
        {
            entity.SetField("<LightSourceExtinguished>k__BackingField", value);
            return entity;
        }

        public static T SetLightSourceObscurationChanged<T>(this T entity, RulesetLightSource.LightSourceObscurationChangedHandler value)
            where T : RulesetLightSource
        {
            entity.SetField("<LightSourceObscurationChanged>k__BackingField", value);
            return entity;
        }

        public static T SetLightSourceType<T>(this T entity, RuleDefinitions.LightSourceType value)
            where T : RulesetLightSource
        {
            entity.SetField("lightSourceType", value);
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : RulesetLightSource
        {
            entity.Name = value;
            return entity;
        }

        public static T SetSourceName<T>(this T entity, System.String value)
            where T : RulesetLightSource
        {
            entity.SetField("sourceName", value);
            return entity;
        }

        public static T SetSpecificLocationPosition<T>(this T entity, TA.int3 value)
            where T : RulesetLightSource
        {
            entity.SetField("specificLocationPosition", value);
            return entity;
        }

        public static T SetSpotAngle<T>(this T entity, System.Single value)
            where T : RulesetLightSource
        {
            entity.SetField("spotAngle", value);
            return entity;
        }

        public static T SetSpotDirection<T>(this T entity, UnityEngine.Vector3 value)
            where T : RulesetLightSource
        {
            entity.SetField("spotDirection", value);
            return entity;
        }

        public static T SetTargetGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetLightSource
        {
            entity.SetField("targetGuid", value);
            return entity;
        }

        public static T SetTargetItemGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetLightSource
        {
            entity.SetField("targetItemGuid", value);
            return entity;
        }

        public static T SetUseSpecificLocationPosition<T>(this T entity, System.Boolean value)
            where T : RulesetLightSource
        {
            entity.SetField("useSpecificLocationPosition", value);
            return entity;
        }
    }
}