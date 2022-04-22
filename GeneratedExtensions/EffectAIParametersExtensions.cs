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
    [TargetType(typeof(EffectAIParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EffectAIParametersExtensions
    {
        public static EffectAIParameters Copy(this EffectAIParameters entity)
        {
            var copy = new EffectAIParameters();
            copy.Copy(entity);
            return copy;
        }

        public static T SetAoeScoreMultiplier<T>(this T entity, System.Single value)
            where T : EffectAIParameters
        {
            entity.SetField("aoeScoreMultiplier", value);
            return entity;
        }

        public static T SetCooldownForBattle<T>(this T entity, System.Int32 value)
            where T : EffectAIParameters
        {
            entity.SetField("cooldownForBattle", value);
            return entity;
        }

        public static T SetCooldownForCaster<T>(this T entity, System.Int32 value)
            where T : EffectAIParameters
        {
            entity.SetField("cooldownForCaster", value);
            return entity;
        }

        public static T SetDynamicCooldown<T>(this T entity, System.Boolean value)
            where T : EffectAIParameters
        {
            entity.SetField("dynamicCooldown", value);
            return entity;
        }
    }
}