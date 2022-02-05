using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
    [TargetType(typeof(FactionDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FactionDefinitionExtensions
    {
        public static T SetAttackingPenalty<T>(this T entity, System.Int32 value)
            where T : FactionDefinition
        {
            entity.SetField("attackingPenalty", value);
            return entity;
        }

        public static T SetBuiltIn<T>(this T entity, System.Boolean value)
            where T : FactionDefinition
        {
            entity.SetField("builtIn", value);
            return entity;
        }

        public static T SetKillingPenalty<T>(this T entity, System.Int32 value)
            where T : FactionDefinition
        {
            entity.SetField("killingPenalty", value);
            return entity;
        }

        public static T SetMaxRelationCap<T>(this T entity, System.Int32 value)
            where T : FactionDefinition
        {
            entity.SetField("maxRelationCap", value);
            return entity;
        }

        public static T SetMinRelationCap<T>(this T entity, System.Int32 value)
            where T : FactionDefinition
        {
            entity.SetField("minRelationCap", value);
            return entity;
        }

        public static T SetSmallSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : FactionDefinition
        {
            entity.SetField("smallSpriteReference", value);
            return entity;
        }

        public static T SetStealingPenalty<T>(this T entity, System.Int32 value)
            where T : FactionDefinition
        {
            entity.SetField("stealingPenalty", value);
            return entity;
        }
    }
}