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
    [TargetType(typeof(UserFunctorParametersDescription))]
    public static partial class UserFunctorParametersDescriptionExtensions
    {
        public static System.Collections.Generic.List<System.String> GetValidStepNames<T>(this T entity)
            where T : UserFunctorParametersDescription
        {
            return entity.GetField<System.Collections.Generic.List<System.String>>("validStepNames");
        }

        public static T SetBoolParameter<T>(this T entity, System.Boolean value)
            where T : UserFunctorParametersDescription
        {
            entity.BoolParameter = value;
            return entity;
        }

        public static T SetBoolParameter2<T>(this T entity, System.Boolean value)
            where T : UserFunctorParametersDescription
        {
            entity.BoolParameter2 = value;
            return entity;
        }

        public static T SetBoolParameter3<T>(this T entity, System.Boolean value)
            where T : UserFunctorParametersDescription
        {
            entity.BoolParameter3 = value;
            return entity;
        }

        public static T SetBoolParameter4<T>(this T entity, System.Boolean value)
            where T : UserFunctorParametersDescription
        {
            entity.BoolParameter4 = value;
            return entity;
        }

        public static T SetBoolParameter5<T>(this T entity, System.Boolean value)
            where T : UserFunctorParametersDescription
        {
            entity.BoolParameter5 = value;
            return entity;
        }

        public static T SetFloatParameter<T>(this T entity, System.Single value)
            where T : UserFunctorParametersDescription
        {
            entity.FloatParameter = value;
            return entity;
        }

        public static T SetIntParameter<T>(this T entity, System.Int32 value)
            where T : UserFunctorParametersDescription
        {
            entity.IntParameter = value;
            return entity;
        }

        public static T SetIntParameter2<T>(this T entity, System.Int32 value)
            where T : UserFunctorParametersDescription
        {
            entity.IntParameter2 = value;
            return entity;
        }

        public static T SetIntParameter3<T>(this T entity, System.Int32 value)
            where T : UserFunctorParametersDescription
        {
            entity.IntParameter3 = value;
            return entity;
        }

        public static T SetIntParameter4<T>(this T entity, System.Int32 value)
            where T : UserFunctorParametersDescription
        {
            entity.IntParameter4 = value;
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : UserFunctorParametersDescription
        {
            entity.StringParameter = value;
            return entity;
        }

        public static T SetStringParameter2<T>(this T entity, System.String value)
            where T : UserFunctorParametersDescription
        {
            entity.StringParameter2 = value;
            return entity;
        }

        public static T SetStringParameter3<T>(this T entity, System.String value)
            where T : UserFunctorParametersDescription
        {
            entity.StringParameter3 = value;
            return entity;
        }

        public static T SetType<T>(this T entity, System.String value)
            where T : UserFunctorParametersDescription
        {
            entity.Type = value;
            return entity;
        }
    }
}