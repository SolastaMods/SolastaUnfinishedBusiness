using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
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
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(DialogCameraMovementParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DialogCameraMovementParametersExtensions
    {
        public static T SetAccelerationDuration<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.AccelerationDuration = value;
            return entity;
        }

        public static T SetDollySpeed<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.DollySpeed = value;
            return entity;
        }

        public static T SetOrbitalSpeed<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.OrbitalSpeed = value;
            return entity;
        }

        public static T SetSpeedForward<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.SpeedForward = value;
            return entity;
        }

        public static T SetSpeedRight<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.SpeedRight = value;
            return entity;
        }

        public static T SetSpeedUp<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.SpeedUp = value;
            return entity;
        }

        public static T SetZoomDuration<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.ZoomDuration = value;
            return entity;
        }

        public static T SetZoomFrom<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.ZoomFrom = value;
            return entity;
        }

        public static T SetZoomTo<T>(this T entity, System.Single value)
            where T : DialogCameraMovementParameters
        {
            entity.ZoomTo = value;
            return entity;
        }
    }
}