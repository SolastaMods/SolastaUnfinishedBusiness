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
    [TargetType(typeof(FightingStyleDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FightingStyleDefinitionExtensions
    {
        public static T AddFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : FightingStyleDefinition
        {
            AddFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : FightingStyleDefinition
        {
            entity.Features.AddRange(value);
            return entity;
        }

        public static T ClearFeatures<T>(this T entity)
            where T : FightingStyleDefinition
        {
            entity.Features.Clear();
            return entity;
        }

        public static T SetCondition<T>(this T entity, FightingStyleDefinition.TriggerCondition value)
            where T : FightingStyleDefinition
        {
            entity.SetField("condition", value);
            return entity;
        }

        public static T SetFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : FightingStyleDefinition
        {
            SetFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : FightingStyleDefinition
        {
            entity.Features.SetRange(value);
            return entity;
        }
    }
}