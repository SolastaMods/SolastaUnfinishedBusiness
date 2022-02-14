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
    [TargetType(typeof(FurnitureBlueprint)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class FurnitureBlueprintExtensions
    {
        public static T AddLightSources<T>(this T entity,  params  UnityEngine . Vector2 [ ]  value)
            where T : FurnitureBlueprint
        {
            AddLightSources(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddLightSources<T>(this T entity, IEnumerable<UnityEngine.Vector2> value)
            where T : FurnitureBlueprint
        {
            entity.LightSources.AddRange(value);
            return entity;
        }

        public static T ClearLightSources<T>(this T entity)
            where T : FurnitureBlueprint
        {
            entity.LightSources.Clear();
            return entity;
        }

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

        public static T SetLightSources<T>(this T entity,  params  UnityEngine . Vector2 [ ]  value)
            where T : FurnitureBlueprint
        {
            SetLightSources(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetLightSources<T>(this T entity, IEnumerable<UnityEngine.Vector2> value)
            where T : FurnitureBlueprint
        {
            entity.LightSources.SetRange(value);
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