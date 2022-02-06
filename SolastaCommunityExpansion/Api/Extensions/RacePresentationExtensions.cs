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
    [TargetType(typeof(RacePresentation)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RacePresentationExtensions
    {
        public static T SetBeardBlendShapes<T>(this T entity, RacePresentation.BeardBlendShape[] value)
            where T : RacePresentation
        {
            entity.SetField("beardBlendShapes", value);
            return entity;
        }

        public static T SetBodyAssetPrefix<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("bodyAssetPrefix", value);
            return entity;
        }

        public static T SetCanModifyMusculature<T>(this T entity, System.Boolean value)
            where T : RacePresentation
        {
            entity.SetField("canModifyMusculature", value);
            return entity;
        }

        public static T SetEquipmentLayoutPath<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("equipmentLayoutPath", value);
            return entity;
        }

        public static T SetFemaleVoiceDefinition<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("femaleVoiceDefinition", value);
            return entity;
        }

        public static T SetHasSurName<T>(this T entity, System.Boolean value)
            where T : RacePresentation
        {
            entity.SetField("hasSurName", value);
            return entity;
        }

        public static T SetMaleVoiceDefinition<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("maleVoiceDefinition", value);
            return entity;
        }

        public static T SetMorphotypeAssetPrefix<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("morphotypeAssetPrefix", value);
            return entity;
        }

        public static T SetNeedBeard<T>(this T entity, System.Boolean value)
            where T : RacePresentation
        {
            entity.SetField("needBeard", value);
            return entity;
        }

        public static T SetPortraitShieldOffset<T>(this T entity, UnityEngine.Vector3 value)
            where T : RacePresentation
        {
            entity.SetField("portraitShieldOffset", value);
            return entity;
        }

        public static T SetPreferedHairColors<T>(this T entity, TA.RangedInt value)
            where T : RacePresentation
        {
            entity.SetField("preferedHairColors", value);
            return entity;
        }

        public static T SetPreferedSkinColors<T>(this T entity, TA.RangedInt value)
            where T : RacePresentation
        {
            entity.SetField("preferedSkinColors", value);
            return entity;
        }

        public static T SetSurNameTitle<T>(this T entity, System.String value)
            where T : RacePresentation
        {
            entity.SetField("surNameTitle", value);
            return entity;
        }

        public static T SetUseBeardBlendShape<T>(this T entity, System.Boolean value)
            where T : RacePresentation
        {
            entity.SetField("useBeardBlendShape", value);
            return entity;
        }
    }
}