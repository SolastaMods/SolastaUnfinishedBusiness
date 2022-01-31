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
    [TargetType(typeof(UserEffectDescription))]
    public static partial class UserEffectDescriptionExtensions
    {
        public static T SetDurationParameter<T>(this T entity, System.Int32 value)
            where T : UserEffectDescription
        {
            entity.DurationParameter = value;
            return entity;
        }

        public static T SetDurationType<T>(this T entity, UserEffectDescription.Duration value)
            where T : UserEffectDescription
        {
            entity.DurationType = value;
            return entity;
        }

        public static T SetEndOfEffect<T>(this T entity, UserEffectDescription.TurnOccurenceType value)
            where T : UserEffectDescription
        {
            entity.EndOfEffect = value;
            return entity;
        }

        public static T SetHasSavingThrow<T>(this T entity, System.Boolean value)
            where T : UserEffectDescription
        {
            entity.HasSavingThrow = value;
            return entity;
        }

        public static T SetSaveAbility<T>(this T entity, System.String value)
            where T : UserEffectDescription
        {
            entity.SaveAbility = value;
            return entity;
        }

        public static T SetSaveDC<T>(this T entity, System.Int32 value)
            where T : UserEffectDescription
        {
            entity.SaveDC = value;
            return entity;
        }
    }
}