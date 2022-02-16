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
    [TargetType(typeof(CameraModeManualParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CameraModeManualParametersExtensions
    {
        public static T SetBoundsSource<T>(this T entity, CameraController.CameraBoundsSource value)
            where T : CameraModeManualParameters
        {
            entity.SetField("boundsSource", value);
            return entity;
        }

        public static T SetCanRotate<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("canRotate", value);
            return entity;
        }

        public static T SetCanStrafe<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("canStrafe", value);
            return entity;
        }

        public static T SetCanZoom<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("canZoom", value);
            return entity;
        }

        public static T SetDebugOrbitCorrected<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("debugOrbitCorrected", value);
            return entity;
        }

        public static T SetDefaultFieldOfView<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("defaultFieldOfView", value);
            return entity;
        }

        public static T SetDefaultOrbitsRadii<T>(this T entity, System.Single[] value)
            where T : CameraModeManualParameters
        {
            entity.SetField("defaultOrbitsRadii", value);
            return entity;
        }

        public static T SetDepthOfFieldCloseUpProfile<T>(this T entity, UnityEngine.Rendering.PostProcessing.PostProcessProfile value)
            where T : CameraModeManualParameters
        {
            entity.SetField("depthOfFieldCloseUpProfile", value);
            return entity;
        }

        public static T SetDofOffset<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("dofOffset", value);
            return entity;
        }

        public static T SetElevationCorrectionDownwardDampingSpeed<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationCorrectionDownwardDampingSpeed", value);
            return entity;
        }

        public static T SetElevationCorrectionLowSpeedThreshold<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationCorrectionLowSpeedThreshold", value);
            return entity;
        }

        public static T SetElevationCorrectionSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationCorrectionSensitivity", value);
            return entity;
        }

        public static T SetElevationCorrectionUpwardDampingSpeed<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationCorrectionUpwardDampingSpeed", value);
            return entity;
        }

        public static T SetElevationPersistenceCorrectionSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationPersistenceCorrectionSensitivity", value);
            return entity;
        }

        public static T SetElevationPersistenceDistance<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationPersistenceDistance", value);
            return entity;
        }

        public static T SetElevationType<T>(this T entity, CameraModeManualParameters.CameraElevationType value)
            where T : CameraModeManualParameters
        {
            entity.SetField("elevationType", value);
            return entity;
        }

        public static T SetFixedZoom<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("fixedZoom", value);
            return entity;
        }

        public static T SetFocusDOF<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("focusDOF", value);
            return entity;
        }

        public static T SetHasElevationCorrection<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("hasElevationCorrection", value);
            return entity;
        }

        public static T SetHeldZoomOnly<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("heldZoomOnly", value);
            return entity;
        }

        public static T SetIsCampaignMapCamera<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("isCampaignMapCamera", value);
            return entity;
        }

        public static T SetKeyboardOrbitSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("keyboardOrbitSensitivity", value);
            return entity;
        }

        public static T SetKeyboardZoomSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("keyboardZoomSensitivity", value);
            return entity;
        }

        public static T SetLocationPanAnimation<T>(this T entity, UnityEngine.AnimationCurve value)
            where T : CameraModeManualParameters
        {
            entity.SetField("locationPanAnimation", value);
            return entity;
        }

        public static T SetLockedDuringTravel<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("lockedDuringTravel", value);
            return entity;
        }

        public static T SetMaxElevationRangeOffset<T>(this T entity, System.Int32 value)
            where T : CameraModeManualParameters
        {
            entity.SetField("maxElevationRangeOffset", value);
            return entity;
        }

        public static T SetMaxZoomLevelStrafeVelocityModifier<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("maxZoomLevelStrafeVelocityModifier", value);
            return entity;
        }

        public static T SetMinOrbitRadius<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("minOrbitRadius", value);
            return entity;
        }

        public static T SetMinZoomLevelStrafeVelocityModifier<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("minZoomLevelStrafeVelocityModifier", value);
            return entity;
        }

        public static T SetMouseOrbitSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("mouseOrbitSensitivity", value);
            return entity;
        }

        public static T SetOrbitChangeSpeed<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("orbitChangeSpeed", value);
            return entity;
        }

        public static T SetOrbitCorrectedByGrid<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("orbitCorrectedByGrid", value);
            return entity;
        }

        public static T SetOrbitCorrectionPadding<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("orbitCorrectionPadding", value);
            return entity;
        }

        public static T SetRefocusZoomBlend<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("refocusZoomBlend", value);
            return entity;
        }

        public static T SetScrollGridStep<T>(this T entity, System.Int32 value)
            where T : CameraModeManualParameters
        {
            entity.SetField("scrollGridStep", value);
            return entity;
        }

        public static T SetStrafeDragSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("strafeDragSensitivity", value);
            return entity;
        }

        public static T SetStrafeVelocity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("strafeVelocity", value);
            return entity;
        }

        public static T SetTargetDampingFactor<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("targetDampingFactor", value);
            return entity;
        }

        public static T SetTransitionFromLocationAnimation<T>(this T entity, UnityEngine.AnimationCurve value)
            where T : CameraModeManualParameters
        {
            entity.SetField("transitionFromLocationAnimation", value);
            return entity;
        }

        public static T SetTransitionToLocationAnimation<T>(this T entity, UnityEngine.AnimationCurve value)
            where T : CameraModeManualParameters
        {
            entity.SetField("transitionToLocationAnimation", value);
            return entity;
        }

        public static T SetUsePreciseStrafingBounds<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("usePreciseStrafingBounds", value);
            return entity;
        }

        public static T SetZoomLevelAffectsStrafeVelocity<T>(this T entity, System.Boolean value)
            where T : CameraModeManualParameters
        {
            entity.SetField("zoomLevelAffectsStrafeVelocity", value);
            return entity;
        }

        public static T SetZoomOutSensitivityMultiplier<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("zoomOutSensitivityMultiplier", value);
            return entity;
        }

        public static T SetZoomSensitivity<T>(this T entity, System.Single value)
            where T : CameraModeManualParameters
        {
            entity.SetField("zoomSensitivity", value);
            return entity;
        }

        public static T SetZoomType<T>(this T entity, CameraModeManualParameters.CameraZoomType value)
            where T : CameraModeManualParameters
        {
            entity.SetField("zoomType", value);
            return entity;
        }
    }
}