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
    [TargetType(typeof(PointPool)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class PointPoolExtensions
    {
        public static T AddRestrictedChoices<T>(this T entity,  params  System . String [ ]  value)
            where T : PointPool
        {
            AddRestrictedChoices(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRestrictedChoices<T>(this T entity, IEnumerable<System.String> value)
            where T : PointPool
        {
            entity.RestrictedChoices.AddRange(value);
            return entity;
        }

        public static T ClearRestrictedChoices<T>(this T entity)
            where T : PointPool
        {
            entity.RestrictedChoices.Clear();
            return entity;
        }

        public static T SetDescription<T>(this T entity, System.String value)
            where T : PointPool
        {
            entity.Description = value;
            return entity;
        }

        public static T SetMaxPoints<T>(this T entity, System.Int32 value)
            where T : PointPool
        {
            entity.MaxPoints = value;
            return entity;
        }

        public static T SetRemainingPoints<T>(this T entity, System.Int32 value)
            where T : PointPool
        {
            entity.RemainingPoints = value;
            return entity;
        }

        public static T SetRestrictedChoices<T>(this T entity,  params  System . String [ ]  value)
            where T : PointPool
        {
            SetRestrictedChoices(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRestrictedChoices<T>(this T entity, IEnumerable<System.String> value)
            where T : PointPool
        {
            entity.RestrictedChoices.SetRange(value);
            return entity;
        }

        public static T SetUniqueChoices<T>(this T entity, System.Boolean value)
            where T : PointPool
        {
            entity.SetField("uniqueChoices", value);
            return entity;
        }
    }
}