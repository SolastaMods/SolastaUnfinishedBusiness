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
    [TargetType(typeof(GadgetParameterDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GadgetParameterDescriptionExtensions
    {
        public static T SetBoolDefaultValue<T>(this T entity, System.Boolean value)
            where T : GadgetParameterDescription
        {
            entity.SetField("boolDefaultValue", value);
            return entity;
        }

        public static T SetConditionalDisplay<T>(this T entity, System.Boolean value)
            where T : GadgetParameterDescription
        {
            entity.SetField("conditionalDisplay", value);
            return entity;
        }

        public static T SetConditionalMatch<T>(this T entity, System.Boolean value)
            where T : GadgetParameterDescription
        {
            entity.SetField("conditionalMatch", value);
            return entity;
        }

        public static T SetConditionalParameter<T>(this T entity, System.String value)
            where T : GadgetParameterDescription
        {
            entity.SetField("conditionalParameter", value);
            return entity;
        }

        public static T SetFloatDefaultValue<T>(this T entity, System.Single value)
            where T : GadgetParameterDescription
        {
            entity.SetField("floatDefaultValue", value);
            return entity;
        }

        public static T SetGadgetExecutionFilter<T>(this T entity, System.String value)
            where T : GadgetParameterDescription
        {
            entity.SetField("gadgetExecutionFilter", value);
            return entity;
        }

        public static T SetIntDefaultValue<T>(this T entity, System.Int32 value)
            where T : GadgetParameterDescription
        {
            entity.SetField("intDefaultValue", value);
            return entity;
        }

        public static T SetIntMaxValue<T>(this T entity, System.Int32 value)
            where T : GadgetParameterDescription
        {
            entity.SetField("intMaxValue", value);
            return entity;
        }

        public static T SetIntMinValue<T>(this T entity, System.Int32 value)
            where T : GadgetParameterDescription
        {
            entity.SetField("intMinValue", value);
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : GadgetParameterDescription
        {
            entity.SetField("name", value);
            return entity;
        }

        public static T SetRequiresNonEmpty<T>(this T entity, System.Boolean value)
            where T : GadgetParameterDescription
        {
            entity.SetField("requiresNonEmpty", value);
            return entity;
        }

        public static T SetType<T>(this T entity, GadgetBlueprintDefinitions.Type value)
            where T : GadgetParameterDescription
        {
            entity.SetField("type", value);
            return entity;
        }
    }
}