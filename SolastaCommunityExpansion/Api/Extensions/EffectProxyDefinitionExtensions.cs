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
    [TargetType(typeof(EffectProxyDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EffectProxyDefinitionExtensions
    {
        public static T AddAdditionalFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : EffectProxyDefinition
        {
            AddAdditionalFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAdditionalFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : EffectProxyDefinition
        {
            entity.AdditionalFeatures.AddRange(value);
            return entity;
        }

        public static T ClearAdditionalFeatures<T>(this T entity)
            where T : EffectProxyDefinition
        {
            entity.AdditionalFeatures.Clear();
            return entity;
        }

        public static T SetActionId<T>(this T entity, ActionDefinitions.Id value)
            where T : EffectProxyDefinition
        {
            entity.SetField("actionId", value);
            return entity;
        }

        public static T SetAddAbilityToDamage<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("addAbilityToDamage", value);
            return entity;
        }

        public static T SetAdditionalFeatures<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : EffectProxyDefinition
        {
            SetAdditionalFeatures(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAdditionalFeatures<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : EffectProxyDefinition
        {
            entity.AdditionalFeatures.SetRange(value);
            return entity;
        }

        public static T SetAddLightSource<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("addLightSource", value);
            return entity;
        }

        public static T SetAttackImpactParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : EffectProxyDefinition
        {
            entity.SetField("attackImpactParticle", value);
            return entity;
        }

        public static T SetAttackMethod<T>(this T entity, RuleDefinitions.ProxyAttackMethod value)
            where T : EffectProxyDefinition
        {
            entity.SetField("attackMethod", value);
            return entity;
        }

        public static T SetAttackParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : EffectProxyDefinition
        {
            entity.SetField("attackParticle", value);
            return entity;
        }

        public static T SetAttackPower<T>(this T entity, FeatureDefinitionPower value)
            where T : EffectProxyDefinition
        {
            entity.SetField("attackPower", value);
            return entity;
        }

        public static T SetAutoTerminateOnTriggerPower<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("autoTerminateOnTriggerPower", value);
            return entity;
        }

        public static T SetCanAttack<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("canAttack", value);
            return entity;
        }

        public static T SetCanMove<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("canMove", value);
            return entity;
        }

        public static T SetCanMoveOnCharacters<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("canMoveOnCharacters", value);
            return entity;
        }

        public static T SetCanRotate<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("canRotate", value);
            return entity;
        }

        public static T SetCanTriggerPower<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("canTriggerPower", value);
            return entity;
        }

        public static T SetConstrainedToSpellArea<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("constrainedToSpellArea", value);
            return entity;
        }

        public static T SetDamageDie<T>(this T entity, RuleDefinitions.DieType value)
            where T : EffectProxyDefinition
        {
            entity.SetField("damageDie", value);
            return entity;
        }

        public static T SetDamageDieNum<T>(this T entity, System.Int32 value)
            where T : EffectProxyDefinition
        {
            entity.SetField("damageDieNum", value);
            return entity;
        }

        public static T SetDamageType<T>(this T entity, System.String value)
            where T : EffectProxyDefinition
        {
            entity.SetField("damageType", value);
            return entity;
        }

        public static T SetFirstAttackIsFree<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("firstAttackIsFree", value);
            return entity;
        }

        public static T SetFreeActionId<T>(this T entity, ActionDefinitions.Id value)
            where T : EffectProxyDefinition
        {
            entity.SetField("freeActionId", value);
            return entity;
        }

        public static T SetHasPortrait<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("hasPortrait", value);
            return entity;
        }

        public static T SetHasPresentation<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("hasPresentation", value);
            return entity;
        }

        public static T SetImpactsPlacement<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("impactsPlacement", value);
            return entity;
        }

        public static T SetIsEmptyPresentation<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("isEmptyPresentation", value);
            return entity;
        }

        public static T SetLightSourceForm<T>(this T entity, LightSourceForm value)
            where T : EffectProxyDefinition
        {
            entity.SetField("lightSourceForm", value);
            return entity;
        }

        public static T SetLightSourceOffset<T>(this T entity, UnityEngine.Vector3 value)
            where T : EffectProxyDefinition
        {
            entity.SetField("lightSourceOffset", value);
            return entity;
        }

        public static T SetMaxSpellLevelImmunity<T>(this T entity, System.Int32 value)
            where T : EffectProxyDefinition
        {
            entity.SetField("maxSpellLevelImmunity", value);
            return entity;
        }

        public static T SetModelScale<T>(this T entity, System.Single value)
            where T : EffectProxyDefinition
        {
            entity.SetField("modelScale", value);
            return entity;
        }

        public static T SetPortraitSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : EffectProxyDefinition
        {
            entity.SetField("portraitSpriteReference", value);
            return entity;
        }

        public static T SetPrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : EffectProxyDefinition
        {
            entity.SetField("prefabReference", value);
            return entity;
        }

        public static T SetShowWorldLocationFeedbacks<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("showWorldLocationFeedbacks", value);
            return entity;
        }

        public static T SetSoundEffectOnHitDescription<T>(this T entity, SoundEffectOnHitDescription value)
            where T : EffectProxyDefinition
        {
            entity.SetField("soundEffectOnHitDescription", value);
            return entity;
        }

        public static T SetSpellImmunityFromOutside<T>(this T entity, System.Boolean value)
            where T : EffectProxyDefinition
        {
            entity.SetField("spellImmunityFromOutside", value);
            return entity;
        }

        public static T SetStartEvent<T>(this T entity, AK.Wwise.Event value)
            where T : EffectProxyDefinition
        {
            entity.SetField("startEvent", value);
            return entity;
        }

        public static T SetStopEvent<T>(this T entity, AK.Wwise.Event value)
            where T : EffectProxyDefinition
        {
            entity.SetField("stopEvent", value);
            return entity;
        }
    }
}