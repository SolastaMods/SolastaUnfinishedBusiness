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
    [TargetType(typeof(CharacterBackgroundDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterBackgroundDefinitionExtensions
    {
        public static T AddBackgroudSubtypes<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddBackgroudSubtypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddBackgroudSubtypes<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.BackgroudSubtypes.AddRange(value);
            return entity;
        }

        public static T AddDefaultOptionalPersonalityFlags<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddDefaultOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDefaultOptionalPersonalityFlags<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.DefaultOptionalPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T AddEquipmentRows<T>(this T entity,  params  CharacterClassDefinition . HeroEquipmentRow [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddEquipmentRows(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEquipmentRows<T>(this T entity, IEnumerable<CharacterClassDefinition.HeroEquipmentRow> value)
            where T : CharacterBackgroundDefinition
        {
            entity.EquipmentRows.AddRange(value);
            return entity;
        }

        public static T AddFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterBackgroundDefinition
        {
            entity.Features.AddRange(value);
            return entity;
        }

        public static T AddForbiddenAlignments<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddForbiddenAlignments(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddForbiddenAlignments<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.ForbiddenAlignments.AddRange(value);
            return entity;
        }

        public static T AddOptionalPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOptionalPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterBackgroundDefinition
        {
            entity.OptionalPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T AddStaticPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            AddStaticPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStaticPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterBackgroundDefinition
        {
            entity.StaticPersonalityFlags.AddRange(value);
            return entity;
        }

        public static T ClearBackgroudSubtypes<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.BackgroudSubtypes.Clear();
            return entity;
        }

        public static T ClearDefaultOptionalPersonalityFlags<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.DefaultOptionalPersonalityFlags.Clear();
            return entity;
        }

        public static T ClearEquipmentRows<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.EquipmentRows.Clear();
            return entity;
        }

        public static T ClearFeatures<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.Features.Clear();
            return entity;
        }

        public static T ClearForbiddenAlignments<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.ForbiddenAlignments.Clear();
            return entity;
        }

        public static T ClearOptionalPersonalityFlags<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.OptionalPersonalityFlags.Clear();
            return entity;
        }

        public static T ClearStaticPersonalityFlags<T>(this T entity)
            where T : CharacterBackgroundDefinition
        {
            entity.StaticPersonalityFlags.Clear();
            return entity;
        }

        public static T SetBackgroudSubtypes<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetBackgroudSubtypes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetBackgroudSubtypes<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.BackgroudSubtypes.SetRange(value);
            return entity;
        }

        public static T SetBanterList<T>(this T entity, BanterDefinitions.BanterList value)
            where T : CharacterBackgroundDefinition
        {
            entity.SetField("banterList", value);
            return entity;
        }

        public static T SetDefaultOptionalPersonalityFlags<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetDefaultOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDefaultOptionalPersonalityFlags<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.DefaultOptionalPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetEquipmentRows<T>(this T entity,  params  CharacterClassDefinition . HeroEquipmentRow [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetEquipmentRows(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEquipmentRows<T>(this T entity, IEnumerable<CharacterClassDefinition.HeroEquipmentRow> value)
            where T : CharacterBackgroundDefinition
        {
            entity.EquipmentRows.SetRange(value);
            return entity;
        }

        public static T SetFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : CharacterBackgroundDefinition
        {
            entity.Features.SetRange(value);
            return entity;
        }

        public static T SetForbiddenAlignments<T>(this T entity,  params  System . String [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetForbiddenAlignments(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetForbiddenAlignments<T>(this T entity, IEnumerable<System.String> value)
            where T : CharacterBackgroundDefinition
        {
            entity.ForbiddenAlignments.SetRange(value);
            return entity;
        }

        public static T SetHasSubtype<T>(this T entity, System.Boolean value)
            where T : CharacterBackgroundDefinition
        {
            entity.SetField("hasSubtype", value);
            return entity;
        }

        public static T SetOptionalPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetOptionalPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOptionalPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterBackgroundDefinition
        {
            entity.OptionalPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetRequiresDeity<T>(this T entity, System.Boolean value)
            where T : CharacterBackgroundDefinition
        {
            entity.SetField("requiresDeity", value);
            return entity;
        }

        public static T SetStaticPersonalityFlags<T>(this T entity,  params  PersonalityFlagOccurence [ ]  value)
            where T : CharacterBackgroundDefinition
        {
            SetStaticPersonalityFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStaticPersonalityFlags<T>(this T entity, IEnumerable<PersonalityFlagOccurence> value)
            where T : CharacterBackgroundDefinition
        {
            entity.StaticPersonalityFlags.SetRange(value);
            return entity;
        }

        public static T SetSubTypeName<T>(this T entity, System.String value)
            where T : CharacterBackgroundDefinition
        {
            entity.SetField("subTypeName", value);
            return entity;
        }
    }
}