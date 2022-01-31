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
    [TargetType(typeof(MonsterPresentationDefinition))]
    public static partial class MonsterPresentationDefinitionExtensions
    {
        public static T SetCustomMaterials<T>(this T entity, UnityEngine.AddressableAssets.AssetReference[] value)
            where T : MonsterPresentationDefinition
        {
            entity.SetField("customMaterials", value);
            return entity;
        }

        public static T SetModelScale<T>(this T entity, System.Single value)
            where T : MonsterPresentationDefinition
        {
            entity.SetField("modelScale", value);
            return entity;
        }

        public static T SetPrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentationDefinition
        {
            entity.SetField("prefabReference", value);
            return entity;
        }

        public static T SetSex<T>(this T entity, RuleDefinitions.CreatureSex value)
            where T : MonsterPresentationDefinition
        {
            entity.SetField("sex", value);
            return entity;
        }

        public static T SetUseCustomMaterials<T>(this T entity, System.Boolean value)
            where T : MonsterPresentationDefinition
        {
            entity.SetField("useCustomMaterials", value);
            return entity;
        }
    }
}