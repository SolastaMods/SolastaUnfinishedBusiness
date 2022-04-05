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
    [TargetType(typeof(FeatureDefinitionLightAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionLightAffinityExtensions
    {
        public static T AddLightingEffectAndConditionList<T>(this T entity,  params  FeatureDefinitionLightAffinity . LightingEffectAndCondition [ ]  value)
            where T : FeatureDefinitionLightAffinity
        {
            AddLightingEffectAndConditionList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLightingEffectAndConditionList<T>(this T entity, IEnumerable<FeatureDefinitionLightAffinity.LightingEffectAndCondition> value)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.AddRange(value);
            return entity;
        }

        public static T ClearLightingEffectAndConditionList<T>(this T entity)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.Clear();
            return entity;
        }

        public static T SetLightingEffectAndConditionList<T>(this T entity,  params  FeatureDefinitionLightAffinity . LightingEffectAndCondition [ ]  value)
            where T : FeatureDefinitionLightAffinity
        {
            SetLightingEffectAndConditionList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLightingEffectAndConditionList<T>(this T entity, IEnumerable<FeatureDefinitionLightAffinity.LightingEffectAndCondition> value)
            where T : FeatureDefinitionLightAffinity
        {
            entity.LightingEffectAndConditionList.SetRange(value);
            return entity;
        }
    }
}