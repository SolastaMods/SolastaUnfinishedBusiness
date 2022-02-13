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
    [TargetType(typeof(MetamagicOptionDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MetamagicOptionDefinitionExtensions
    {
        public static T SetBoundFeature<T>(this T entity, FeatureDefinition value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("boundFeature", value);
            return entity;
        }

        public static T SetCostMethod<T>(this T entity, RuleDefinitions.MetamagicCostMethod value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("costMethod", value);
            return entity;
        }

        public static T SetMetamagicType<T>(this T entity, RuleDefinitions.MetamagicType value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("metamagicType", value);
            return entity;
        }

        public static T SetParameterMethod<T>(this T entity, RuleDefinitions.MetamagicParameterMethod value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("parameterMethod", value);
            return entity;
        }

        public static T SetParameterValue<T>(this T entity, System.Int32 value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("parameterValue", value);
            return entity;
        }

        public static T SetSorceryPointsCost<T>(this T entity, System.Int32 value)
            where T : MetamagicOptionDefinition
        {
            entity.SetField("sorceryPointsCost", value);
            return entity;
        }
    }
}