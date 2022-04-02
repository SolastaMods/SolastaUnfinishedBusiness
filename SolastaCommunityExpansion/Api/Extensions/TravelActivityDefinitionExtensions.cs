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
    [TargetType(typeof(TravelActivityDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TravelActivityDefinitionExtensions
    {
        public static T AddFixedEvents<T>(this T entity,  params  TravelEventFixedOccurenceDescription [ ]  value)
            where T : TravelActivityDefinition
        {
            AddFixedEvents(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFixedEvents<T>(this T entity, IEnumerable<TravelEventFixedOccurenceDescription> value)
            where T : TravelActivityDefinition
        {
            entity.FixedEvents.AddRange(value);
            return entity;
        }

        public static T AddRandomEvents<T>(this T entity,  params  TravelEventProbabilityDescription [ ]  value)
            where T : TravelActivityDefinition
        {
            AddRandomEvents(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRandomEvents<T>(this T entity, IEnumerable<TravelEventProbabilityDescription> value)
            where T : TravelActivityDefinition
        {
            entity.RandomEvents.AddRange(value);
            return entity;
        }

        public static T ClearFixedEvents<T>(this T entity)
            where T : TravelActivityDefinition
        {
            entity.FixedEvents.Clear();
            return entity;
        }

        public static T ClearRandomEvents<T>(this T entity)
            where T : TravelActivityDefinition
        {
            entity.RandomEvents.Clear();
            return entity;
        }

        public static T SetFatigueImpactPerHour<T>(this T entity, System.Single value)
            where T : TravelActivityDefinition
        {
            entity.SetField("fatigueImpactPerHour", value);
            return entity;
        }

        public static T SetFixedEvents<T>(this T entity,  params  TravelEventFixedOccurenceDescription [ ]  value)
            where T : TravelActivityDefinition
        {
            SetFixedEvents(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFixedEvents<T>(this T entity, IEnumerable<TravelEventFixedOccurenceDescription> value)
            where T : TravelActivityDefinition
        {
            entity.FixedEvents.SetRange(value);
            return entity;
        }

        public static T SetIsDynamic<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("isDynamic", value);
            return entity;
        }

        public static T SetNavigationActivity<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("navigationActivity", value);
            return entity;
        }

        public static T SetRandomEvents<T>(this T entity,  params  TravelEventProbabilityDescription [ ]  value)
            where T : TravelActivityDefinition
        {
            SetRandomEvents(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRandomEvents<T>(this T entity, IEnumerable<TravelEventProbabilityDescription> value)
            where T : TravelActivityDefinition
        {
            entity.RandomEvents.SetRange(value);
            return entity;
        }

        public static T SetRestActivity<T>(this T entity, System.Boolean value)
            where T : TravelActivityDefinition
        {
            entity.SetField("restActivity", value);
            return entity;
        }

        public static T SetStandardDurationHours<T>(this T entity, System.Int32 value)
            where T : TravelActivityDefinition
        {
            entity.SetField("standardDurationHours", value);
            return entity;
        }

        public static T SetStandardStartHour<T>(this T entity, System.Int32 value)
            where T : TravelActivityDefinition
        {
            entity.SetField("standardStartHour", value);
            return entity;
        }
    }
}