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
    [TargetType(typeof(MerchantDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MerchantDefinitionExtensions
    {
        public static T AddStockUnitDescriptions<T>(this T entity,  params  StockUnitDescription [ ]  value)
            where T : MerchantDefinition
        {
            AddStockUnitDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddStockUnitDescriptions<T>(this T entity, IEnumerable<StockUnitDescription> value)
            where T : MerchantDefinition
        {
            entity.StockUnitDescriptions.AddRange(value);
            return entity;
        }

        public static T ClearStockUnitDescriptions<T>(this T entity)
            where T : MerchantDefinition
        {
            entity.StockUnitDescriptions.Clear();
            return entity;
        }

        public static T SetBuyBackPercent<T>(this T entity, System.Int32 value)
            where T : MerchantDefinition
        {
            entity.BuyBackPercent = value;
            return entity;
        }

        public static T SetCanDetectMagic<T>(this T entity, System.Boolean value)
            where T : MerchantDefinition
        {
            entity.CanDetectMagic = value;
            return entity;
        }

        public static T SetCanIdentify<T>(this T entity, System.Boolean value)
            where T : MerchantDefinition
        {
            entity.CanIdentify = value;
            return entity;
        }

        public static T SetDetectMagicCostGp<T>(this T entity, System.Int32 value)
            where T : MerchantDefinition
        {
            entity.DetectMagicCostGp = value;
            return entity;
        }

        public static T SetExposeFaction<T>(this T entity, System.Boolean value)
            where T : MerchantDefinition
        {
            entity.SetField("exposeFaction", value);
            return entity;
        }

        public static T SetFactionAffinity<T>(this T entity, System.String value)
            where T : MerchantDefinition
        {
            entity.SetField("factionAffinity", value);
            return entity;
        }

        public static T SetIdentifyCostGp<T>(this T entity, System.Int32 value)
            where T : MerchantDefinition
        {
            entity.IdentifyCostGp = value;
            return entity;
        }

        public static T SetInDungeonEditor<T>(this T entity, System.Boolean value)
            where T : MerchantDefinition
        {
            entity.InDungeonEditor = value;
            return entity;
        }

        public static T SetOverchargePercent<T>(this T entity, System.Int32 value)
            where T : MerchantDefinition
        {
            entity.OverchargePercent = value;
            return entity;
        }

        public static T SetStockUnitDescriptions<T>(this T entity,  params  StockUnitDescription [ ]  value)
            where T : MerchantDefinition
        {
            SetStockUnitDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetStockUnitDescriptions<T>(this T entity, IEnumerable<StockUnitDescription> value)
            where T : MerchantDefinition
        {
            entity.StockUnitDescriptions.SetRange(value);
            return entity;
        }

        public static T SetUserMerchantInventory<T>(this T entity, System.Boolean value)
            where T : MerchantDefinition
        {
            entity.SetField("<UserMerchantInventory>k__BackingField", value);
            return entity;
        }
    }
}