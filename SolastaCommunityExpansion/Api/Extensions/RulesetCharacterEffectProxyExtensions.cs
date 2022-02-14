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
    [TargetType(typeof(RulesetCharacterEffectProxy)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetCharacterEffectProxyExtensions
    {
        public static T AddAdditionalPersonalLightSources<T>(this T entity,  params  RulesetLightSource [ ]  value)
            where T : RulesetCharacterEffectProxy
        {
            AddAdditionalPersonalLightSources(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAdditionalPersonalLightSources<T>(this T entity, IEnumerable<RulesetLightSource> value)
            where T : RulesetCharacterEffectProxy
        {
            entity.AdditionalPersonalLightSources.AddRange(value);
            return entity;
        }

        public static T ClearAdditionalPersonalLightSources<T>(this T entity)
            where T : RulesetCharacterEffectProxy
        {
            entity.AdditionalPersonalLightSources.Clear();
            return entity;
        }

        public static T SetAdditionalPersonalLightSourceAdded<T>(this T entity, RulesetCharacterEffectProxy.AdditionalPersonalLightSourceAddedHandler value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("<AdditionalPersonalLightSourceAdded>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalPersonalLightSourceRemoved<T>(this T entity, RulesetCharacterEffectProxy.AdditionalPersonalLightSourceRemovedHandler value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("<AdditionalPersonalLightSourceRemoved>k__BackingField", value);
            return entity;
        }

        public static T SetAdditionalPersonalLightSources<T>(this T entity,  params  RulesetLightSource [ ]  value)
            where T : RulesetCharacterEffectProxy
        {
            SetAdditionalPersonalLightSources(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAdditionalPersonalLightSources<T>(this T entity, IEnumerable<RulesetLightSource> value)
            where T : RulesetCharacterEffectProxy
        {
            entity.AdditionalPersonalLightSources.SetRange(value);
            return entity;
        }

        public static T SetControllerGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("controllerGuid", value);
            return entity;
        }

        public static T SetEffectDefinitionName<T>(this T entity, System.String value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("effectDefinitionName", value);
            return entity;
        }

        public static T SetEffectGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("effectGuid", value);
            return entity;
        }

        public static T SetEffectProxyDefinition<T>(this T entity, EffectProxyDefinition value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("effectProxyDefinition", value);
            return entity;
        }

        public static T SetHasAttackedOnce<T>(this T entity, System.Boolean value)
            where T : RulesetCharacterEffectProxy
        {
            entity.HasAttackedOnce = value;
            return entity;
        }

        public static T SetOnEffectProxyDestroying<T>(this T entity, RulesetCharacterEffectProxy.OnEffectProxyDestroyingHandler value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("<OnEffectProxyDestroying>k__BackingField", value);
            return entity;
        }

        public static T SetSourceAbilityBonus<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("sourceAbilityBonus", value);
            return entity;
        }

        public static T SetSourceAbilityName<T>(this T entity, System.String value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("sourceAbilityName", value);
            return entity;
        }

        public static T SetSourceDC<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("sourceDC", value);
            return entity;
        }

        public static T SetSourceProficiencyBonus<T>(this T entity, System.Int32 value)
            where T : RulesetCharacterEffectProxy
        {
            entity.SetField("sourceProficiencyBonus", value);
            return entity;
        }
    }
}