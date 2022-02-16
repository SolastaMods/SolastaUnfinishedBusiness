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
    [TargetType(typeof(ToolTypeDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ToolTypeDefinitionExtensions
    {
        public static T AddRequiredProficiencyOptions<T>(this T entity,  params  System . String [ ]  value)
            where T : ToolTypeDefinition
        {
            AddRequiredProficiencyOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRequiredProficiencyOptions<T>(this T entity, IEnumerable<System.String> value)
            where T : ToolTypeDefinition
        {
            entity.RequiredProficiencyOptions.AddRange(value);
            return entity;
        }

        public static T ClearRequiredProficiencyOptions<T>(this T entity)
            where T : ToolTypeDefinition
        {
            entity.RequiredProficiencyOptions.Clear();
            return entity;
        }

        public static T SetHasParentCategory<T>(this T entity, System.Boolean value)
            where T : ToolTypeDefinition
        {
            entity.SetField("hasParentCategory", value);
            return entity;
        }

        public static T SetMainAbilityScore<T>(this T entity, System.String value)
            where T : ToolTypeDefinition
        {
            entity.SetField("mainAbilityScore", value);
            return entity;
        }

        public static T SetRequiredProficiencyOptions<T>(this T entity,  params  System . String [ ]  value)
            where T : ToolTypeDefinition
        {
            SetRequiredProficiencyOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRequiredProficiencyOptions<T>(this T entity, IEnumerable<System.String> value)
            where T : ToolTypeDefinition
        {
            entity.RequiredProficiencyOptions.SetRange(value);
            return entity;
        }

        public static T SetToolCategory<T>(this T entity, System.String value)
            where T : ToolTypeDefinition
        {
            entity.SetField("toolCategory", value);
            return entity;
        }
    }
}