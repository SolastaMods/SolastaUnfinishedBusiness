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
    [TargetType(typeof(GuiEquipmentItem)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GuiEquipmentItemExtensions
    {
        public static T AddEffectForms<T>(this T entity,  params  EffectForm [ ]  value)
            where T : GuiEquipmentItem
        {
            AddEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : GuiEquipmentItem
        {
            entity.EffectForms.AddRange(value);
            return entity;
        }

        public static T AddFunctionDescriptions<T>(this T entity,  params  DeviceFunctionDescription [ ]  value)
            where T : GuiEquipmentItem
        {
            AddFunctionDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFunctionDescriptions<T>(this T entity, IEnumerable<DeviceFunctionDescription> value)
            where T : GuiEquipmentItem
        {
            entity.FunctionDescriptions.AddRange(value);
            return entity;
        }

        public static T AddInterestedFactions<T>(this T entity,  params  System . String [ ]  value)
            where T : GuiEquipmentItem
        {
            AddInterestedFactions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddInterestedFactions<T>(this T entity, IEnumerable<System.String> value)
            where T : GuiEquipmentItem
        {
            entity.InterestedFactions.AddRange(value);
            return entity;
        }

        public static T AddPropertiesList<T>(this T entity,  params  ItemPropertyDescription [ ]  value)
            where T : GuiEquipmentItem
        {
            AddPropertiesList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddPropertiesList<T>(this T entity, IEnumerable<ItemPropertyDescription> value)
            where T : GuiEquipmentItem
        {
            entity.PropertiesList.AddRange(value);
            return entity;
        }

        public static T AddUsableFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : GuiEquipmentItem
        {
            AddUsableFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsableFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : GuiEquipmentItem
        {
            entity.UsableFunctions.AddRange(value);
            return entity;
        }

        public static T ClearEffectForms<T>(this T entity)
            where T : GuiEquipmentItem
        {
            entity.EffectForms.Clear();
            return entity;
        }

        public static T ClearFunctionDescriptions<T>(this T entity)
            where T : GuiEquipmentItem
        {
            entity.FunctionDescriptions.Clear();
            return entity;
        }

        public static T ClearInterestedFactions<T>(this T entity)
            where T : GuiEquipmentItem
        {
            entity.InterestedFactions.Clear();
            return entity;
        }

        public static T ClearPropertiesList<T>(this T entity)
            where T : GuiEquipmentItem
        {
            entity.PropertiesList.Clear();
            return entity;
        }

        public static T ClearUsableFunctions<T>(this T entity)
            where T : GuiEquipmentItem
        {
            entity.UsableFunctions.Clear();
            return entity;
        }

        public static System.Collections.Generic.Dictionary<System.String, TagsDefinitions.Criticity> GetItemTags<T>(this T entity)
            where T : GuiEquipmentItem
        {
            return entity.GetField<System.Collections.Generic.Dictionary<System.String, TagsDefinitions.Criticity>>("itemTags");
        }

        public static System.Collections.Generic.List<ItemPropertyDescription> GetProperties<T>(this T entity)
            where T : GuiEquipmentItem
        {
            return entity.GetField<System.Collections.Generic.List<ItemPropertyDescription>>("properties");
        }

        public static T SetCompareason<T>(this T entity, System.Boolean value)
            where T : GuiEquipmentItem
        {
            entity.SetField("compareason", value);
            return entity;
        }

        public static T SetEffectForms<T>(this T entity,  params  EffectForm [ ]  value)
            where T : GuiEquipmentItem
        {
            SetEffectForms(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEffectForms<T>(this T entity, IEnumerable<EffectForm> value)
            where T : GuiEquipmentItem
        {
            entity.EffectForms.SetRange(value);
            return entity;
        }

        public static T SetEquipmentItem<T>(this T entity, RulesetItem value)
            where T : GuiEquipmentItem
        {
            entity.SetField("equipmentItem", value);
            return entity;
        }

        public static T SetFunctionDescriptions<T>(this T entity,  params  DeviceFunctionDescription [ ]  value)
            where T : GuiEquipmentItem
        {
            SetFunctionDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFunctionDescriptions<T>(this T entity, IEnumerable<DeviceFunctionDescription> value)
            where T : GuiEquipmentItem
        {
            entity.FunctionDescriptions.SetRange(value);
            return entity;
        }

        public static T SetGuiItemDefinition<T>(this T entity, GuiItemDefinition value)
            where T : GuiEquipmentItem
        {
            entity.SetField("guiItemDefinition", value);
            return entity;
        }

        public static T SetInterestedFactions<T>(this T entity,  params  System . String [ ]  value)
            where T : GuiEquipmentItem
        {
            SetInterestedFactions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetInterestedFactions<T>(this T entity, IEnumerable<System.String> value)
            where T : GuiEquipmentItem
        {
            entity.InterestedFactions.SetRange(value);
            return entity;
        }

        public static T SetInventorySlot<T>(this T entity, RulesetInventorySlot value)
            where T : GuiEquipmentItem
        {
            entity.SetField("inventorySlot", value);
            return entity;
        }

        public static T SetPropertiesList<T>(this T entity,  params  ItemPropertyDescription [ ]  value)
            where T : GuiEquipmentItem
        {
            SetPropertiesList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetPropertiesList<T>(this T entity, IEnumerable<ItemPropertyDescription> value)
            where T : GuiEquipmentItem
        {
            entity.PropertiesList.SetRange(value);
            return entity;
        }

        public static T SetUsableFunctions<T>(this T entity,  params  RulesetDeviceFunction [ ]  value)
            where T : GuiEquipmentItem
        {
            SetUsableFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsableFunctions<T>(this T entity, IEnumerable<RulesetDeviceFunction> value)
            where T : GuiEquipmentItem
        {
            entity.UsableFunctions.SetRange(value);
            return entity;
        }
    }
}