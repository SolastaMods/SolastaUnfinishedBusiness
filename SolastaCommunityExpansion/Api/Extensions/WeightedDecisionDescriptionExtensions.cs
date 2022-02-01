using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
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
    [TargetType(typeof(WeightedDecisionDescription))]
    public static partial class WeightedDecisionDescriptionExtensions
    {
        public static T SetCooldown<T>(this T entity, System.Int32 value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("cooldown", value);
            return entity;
        }

        public static T SetDecision<T>(this T entity, TA.AI.DecisionDefinition value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("decision", value);
            return entity;
        }

        public static T SetDynamicCooldown<T>(this T entity, System.Boolean value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("dynamicCooldown", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, System.Single value)
            where T : WeightedDecisionDescription
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}