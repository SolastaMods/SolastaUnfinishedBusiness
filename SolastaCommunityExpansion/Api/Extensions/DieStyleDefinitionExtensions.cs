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
    [TargetType(typeof(DieStyleDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DieStyleDefinitionExtensions
    {
        public static T SetD10MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d10MaterialReference", value);
            return entity;
        }

        public static T SetD12MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d12MaterialReference", value);
            return entity;
        }

        public static T SetD20MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d20MaterialReference", value);
            return entity;
        }

        public static T SetD4MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d4MaterialReference", value);
            return entity;
        }

        public static T SetD6MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d6MaterialReference", value);
            return entity;
        }

        public static T SetD8MaterialReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("d8MaterialReference", value);
            return entity;
        }

        public static T SetParticlePrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : DieStyleDefinition
        {
            entity.SetField("particlePrefabReference", value);
            return entity;
        }

        public static T SetUseMetallicLayer<T>(this T entity, System.Boolean value)
            where T : DieStyleDefinition
        {
            entity.SetField("useMetallicLayer", value);
            return entity;
        }
    }
}