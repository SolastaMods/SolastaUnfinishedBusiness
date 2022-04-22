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
    [TargetType(typeof(RulesetItem)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RulesetItemExtensions
    {
        public static T AddDynamicItemProperties<T>(this T entity,  params  RulesetItemProperty [ ]  value)
            where T : RulesetItem
        {
            AddDynamicItemProperties(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDynamicItemProperties<T>(this T entity, IEnumerable<RulesetItemProperty> value)
            where T : RulesetItem
        {
            entity.DynamicItemProperties.AddRange(value);
            return entity;
        }

        public static T ClearDynamicItemProperties<T>(this T entity)
            where T : RulesetItem
        {
            entity.DynamicItemProperties.Clear();
            return entity;
        }

        public static System.Collections.Generic.List<FeatureDefinition> GetAttackModifiers<T>(this T entity)
            where T : RulesetItem
        {
            return entity.GetField<System.Collections.Generic.List<FeatureDefinition>>("attackModifiers");
        }

        public static System.Collections.Generic.List<RulesetItemProperty> GetToRemove<T>(this T entity)
            where T : RulesetItem
        {
            return entity.GetField<System.Collections.Generic.List<RulesetItemProperty>>("toRemove");
        }

        public static T SetAttunedToCharacter<T>(this T entity, System.String value)
            where T : RulesetItem
        {
            entity.AttunedToCharacter = value;
            return entity;
        }

        public static T SetBearerGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetItem
        {
            entity.BearerGuid = value;
            return entity;
        }

        public static T SetDeityMark<T>(this T entity, System.String value)
            where T : RulesetItem
        {
            entity.DeityMark = value;
            return entity;
        }

        public static T SetDynamicItemProperties<T>(this T entity,  params  RulesetItemProperty [ ]  value)
            where T : RulesetItem
        {
            SetDynamicItemProperties(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDynamicItemProperties<T>(this T entity, IEnumerable<RulesetItemProperty> value)
            where T : RulesetItem
        {
            entity.DynamicItemProperties.SetRange(value);
            return entity;
        }

        public static T SetGains<T>(this T entity, System.Int32[] value)
            where T : RulesetItem
        {
            entity.SetField("gains", value);
            return entity;
        }

        public static T SetIdentified<T>(this T entity, System.Boolean value)
            where T : RulesetItem
        {
            entity.Identified = value;
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : RulesetItem
        {
            entity.ItemDefinition = value;
            return entity;
        }

        public static T SetItemDestroyed<T>(this T entity, RulesetItem.ItemDestroyedHandler value)
            where T : RulesetItem
        {
            entity.SetField("<ItemDestroyed>k__BackingField", value);
            return entity;
        }

        public static T SetItemDurationRefreshed<T>(this T entity, RulesetItem.ItemDurationRefreshedHandler value)
            where T : RulesetItem
        {
            entity.SetField("<ItemDurationRefreshed>k__BackingField", value);
            return entity;
        }

        public static T SetItemPropertyRemoved<T>(this T entity, RulesetItem.ItemPropertyRemovedHandler value)
            where T : RulesetItem
        {
            entity.SetField("<ItemPropertyRemoved>k__BackingField", value);
            return entity;
        }

        public static T SetMagicDetected<T>(this T entity, System.Boolean value)
            where T : RulesetItem
        {
            entity.MagicDetected = value;
            return entity;
        }

        public static T SetName<T>(this T entity, System.String value)
            where T : RulesetItem
        {
            entity.Name = value;
            return entity;
        }

        public static T SetOwnerName<T>(this T entity, System.String value)
            where T : RulesetItem
        {
            entity.OwnerName = value;
            return entity;
        }

        public static T SetRulesetLightSource<T>(this T entity, RulesetLightSource value)
            where T : RulesetItem
        {
            entity.RulesetLightSource = value;
            return entity;
        }

        public static T SetSourceSummoningEffectGuid<T>(this T entity, System.UInt64 value)
            where T : RulesetItem
        {
            entity.SourceSummoningEffectGuid = value;
            return entity;
        }
    }
}