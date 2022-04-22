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
    [TargetType(typeof(FeatureDefinitionFeatureSet)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FeatureDefinitionFeatureSetExtensions
    {
        public static T AddFeatureSet<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : FeatureDefinitionFeatureSet
        {
            AddFeatureSet(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureSet<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.FeatureSet.AddRange(value);
            return entity;
        }

        public static T ClearFeatureSet<T>(this T entity)
            where T : FeatureDefinitionFeatureSet
        {
            entity.FeatureSet.Clear();
            return entity;
        }

        public static T SetDefaultSelection<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("defaultSelection", value);
            return entity;
        }

        public static T SetEnumerateInDescription<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("enumerateInDescription", value);
            return entity;
        }

        public static T SetFeatureSet<T>(this T entity,  params  FeatureDefinition [ ]  value)
            where T : FeatureDefinitionFeatureSet
        {
            SetFeatureSet(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureSet<T>(this T entity, IEnumerable<FeatureDefinition> value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.FeatureSet.SetRange(value);
            return entity;
        }

        public static T SetHasRacialAffinity<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("hasRacialAffinity", value);
            return entity;
        }

        public static T SetMode<T>(this T entity, FeatureDefinitionFeatureSet.FeatureSetMode value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("mode", value);
            return entity;
        }

        public static T SetUniqueChoices<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionFeatureSet
        {
            entity.SetField("uniqueChoices", value);
            return entity;
        }
    }
}