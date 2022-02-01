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
    [TargetType(typeof(FeatureDefinitionAdditionalAction))]
    public static partial class FeatureDefinitionAdditionalActionExtensions
    {
        public static T SetActionType<T>(this T entity, ActionDefinitions.ActionType value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("actionType", value);
            return entity;
        }

        public static T SetMaxAttacksNumber<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("maxAttacksNumber", value);
            return entity;
        }

        public static T SetTriggerCondition<T>(this T entity, RuleDefinitions.AdditionalActionTriggerCondition value)
            where T : FeatureDefinitionAdditionalAction
        {
            entity.SetField("triggerCondition", value);
            return entity;
        }
    }
}