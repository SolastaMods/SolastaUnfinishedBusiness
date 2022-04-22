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
    [TargetType(typeof(WeaponTypeDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class WeaponTypeDefinitionExtensions
    {
        public static T SetAnimationTag<T>(this T entity, System.String value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("animationTag", value);
            return entity;
        }

        public static T SetIsAttachedToBone<T>(this T entity, AnimationDefinitions.BoneType value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("isAttachedToBone", value);
            return entity;
        }

        public static T SetIsBow<T>(this T entity, System.Boolean value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("isBow", value);
            return entity;
        }

        public static T SetMeleeAttackerParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("meleeAttackerParticleReference", value);
            return entity;
        }

        public static T SetMeleeImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("meleeImpactParticleReference", value);
            return entity;
        }

        public static T SetSecondaryAnimationTag<T>(this T entity, System.String value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("secondaryAnimationTag", value);
            return entity;
        }

        public static T SetSoundEffectDescription<T>(this T entity, SoundEffectDescription value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("soundEffectDescription", value);
            return entity;
        }

        public static T SetSoundEffectOnHitDescription<T>(this T entity, SoundEffectOnHitDescription value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("soundEffectOnHitDescription", value);
            return entity;
        }

        public static T SetThrowAttackerParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("throwAttackerParticleReference", value);
            return entity;
        }

        public static T SetThrowImpactParticleReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("throwImpactParticleReference", value);
            return entity;
        }

        public static T SetWeaponCategory<T>(this T entity, System.String value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("weaponCategory", value);
            return entity;
        }

        public static T SetWeaponProximity<T>(this T entity, RuleDefinitions.AttackProximity value)
            where T : WeaponTypeDefinition
        {
            entity.SetField("weaponProximity", value);
            return entity;
        }
    }
}