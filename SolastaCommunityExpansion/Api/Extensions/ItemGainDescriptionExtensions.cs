using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
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
    [TargetType(typeof(ItemGainDescription))]
    public static partial class ItemGainDescriptionExtensions
    {
        public static T SetDispatch<T>(this T entity, ItemGainDescription.DispatchType value)
            where T : ItemGainDescription
        {
            entity.Dispatch = value;
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : ItemGainDescription
        {
            entity.ItemDefinition = value;
            return entity;
        }

        public static T SetItemsNumber<T>(this T entity, System.Int32 value)
            where T : ItemGainDescription
        {
            entity.ItemsNumber = value;
            return entity;
        }

        public static T SetLootPackDefinition<T>(this T entity, LootPackDefinition value)
            where T : ItemGainDescription
        {
            entity.LootPackDefinition = value;
            return entity;
        }

        public static T SetPackage<T>(this T entity, ItemGainDescription.PackageType value)
            where T : ItemGainDescription
        {
            entity.Package = value;
            return entity;
        }

        public static T SetRoleName<T>(this T entity, System.String value)
            where T : ItemGainDescription
        {
            entity.RoleName = value;
            return entity;
        }
    }
}