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
    [TargetType(typeof(RulesetItemDevice)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetItemDeviceExtensions
    {
        public static T AddUsableFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : RulesetItemDevice
        {
            AddUsableFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsableFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : RulesetItemDevice
        {
            entity.UsableFunctions.AddRange(value);
            return entity;
        }

        public static T ClearUsableFunctions<T>(this T entity)
            where T : RulesetItemDevice
        {
            entity.UsableFunctions.Clear();
            return entity;
        }

        public static T SetUsableFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : RulesetItemDevice
        {
            SetUsableFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsableFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : RulesetItemDevice
        {
            entity.UsableFunctions.SetRange(value);
            return entity;
        }
    }
}