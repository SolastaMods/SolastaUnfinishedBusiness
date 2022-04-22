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
    [TargetType(typeof(FeatureDefinitionPerceptionAffinity)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionPerceptionAffinityExtensions
    {
        public static T AddRequiredSensesToPerceive<T>(this T entity,  params  SenseMode . Type [ ]  value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            AddRequiredSensesToPerceive(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRequiredSensesToPerceive<T>(this T entity, IEnumerable<SenseMode.Type> value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.RequiredSensesToPerceive.AddRange(value);
            return entity;
        }

        public static T ClearRequiredSensesToPerceive<T>(this T entity)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.RequiredSensesToPerceive.Clear();
            return entity;
        }

        public static T SetCannotBeSurprised<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.SetField("cannotBeSurprised", value);
            return entity;
        }

        public static T SetCharacterFamilyRevealed<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.SetField("characterFamilyRevealed", value);
            return entity;
        }

        public static T SetCharacterPositionRevealed<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.SetField("characterPositionRevealed", value);
            return entity;
        }

        public static T SetImpairedSight<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.SetField("impairedSight", value);
            return entity;
        }

        public static T SetRequiredSensesToPerceive<T>(this T entity,  params  SenseMode . Type [ ]  value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            SetRequiredSensesToPerceive(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRequiredSensesToPerceive<T>(this T entity, IEnumerable<SenseMode.Type> value)
            where T : FeatureDefinitionPerceptionAffinity
        {
            entity.RequiredSensesToPerceive.SetRange(value);
            return entity;
        }
    }
}