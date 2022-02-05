using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
    [TargetType(typeof(RoomBlueprint)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class RoomBlueprintExtensions
    {
        public static T SetCellInfos<T>(this T entity, System.Int32[] value)
            where T : RoomBlueprint
        {
            entity.SetField("cellInfos", value);
            return entity;
        }

        public static T SetEmbeddedGadgets<T>(this T entity, EmbeddedGadgetDescription[] value)
            where T : RoomBlueprint
        {
            entity.SetField("embeddedGadgets", value);
            return entity;
        }

        public static T SetEmbeddedProps<T>(this T entity, EmbeddedPropDescription[] value)
            where T : RoomBlueprint
        {
            entity.SetField("embeddedProps", value);
            return entity;
        }

        public static T SetGroundElevationSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : RoomBlueprint
        {
            entity.SetField("groundElevationSpriteReference", value);
            return entity;
        }

        public static T SetGroundMaskSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : RoomBlueprint
        {
            entity.SetField("groundMaskSpriteReference", value);
            return entity;
        }

        public static T SetSurfaceInfos<T>(this T entity, System.Int32[] value)
            where T : RoomBlueprint
        {
            entity.SetField("surfaceInfos", value);
            return entity;
        }

        public static T SetWallAndOpeningSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : RoomBlueprint
        {
            entity.SetField("wallAndOpeningSpriteReference", value);
            return entity;
        }

        public static T SetWallSpriteReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : RoomBlueprint
        {
            entity.SetField("wallSpriteReference", value);
            return entity;
        }
    }
}