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
    [TargetType(typeof(TravelEventDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TravelEventDefinitionExtensions
    {
        public static T AddFailureLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            AddFailureLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFailureLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.FailureLogEntries.AddRange(value);
            return entity;
        }

        public static T AddIngredientGatheringProficiencies<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            AddIngredientGatheringProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddIngredientGatheringProficiencies<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.IngredientGatheringProficiencies.AddRange(value);
            return entity;
        }

        public static T AddOtherLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            AddOtherLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOtherLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.OtherLogEntries.AddRange(value);
            return entity;
        }

        public static T AddSuccessLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            AddSuccessLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSuccessLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.SuccessLogEntries.AddRange(value);
            return entity;
        }

        public static T ClearFailureLogEntries<T>(this T entity)
            where T : TravelEventDefinition
        {
            entity.FailureLogEntries.Clear();
            return entity;
        }

        public static T ClearIngredientGatheringProficiencies<T>(this T entity)
            where T : TravelEventDefinition
        {
            entity.IngredientGatheringProficiencies.Clear();
            return entity;
        }

        public static T ClearOtherLogEntries<T>(this T entity)
            where T : TravelEventDefinition
        {
            entity.OtherLogEntries.Clear();
            return entity;
        }

        public static T ClearSuccessLogEntries<T>(this T entity)
            where T : TravelEventDefinition
        {
            entity.SuccessLogEntries.Clear();
            return entity;
        }

        public static T SetCooldownValue<T>(this T entity, System.Int32 value)
            where T : TravelEventDefinition
        {
            entity.SetField("cooldownValue", value);
            return entity;
        }

        public static T SetDumpsAdventureLog<T>(this T entity, System.Boolean value)
            where T : TravelEventDefinition
        {
            entity.SetField("dumpsAdventureLog", value);
            return entity;
        }

        public static T SetEventPeriodicity<T>(this T entity, TravelDefinitions.EventPeriodicity value)
            where T : TravelEventDefinition
        {
            entity.SetField("eventPeriodicity", value);
            return entity;
        }

        public static T SetEventType<T>(this T entity, TravelDefinitions.EventType value)
            where T : TravelEventDefinition
        {
            entity.SetField("eventType", value);
            return entity;
        }

        public static T SetFailureLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            SetFailureLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFailureLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.FailureLogEntries.SetRange(value);
            return entity;
        }

        public static T SetIngredientGatheringProficiencies<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            SetIngredientGatheringProficiencies(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetIngredientGatheringProficiencies<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.IngredientGatheringProficiencies.SetRange(value);
            return entity;
        }

        public static T SetOtherLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            SetOtherLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOtherLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.OtherLogEntries.SetRange(value);
            return entity;
        }

        public static T SetRecordedInAdventureLog<T>(this T entity, System.Boolean value)
            where T : TravelEventDefinition
        {
            entity.SetField("recordedInAdventureLog", value);
            return entity;
        }

        public static T SetSuccessLogEntries<T>(this T entity,  params  System . String [ ]  value)
            where T : TravelEventDefinition
        {
            SetSuccessLogEntries(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSuccessLogEntries<T>(this T entity, IEnumerable<System.String> value)
            where T : TravelEventDefinition
        {
            entity.SuccessLogEntries.SetRange(value);
            return entity;
        }
    }
}