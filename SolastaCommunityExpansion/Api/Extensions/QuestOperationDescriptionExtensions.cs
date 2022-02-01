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
    [TargetType(typeof(QuestOperationDescription))]
    public static partial class QuestOperationDescriptionExtensions
    {
        public static T SetOutcomeNumber<T>(this T entity, System.Int32 value)
            where T : QuestOperationDescription
        {
            entity.OutcomeNumber = value;
            return entity;
        }

        public static T SetQuestTreeName<T>(this T entity, System.String value)
            where T : QuestOperationDescription
        {
            entity.QuestTreeName = value;
            return entity;
        }

        public static T SetStepName<T>(this T entity, System.String value)
            where T : QuestOperationDescription
        {
            entity.StepName = value;
            return entity;
        }

        public static T SetType<T>(this T entity, System.String value)
            where T : QuestOperationDescription
        {
            entity.Type = value;
            return entity;
        }
    }
}