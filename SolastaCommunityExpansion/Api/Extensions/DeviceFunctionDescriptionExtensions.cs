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
    [TargetType(typeof(DeviceFunctionDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DeviceFunctionDescriptionExtensions
    {
        public static T SetCanOverchargeSpell<T>(this T entity, System.Boolean value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("canOverchargeSpell", value);
            return entity;
        }

        public static T SetDurationType<T>(this T entity, RuleDefinitions.DurationType value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("durationType", value);
            return entity;
        }

        public static T SetFeatureDefinitionPower<T>(this T entity, FeatureDefinitionPower value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("featureDefinitionPower", value);
            return entity;
        }

        public static T SetParentUsage<T>(this T entity, EquipmentDefinitions.ItemUsage value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("parentUsage", value);
            return entity;
        }

        public static T SetRechargeRate<T>(this T entity, RuleDefinitions.RechargeRate value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("rechargeRate", value);
            return entity;
        }

        public static T SetSpellDefinition<T>(this T entity, SpellDefinition value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("spellDefinition", value);
            return entity;
        }

        public static T SetType<T>(this T entity, DeviceFunctionDescription.FunctionType value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("type", value);
            return entity;
        }

        public static T SetUseAffinity<T>(this T entity, DeviceFunctionDescription.FunctionUseAffinity value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("useAffinity", value);
            return entity;
        }

        public static T SetUseAmount<T>(this T entity, System.Int32 value)
            where T : DeviceFunctionDescription
        {
            entity.SetField("useAmount", value);
            return entity;
        }
    }
}