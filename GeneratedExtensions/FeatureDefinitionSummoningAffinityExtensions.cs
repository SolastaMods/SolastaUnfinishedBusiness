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
    [TargetType(typeof(FeatureDefinitionSummoningAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionSummoningAffinityExtensions
    {
        public static T AddAddedConditions<T>(this T entity,  params  ConditionDefinition [ ]  value)
            where T : FeatureDefinitionSummoningAffinity
        {
            AddAddedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAddedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.AddRange(value);
            return entity;
        }

        public static T AddEffectForms<T>(this T entity,  params  EffectForm [ ]  value)
            where T : FeatureDefinitionSummoningAffinity
        {
            AddEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.AddRange(value);
            return entity;
        }

        public static T ClearAddedConditions<T>(this T entity)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.Clear();
            return entity;
        }

        public static T ClearEffectForms<T>(this T entity)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.Clear();
            return entity;
        }

        public static T SetAddedConditions<T>(this T entity,  params  ConditionDefinition [ ]  value)
            where T : FeatureDefinitionSummoningAffinity
        {
            SetAddedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAddedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.AddedConditions.SetRange(value);
            return entity;
        }

        public static T SetEffectForms<T>(this T entity,  params  EffectForm [ ]  value)
            where T : FeatureDefinitionSummoningAffinity
        {
            SetEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.EffectForms.SetRange(value);
            return entity;
        }

        public static T SetEffectOnConjuredDeath<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.SetField("effectOnConjuredDeath", value);
            return entity;
        }

        public static T SetRequiredMonsterTag<T>(this T entity, System.String value)
            where T : FeatureDefinitionSummoningAffinity
        {
            entity.SetField("requiredMonsterTag", value);
            return entity;
        }
    }
}