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
    [TargetType(typeof(ConditionOperationDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ConditionOperationDescriptionExtensions
    {
        public static T SetCanSaveToCancel<T>(this T entity, System.Boolean value)
            where T : ConditionOperationDescription
        {
            entity.SetField("canSaveToCancel", value);
            return entity;
        }

        public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
            where T : ConditionOperationDescription
        {
            entity.ConditionDefinition = value;
            return entity;
        }

        public static T SetConditionName<T>(this T entity, System.String value)
            where T : ConditionOperationDescription
        {
            entity.SetField("conditionName", value);
            return entity;
        }

        public static T SetHasSavingThrow<T>(this T entity, System.Boolean value)
            where T : ConditionOperationDescription
        {
            entity.SetField("hasSavingThrow", value);
            return entity;
        }

        public static T SetOperation<T>(this T entity, ConditionOperationDescription.ConditionOperation value)
            where T : ConditionOperationDescription
        {
            entity.Operation = value;
            return entity;
        }

        public static T SetSaveAffinity<T>(this T entity, RuleDefinitions.EffectSavingThrowType value)
            where T : ConditionOperationDescription
        {
            entity.SetField("saveAffinity", value);
            return entity;
        }

        public static T SetSaveOccurence<T>(this T entity, RuleDefinitions.TurnOccurenceType value)
            where T : ConditionOperationDescription
        {
            entity.SetField("saveOccurence", value);
            return entity;
        }
    }
}