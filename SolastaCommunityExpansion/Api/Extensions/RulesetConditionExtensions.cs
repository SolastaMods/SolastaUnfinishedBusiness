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
    [TargetType(typeof(RulesetCondition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetConditionExtensions
    {
        public static T SetAmount<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.Amount = value;
            return entity;
        }

        public static T SetCanSaveToCancel<T>(this T entity, System.Boolean value)
            where T : RulesetCondition
        {
            entity.CanSaveToCancel = value;
            return entity;
        }

        public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
            where T : RulesetCondition
        {
            entity.ConditionDefinition = value;
            return entity;
        }

        public static T SetDurationChanged<T>(this T entity, RulesetCondition.DurationChangedHandler value)
            where T : RulesetCondition
        {
            entity.SetField("<DurationChanged>k__BackingField", value);
            return entity;
        }

        public static T SetDurationParameter<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.SetField("durationParameter", value);
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : RulesetCondition
        {
            entity.SetField("durationType", value);
            return entity;
        }

        public static T SetEffectDefinitionName<T>(this T entity, System.String value)
            where T : RulesetCondition
        {
            entity.SetField("effectDefinitionName", value);
            return entity;
        }

        public static T SetEffectLevel<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.SetField("effectLevel", value);
            return entity;
        }

        public static T SetEndOccurence<T>(this T entity, RuleDefinitions.TurnOccurenceType value)
            where T : RulesetCondition
        {
            entity.EndOccurence = value;
            return entity;
        }

        public static T SetHasSaveOverride<T>(this T entity, System.Boolean value)
            where T : RulesetCondition
        {
            entity.HasSaveOverride = value;
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : RulesetCondition
        {
            entity.Name = value;
            return entity;
        }

        public static T SetRemainingRounds<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.RemainingRounds = value;
            return entity;
        }

        public static T SetSaveOverrideAbilityScoreName<T>(this T entity, System.String value)
            where T : RulesetCondition
        {
            entity.SaveOverrideAbilityScoreName = value;
            return entity;
        }

        public static T SetSaveOverrideDC<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.SaveOverrideDC = value;
            return entity;
        }

        public static T SetSaveOverrideSourceName<T>(this T entity, System.String value)
            where T : RulesetCondition
        {
            entity.SaveOverrideSourceName = value;
            return entity;
        }

        public static T SetSaveOverrideSourceType<T>(this T entity, RuleDefinitions.FeatureSourceType value)
            where T : RulesetCondition
        {
            entity.SaveOverrideSourceType = value;
            return entity;
        }

        public static T SetSourceAbilityBonus<T>(this T entity, System.Int32 value)
            where T : RulesetCondition
        {
            entity.SourceAbilityBonus = value;
            return entity;
        }

        public static T SetSourceFactionName<T>(this T entity, System.String value)
            where T : RulesetCondition
        {
            entity.SetField("sourceFactionName", value);
            return entity;
        }

        public static T SetSourceGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetCondition
        {
            entity.SetField("sourceGuid", value);
            return entity;
        }

        public static T SetTargetGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetCondition
        {
            entity.TargetGuid = value;
            return entity;
        }

        public static T SetTerminationKillsConjured<T>(this T entity, System.Boolean value)
            where T : RulesetCondition
        {
            entity.TerminationKillsConjured = value;
            return entity;
        }
    }
}