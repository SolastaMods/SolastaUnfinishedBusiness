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
    [TargetType(typeof(RulesetEffect)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetEffectExtensions
    {
        public static T AddMagicAttackTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : RulesetEffect
        {
            AddMagicAttackTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMagicAttackTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : RulesetEffect
        {
            entity.MagicAttackTrends.AddRange(value);
            return entity;
        }

        public static T AddSourceTags<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetEffect
        {
            AddSourceTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSourceTags<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetEffect
        {
            entity.SourceTags.AddRange(value);
            return entity;
        }

        public static T AddTrackedConditionGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            AddTrackedConditionGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrackedConditionGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedConditionGuids.AddRange(value);
            return entity;
        }

        public static T AddTrackedEffectProxyGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            AddTrackedEffectProxyGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrackedEffectProxyGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedEffectProxyGuids.AddRange(value);
            return entity;
        }

        public static T AddTrackedItemPropertyGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            AddTrackedItemPropertyGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrackedItemPropertyGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedItemPropertyGuids.AddRange(value);
            return entity;
        }

        public static T AddTrackedLightSourceGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            AddTrackedLightSourceGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrackedLightSourceGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedLightSourceGuids.AddRange(value);
            return entity;
        }

        public static T AddTrackedSummonedItemGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            AddTrackedSummonedItemGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddTrackedSummonedItemGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedSummonedItemGuids.AddRange(value);
            return entity;
        }

        public static T ClearMagicAttackTrends<T>(this T entity)
            where T : RulesetEffect
        {
            entity.MagicAttackTrends.Clear();
            return entity;
        }

        public static T ClearSourceTags<T>(this T entity)
            where T : RulesetEffect
        {
            entity.SourceTags.Clear();
            return entity;
        }

        public static T ClearTrackedConditionGuids<T>(this T entity)
            where T : RulesetEffect
        {
            entity.TrackedConditionGuids.Clear();
            return entity;
        }

        public static T ClearTrackedEffectProxyGuids<T>(this T entity)
            where T : RulesetEffect
        {
            entity.TrackedEffectProxyGuids.Clear();
            return entity;
        }

        public static T ClearTrackedItemPropertyGuids<T>(this T entity)
            where T : RulesetEffect
        {
            entity.TrackedItemPropertyGuids.Clear();
            return entity;
        }

        public static T ClearTrackedLightSourceGuids<T>(this T entity)
            where T : RulesetEffect
        {
            entity.TrackedLightSourceGuids.Clear();
            return entity;
        }

        public static T ClearTrackedSummonedItemGuids<T>(this T entity)
            where T : RulesetEffect
        {
            entity.TrackedSummonedItemGuids.Clear();
            return entity;
        }

        public static System.Collections.Generic.List<ISavingThrowAffinityProvider> GetAccountedProviders<T>(this T entity)
            where T : RulesetEffect
        {
            return entity.GetField<System.Collections.Generic.List<ISavingThrowAffinityProvider>>("accountedProviders");
        }

        public static System.Collections.Generic.List<RulesetCondition> GetConditionsToRemove<T>(this T entity)
            where T : RulesetEffect
        {
            return entity.GetField<System.Collections.Generic.List<RulesetCondition>>("conditionsToRemove");
        }

        public static System.Collections.Generic.Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> GetFeaturesOrigin<T>(this T entity)
            where T : RulesetEffect
        {
            return entity.GetField<System.Collections.Generic.Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>>("featuresOrigin");
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetFeaturesToBrowse<T>(this T entity)
            where T : RulesetEffect
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("featuresToBrowse");
        }

        public static System.Collections.Generic.List<System.UInt64> GetInvalidConditionGuids<T>(this T entity)
            where T : RulesetEffect
        {
            return entity.GetField<System.Collections.Generic.List<System.UInt64>>("invalidConditionGuids");
        }

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

        public static T SetEndOfDurationReached<T>(this T entity, System.Boolean value)
            where T : RulesetEffect
        {
            entity.SetField("<EndOfDurationReached>k__BackingField", value);
            return entity;
        }

        public static T SetMagicAttackTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : RulesetEffect
        {
            SetMagicAttackTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMagicAttackTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : RulesetEffect
        {
            entity.MagicAttackTrends.SetRange(value);
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

        public static T SetSourceTags<T>(this T entity,  params  System . String [ ]  value)
            where T : RulesetEffect
        {
            SetSourceTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSourceTags<T>(this T entity, IEnumerable<System.String> value)
            where T : RulesetEffect
        {
            entity.SourceTags.SetRange(value);
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

        public static T SetTrackedConditionGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            SetTrackedConditionGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrackedConditionGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedConditionGuids.SetRange(value);
            return entity;
        }

        public static T SetTrackedEffectProxyGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            SetTrackedEffectProxyGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrackedEffectProxyGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedEffectProxyGuids.SetRange(value);
            return entity;
        }

        public static T SetTrackedItemPropertyGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            SetTrackedItemPropertyGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrackedItemPropertyGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedItemPropertyGuids.SetRange(value);
            return entity;
        }

        public static T SetTrackedLightSourceGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            SetTrackedLightSourceGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrackedLightSourceGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedLightSourceGuids.SetRange(value);
            return entity;
        }

        public static T SetTrackedSummonedItemGuids<T>(this T entity,  params  System . UInt64 [ ]  value)
            where T : RulesetEffect
        {
            SetTrackedSummonedItemGuids(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetTrackedSummonedItemGuids<T>(this T entity, IEnumerable<System.UInt64> value)
            where T : RulesetEffect
        {
            entity.TrackedSummonedItemGuids.SetRange(value);
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