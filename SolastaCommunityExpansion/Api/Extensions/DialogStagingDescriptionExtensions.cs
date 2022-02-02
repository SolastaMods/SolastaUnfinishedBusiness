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
    [TargetType(typeof(DialogStagingDescription))]
    public static partial class DialogStagingDescriptionExtensions
    {
        public static T SetActive<T>(this T entity, System.Boolean value)
            where T : DialogStagingDescription
        {
            entity.Active = value;
            return entity;
        }

        public static T SetAdditionalOffset<T>(this T entity, System.Single value)
            where T : DialogStagingDescription
        {
            entity.AdditionalOffset = value;
            return entity;
        }

        public static T SetCameraIndex<T>(this T entity, System.Int32 value)
            where T : DialogStagingDescription
        {
            entity.CameraIndex = value;
            return entity;
        }

        public static T SetCameraMovementMode<T>(this T entity, NarrativeDefinitions.CameraMovementMode value)
            where T : DialogStagingDescription
        {
            entity.CameraMovementMode = value;
            return entity;
        }

        public static T SetCameraShot<T>(this T entity, System.String value)
            where T : DialogStagingDescription
        {
            entity.CameraShot = value;
            return entity;
        }

        public static T SetCameraTarget<T>(this T entity, System.String value)
            where T : DialogStagingDescription
        {
            entity.CameraTarget = value;
            return entity;
        }

        public static T SetCameraTargetBone<T>(this T entity, AnimationDefinitions.BoneType value)
            where T : DialogStagingDescription
        {
            entity.CameraTargetBone = value;
            return entity;
        }

        public static T SetCameraTargetRole<T>(this T entity, System.String value)
            where T : DialogStagingDescription
        {
            entity.CameraTargetRole = value;
            return entity;
        }

        public static T SetDialogCameraMovementParameters<T>(this T entity, DialogCameraMovementParameters value)
            where T : DialogStagingDescription
        {
            entity.DialogCameraMovementParameters = value;
            return entity;
        }

        public static T SetFollow<T>(this T entity, System.Boolean value)
            where T : DialogStagingDescription
        {
            entity.Follow = value;
            return entity;
        }

        public static T SetLookAt<T>(this T entity, System.Boolean value)
            where T : DialogStagingDescription
        {
            entity.LookAt = value;
            return entity;
        }
    }
}