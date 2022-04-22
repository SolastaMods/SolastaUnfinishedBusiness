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
    [TargetType(typeof(InventoryOperationDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class InventoryOperationDescriptionExtensions
    {
        public static T SetCampaignDefinitionName<T>(this T entity, System.String value)
            where T : InventoryOperationDescription
        {
            entity.CampaignDefinitionName = value;
            return entity;
        }

        public static T SetGpValue<T>(this T entity, System.Int32 value)
            where T : InventoryOperationDescription
        {
            entity.GpValue = value;
            return entity;
        }

        public static T SetGrantOrLoseQuantity<T>(this T entity, System.Int32 value)
            where T : InventoryOperationDescription
        {
            entity.GrantOrLoseQuantity = value;
            return entity;
        }

        public static T SetItemDefinitionName<T>(this T entity, System.String value)
            where T : InventoryOperationDescription
        {
            entity.ItemDefinitionName = value;
            return entity;
        }

        public static T SetOverrideQuantity<T>(this T entity, System.Boolean value)
            where T : InventoryOperationDescription
        {
            entity.OverrideQuantity = value;
            return entity;
        }

        public static T SetRoleNumber<T>(this T entity, System.Int32 value)
            where T : InventoryOperationDescription
        {
            entity.RoleNumber = value;
            return entity;
        }

        public static T SetTarget<T>(this T entity, InventoryOperationDescription.OperationTarget value)
            where T : InventoryOperationDescription
        {
            entity.Target = value;
            return entity;
        }

        public static T SetType<T>(this T entity, System.String value)
            where T : InventoryOperationDescription
        {
            entity.Type = value;
            return entity;
        }
    }
}