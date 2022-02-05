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
    [TargetType(typeof(TooltipFeatureDeviceParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TooltipFeatureDeviceParametersExtensions
    {
        public static T SetAttunementLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureDeviceParameters
        {
            entity.SetField("attunementLabel", value);
            return entity;
        }

        public static T SetRechargeHeader<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureDeviceParameters
        {
            entity.SetField("rechargeHeader", value);
            return entity;
        }

        public static T SetRechargeLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureDeviceParameters
        {
            entity.SetField("rechargeLabel", value);
            return entity;
        }

        public static T SetUsageGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureDeviceParameters
        {
            entity.SetField("usageGroup", value);
            return entity;
        }

        public static T SetUsageLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureDeviceParameters
        {
            entity.SetField("usageLabel", value);
            return entity;
        }
    }
}