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
    [TargetType(typeof(AlignmentDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class AlignmentDefinitionExtensions
    {
        public static T AddDefaultPersonalityFlags<T>(this T entity,  params  System . String [ ]  value)
            where T : AlignmentDefinition
        {
            AddDefaultPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDefaultPersonalityFlags<T>(this T entity, IEnumerable<System.String> value)
            where T : AlignmentDefinition
        {
            entity.DefaultPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T AddOptionalPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : AlignmentDefinition
        {
            AddOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOptionalPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : AlignmentDefinition
        {
            entity.OptionalPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T ClearDefaultPersonalityFlags<T>(this T entity)
            where T : AlignmentDefinition
        {
            entity.DefaultPersonalityFlags.Clear();
            return entity;
        }

        public static T ClearOptionalPersonalityFlags<T>(this T entity)
            where T : AlignmentDefinition
        {
            entity.OptionalPersonalityFlags.Clear();
            return entity;
        }

        public static T SetDefaultPersonalityFlags<T>(this T entity,  params  System . String [ ]  value)
            where T : AlignmentDefinition
        {
            SetDefaultPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDefaultPersonalityFlags<T>(this T entity, IEnumerable<System.String> value)
            where T : AlignmentDefinition
        {
            entity.DefaultPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetGoodnessAxis<T>(this T entity, System.Int32 value)
            where T : AlignmentDefinition
        {
            entity.SetField("goodnessAxis", value);
            return entity;
        }

        public static T SetLawAxis<T>(this T entity, System.Int32 value)
            where T : AlignmentDefinition
        {
            entity.SetField("lawAxis", value);
            return entity;
        }

        public static T SetOptionalPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : AlignmentDefinition
        {
            SetOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOptionalPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : AlignmentDefinition
        {
            entity.OptionalPersonalityFlags.SetRange(value);
            return entity;
        }
    }
}