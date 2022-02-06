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
    [TargetType(typeof(DialogCharacterAnimationDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DialogCharacterAnimationDescriptionExtensions
    {
        public static T SetAdditionalOffset<T>(this T entity, System.Single value)
            where T : DialogCharacterAnimationDescription
        {
            entity.AdditionalOffset = value;
            return entity;
        }

        public static T SetBodyAnimationType<T>(this T entity, AnimationDefinitions.SpeakType value)
            where T : DialogCharacterAnimationDescription
        {
            entity.BodyAnimationType = value;
            return entity;
        }

        public static T SetBodyAnimationVariation<T>(this T entity, System.Int32 value)
            where T : DialogCharacterAnimationDescription
        {
            entity.BodyAnimationVariation = value;
            return entity;
        }

        public static T SetCutAnimation<T>(this T entity, System.Boolean value)
            where T : DialogCharacterAnimationDescription
        {
            entity.CutAnimation = value;
            return entity;
        }

        public static T SetFacialExpression<T>(this T entity, AnimationDefinitions.FacialExpression value)
            where T : DialogCharacterAnimationDescription
        {
            entity.FacialExpression = value;
            return entity;
        }

        public static T SetLookAtTarget<T>(this T entity, System.String value)
            where T : DialogCharacterAnimationDescription
        {
            entity.LookAtTarget = value;
            return entity;
        }

        public static T SetLookAtType<T>(this T entity, NarrativeDefinitions.LookAtType value)
            where T : DialogCharacterAnimationDescription
        {
            entity.LookAtType = value;
            return entity;
        }

        public static T SetLoopAnimation<T>(this T entity, System.Boolean value)
            where T : DialogCharacterAnimationDescription
        {
            entity.LoopAnimation = value;
            return entity;
        }

        public static T SetOverrideLookAt<T>(this T entity, System.Boolean value)
            where T : DialogCharacterAnimationDescription
        {
            entity.OverrideLookAt = value;
            return entity;
        }

        public static T SetRole<T>(this T entity, System.String value)
            where T : DialogCharacterAnimationDescription
        {
            entity.Role = value;
            return entity;
        }
    }
}