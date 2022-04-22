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
    [TargetType(typeof(TooltipFeatureSpellParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TooltipFeatureSpellParametersExtensions
    {
        public static T SetCastingTimeLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("castingTimeLabel", value);
            return entity;
        }

        public static T SetDurationLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("durationLabel", value);
            return entity;
        }

        public static T SetInvalidColor<T>(this T entity, UnityEngine.Color value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("invalidColor", value);
            return entity;
        }

        public static T SetMaterialComponentMarker<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("materialComponentMarker", value);
            return entity;
        }

        public static T SetMaterialSpecificLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("materialSpecificLabel", value);
            return entity;
        }

        public static T SetSavingThrowHeader<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("savingThrowHeader", value);
            return entity;
        }

        public static T SetSavingThrowLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("savingThrowLabel", value);
            return entity;
        }

        public static T SetSomaticComponentMarker<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("somaticComponentMarker", value);
            return entity;
        }

        public static T SetSpecificComponentMarker<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("specificComponentMarker", value);
            return entity;
        }

        public static T SetValidColor<T>(this T entity, UnityEngine.Color value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("validColor", value);
            return entity;
        }

        public static T SetVerbalComponentMarker<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("verbalComponentMarker", value);
            return entity;
        }

        public static T SetVerticalLayout<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureSpellParameters
        {
            entity.SetField("verticalLayout", value);
            return entity;
        }
    }
}