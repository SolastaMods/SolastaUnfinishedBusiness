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
    [TargetType(typeof(RulesetUsablePower)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetUsablePowerExtensions
    {
        public static T SetMaxUses<T>(this T entity, System.Int32 value)
            where T : RulesetUsablePower
        {
            entity.SetField("maxUses", value);
            return entity;
        }

        public static T SetOriginClass<T>(this T entity, CharacterClassDefinition value)
            where T : RulesetUsablePower
        {
            entity.SetField("originClass", value);
            return entity;
        }

        public static T SetOriginRace<T>(this T entity, CharacterRaceDefinition value)
            where T : RulesetUsablePower
        {
            entity.SetField("originRace", value);
            return entity;
        }

        public static T SetPowerDefinition<T>(this T entity, FeatureDefinitionPower value)
            where T : RulesetUsablePower
        {
            entity.SetField("powerDefinition", value);
            return entity;
        }

        public static T SetRemainingUses<T>(this T entity, System.Int32 value)
            where T : RulesetUsablePower
        {
            entity.SetField("remainingUses", value);
            return entity;
        }

        public static T SetSaveDC<T>(this T entity, System.Int32 value)
            where T : RulesetUsablePower
        {
            entity.SaveDC = value;
            return entity;
        }

        public static T SetSpentPoints<T>(this T entity, System.Int32 value)
            where T : RulesetUsablePower
        {
            entity.SetField("spentPoints", value);
            return entity;
        }

        public static T SetUsesAttribute<T>(this T entity, RulesetAttribute value)
            where T : RulesetUsablePower
        {
            entity.UsesAttribute = value;
            return entity;
        }
    }
}