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
    [TargetType(typeof(QuestValidatorDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class QuestValidatorDescriptionExtensions
    {
        public static T SetBoolParameter<T>(this T entity, System.Boolean value)
            where T : QuestValidatorDescription
        {
            entity.BoolParameter = value;
            return entity;
        }

        public static T SetFloatParameter<T>(this T entity, System.Single value)
            where T : QuestValidatorDescription
        {
            entity.FloatParameter = value;
            return entity;
        }

        public static T SetIntParameter<T>(this T entity, System.Int32 value)
            where T : QuestValidatorDescription
        {
            entity.IntParameter = value;
            return entity;
        }

        public static T SetIntParameter2<T>(this T entity, System.Int32 value)
            where T : QuestValidatorDescription
        {
            entity.IntParameter2 = value;
            return entity;
        }

        public static T SetIntParameter3<T>(this T entity, System.Int32 value)
            where T : QuestValidatorDescription
        {
            entity.IntParameter3 = value;
            return entity;
        }

        public static T SetQuestUpdateType<T>(this T entity, QuestDefinitions.QuestUpdateType value)
            where T : QuestValidatorDescription
        {
            entity.QuestUpdateType = value;
            return entity;
        }

        public static T SetStringParameter<T>(this T entity, System.String value)
            where T : QuestValidatorDescription
        {
            entity.StringParameter = value;
            return entity;
        }

        public static T SetStringParameter2<T>(this T entity, System.String value)
            where T : QuestValidatorDescription
        {
            entity.StringParameter2 = value;
            return entity;
        }

        public static T SetType<T>(this T entity, QuestDefinitions.QuestValidatorType value)
            where T : QuestValidatorDescription
        {
            entity.Type = value;
            return entity;
        }

        public static T SetVariableTest<T>(this T entity, VariableTestDescription value)
            where T : QuestValidatorDescription
        {
            entity.VariableTest = value;
            return entity;
        }

        public static T SetVariableType<T>(this T entity, GameVariableDefinitions.Type value)
            where T : QuestValidatorDescription
        {
            entity.VariableType = value;
            return entity;
        }
    }
}