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
    [TargetType(typeof(TooltipFeaturePowerParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TooltipFeaturePowerParametersExtensions
    {
        public static T SetActivationTimeLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("activationTimeLabel", value);
            return entity;
        }

        public static T SetBottomGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("bottomGroup", value);
            return entity;
        }

        public static T SetDurationLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("durationLabel", value);
            return entity;
        }

        public static T SetRechargeLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("rechargeLabel", value);
            return entity;
        }

        public static T SetSavingThrowHeader<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("savingThrowHeader", value);
            return entity;
        }

        public static T SetSavingThrowLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("savingThrowLabel", value);
            return entity;
        }

        public static T SetUsesLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("usesLabel", value);
            return entity;
        }

        public static T SetVerticalLayout<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeaturePowerParameters
        {
            entity.SetField("verticalLayout", value);
            return entity;
        }
    }
}