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
    [TargetType(typeof(FurnitureBlueprint))]
    public static partial class FurnitureBlueprintExtensions
    {
        public static T SetGroundHighPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("groundHighPlacement", value);
            return entity;
        }

        public static T SetGroundLowPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("groundLowPlacement", value);
            return entity;
        }

        public static T SetGroundPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("groundPlacement", value);
            return entity;
        }

        public static T SetOpeningHighPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("openingHighPlacement", value);
            return entity;
        }

        public static T SetOpeningLowPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("openingLowPlacement", value);
            return entity;
        }

        public static T SetOpeningPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("openingPlacement", value);
            return entity;
        }

        public static T SetPropPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("propPlacement", value);
            return entity;
        }

        public static T SetWallPlacement<T>(this T entity, System.Boolean value)
            where T : FurnitureBlueprint
        {
            entity.SetField("wallPlacement", value);
            return entity;
        }
    }
}