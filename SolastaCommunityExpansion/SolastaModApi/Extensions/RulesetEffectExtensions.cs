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
    [TargetType(typeof(RulesetEffect))]
    public static partial class RulesetEffectExtensions
    {
        public static T SetApplied<T>(this T entity, RulesetEffect.RulesetActiveEffectAppliedHandler value)
            where T : RulesetEffect
        {
            entity.SetField("<Applied>k__BackingField", value);
            return entity;
        }

        public static T SetConditionTrackingStarted<T>(this T entity, RulesetEffect.ConditionTrackingStartedHandler value)
            where T : RulesetEffect
        {
            entity.SetField("<ConditionTrackingStarted>k__BackingField", value);
            return entity;
        }

        public static T SetConditionTrackingStopped<T>(this T entity, RulesetEffect.ConditionTrackingStoppedHandler value)
            where T : RulesetEffect
        {
            entity.SetField("<ConditionTrackingStopped>k__BackingField", value);
            return entity;
        }

        public static T SetDelayEnvironmentRegistration<T>(this T entity, System.Boolean value)
            where T : RulesetEffect
        {
            entity.DelayEnvironmentRegistration = value;
            return entity;
        }

        public static T SetMetamagicOption<T>(this T entity, MetamagicOptionDefinition value)
            where T : RulesetEffect
        {
            entity.MetamagicOption = value;
            return entity;
        }

        public static T SetRemainingRounds<T>(this T entity, System.Int32 value)
            where T : RulesetEffect
        {
            entity.RemainingRounds = value;
            return entity;
        }

        public static T SetTerminated<T>(this T entity, System.Boolean value)
            where T : RulesetEffect
        {
            entity.Terminated = value;
            return entity;
        }

        public static T SetTerminatedSelf<T>(this T entity, RulesetEffect.TerminatedSelfHandler value)
            where T : RulesetEffect
        {
            entity.SetField("<TerminatedSelf>k__BackingField", value);
            return entity;
        }

        public static T SetUpdated<T>(this T entity, RulesetEffect.RulesetActiveEffectUpdatedHandler value)
            where T : RulesetEffect
        {
            entity.SetField("<Updated>k__BackingField", value);
            return entity;
        }
    }
}