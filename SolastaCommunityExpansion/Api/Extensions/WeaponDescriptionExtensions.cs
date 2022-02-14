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
    [TargetType(typeof(WeaponDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class WeaponDescriptionExtensions
    {
        public static T AddWeaponTags<T>(this T entity,  params  System . String [ ]  value)
            where T : WeaponDescription
        {
            AddWeaponTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWeaponTags<T>(this T entity, IEnumerable<System.String> value)
            where T : WeaponDescription
        {
            entity.WeaponTags.AddRange(value);
            return entity;
        }

        public static T ClearWeaponTags<T>(this T entity)
            where T : WeaponDescription
        {
            entity.WeaponTags.Clear();
            return entity;
        }

        public static T SetAmmunitionType<T>(this T entity, System.String value)
            where T : WeaponDescription
        {
            entity.SetField("ammunitionType", value);
            return entity;
        }

        public static T SetCloseRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("closeRange", value);
            return entity;
        }

        public static T SetEffectDescription<T>(this T entity, EffectDescription value)
            where T : WeaponDescription
        {
            entity.EffectDescription = value;
            return entity;
        }

        public static T SetMaxRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("maxRange", value);
            return entity;
        }

        public static T SetReachRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("reachRange", value);
            return entity;
        }

        public static T SetWeaponTags<T>(this T entity,  params  System . String [ ]  value)
            where T : WeaponDescription
        {
            SetWeaponTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWeaponTags<T>(this T entity, IEnumerable<System.String> value)
            where T : WeaponDescription
        {
            entity.WeaponTags.SetRange(value);
            return entity;
        }

        public static T SetWeaponType<T>(this T entity, System.String value)
            where T : WeaponDescription
        {
            entity.SetField("weaponType", value);
            return entity;
        }
    }
}