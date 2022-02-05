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
    [TargetType(typeof(FeatureDefinitionAttributeModifier)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionAttributeModifierExtensions
    {
        public static T SetModifiedAttribute<T>(this T entity, System.String value)
            where T : FeatureDefinitionAttributeModifier
        {
            entity.SetField("modifiedAttribute", value);
            return entity;
        }

        public static T SetModifierAbilityScore<T>(this T entity, System.String value)
            where T : FeatureDefinitionAttributeModifier
        {
            entity.SetField("modifierAbilityScore", value);
            return entity;
        }

        public static T SetModifierType2<T>(this T entity, FeatureDefinitionAttributeModifier.AttributeModifierOperation value)
            where T : FeatureDefinitionAttributeModifier
        {
            entity.SetField("modifierType2", value);
            return entity;
        }

        public static T SetModifierValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionAttributeModifier
        {
            entity.SetField("modifierValue", value);
            return entity;
        }

        public static T SetSituationalContext<T>(this T entity, RuleDefinitions.SituationalContext value)
            where T : FeatureDefinitionAttributeModifier
        {
            entity.SetField("situationalContext", value);
            return entity;
        }
    }
}