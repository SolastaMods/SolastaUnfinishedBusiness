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
    [TargetType(typeof(FeatureDefinitionLanguageAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionLanguageAffinityExtensions
    {
        public static T AddKnownLanguages<T>(this T entity,  params  System . String [ ]  value)
            where T : FeatureDefinitionLanguageAffinity
        {
            AddKnownLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownLanguages<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionLanguageAffinity
        {
            entity.KnownLanguages.AddRange(value);
            return entity;
        }

        public static T ClearKnownLanguages<T>(this T entity)
            where T : FeatureDefinitionLanguageAffinity
        {
            entity.KnownLanguages.Clear();
            return entity;
        }

        public static T SetKnownLanguages<T>(this T entity,  params  System . String [ ]  value)
            where T : FeatureDefinitionLanguageAffinity
        {
            SetKnownLanguages(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownLanguages<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionLanguageAffinity
        {
            entity.KnownLanguages.SetRange(value);
            return entity;
        }

        public static T SetUniversalReader<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionLanguageAffinity
        {
            entity.SetField("universalReader", value);
            return entity;
        }
    }
}