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
    [TargetType(typeof(TooltipFeatureWeaponParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TooltipFeatureWeaponParametersExtensions
    {
        public static T SetAttackModifiersTable<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("attackModifiersTable", value);
            return entity;
        }

        public static T SetDamageModifierGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("damageModifierGroup", value);
            return entity;
        }

        public static T SetDamageModifierValue<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("damageModifierValue", value);
            return entity;
        }

        public static T SetHitModifierGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("hitModifierGroup", value);
            return entity;
        }

        public static T SetHitModifierValue<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("hitModifierValue", value);
            return entity;
        }

        public static T SetMasterTable<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("masterTable", value);
            return entity;
        }

        public static T SetStatsColumns<T>(this T entity, FeatureElementInfoColumn[] value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("statsColumns", value);
            return entity;
        }

        public static T SetStatsGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureWeaponParameters
        {
            entity.SetField("statsGroup", value);
            return entity;
        }
    }
}