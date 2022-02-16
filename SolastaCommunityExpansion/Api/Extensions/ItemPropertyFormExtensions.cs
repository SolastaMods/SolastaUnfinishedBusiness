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
    [TargetType(typeof(ItemPropertyForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ItemPropertyFormExtensions
    {
        public static T AddFeatureBySlotLevel<T>(this T entity,  params  FeatureUnlockByLevel [ ]  value)
            where T : ItemPropertyForm
        {
            AddFeatureBySlotLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFeatureBySlotLevel<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.AddRange(value);
            return entity;
        }

        public static T ClearFeatureBySlotLevel<T>(this T entity)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.Clear();
            return entity;
        }

        public static ItemPropertyForm Copy(this ItemPropertyForm entity)
        {
            var copy = new ItemPropertyForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetFeatureBySlotLevel<T>(this T entity,  params  FeatureUnlockByLevel [ ]  value)
            where T : ItemPropertyForm
        {
            SetFeatureBySlotLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFeatureBySlotLevel<T>(this T entity, IEnumerable<FeatureUnlockByLevel> value)
            where T : ItemPropertyForm
        {
            entity.FeatureBySlotLevel.SetRange(value);
            return entity;
        }

        public static T SetUsageLimitation<T>(this T entity, RuleDefinitions.ItemPropertyUsage value)
            where T : ItemPropertyForm
        {
            entity.SetField("usageLimitation", value);
            return entity;
        }

        public static T SetUseAmount<T>(this T entity, System.Int32 value)
            where T : ItemPropertyForm
        {
            entity.SetField("useAmount", value);
            return entity;
        }
    }
}