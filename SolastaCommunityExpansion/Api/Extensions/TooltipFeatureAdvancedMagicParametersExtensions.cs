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
    [TargetType(typeof(TooltipFeatureAdvancedMagicParameters)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class TooltipFeatureAdvancedMagicParametersExtensions
    {
        public static T SetAdvancedImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("advancedImage", value);
            return entity;
        }

        public static T SetShapeConeCenteredSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeConeCenteredSprite", value);
            return entity;
        }

        public static T SetShapeCubeCenteredSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeCubeCenteredSprite", value);
            return entity;
        }

        public static T SetShapeCubeCenteredWithOffsetSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeCubeCenteredWithOffsetSprite", value);
            return entity;
        }

        public static T SetShapeCubeSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeCubeSprite", value);
            return entity;
        }

        public static T SetShapeCylinderSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeCylinderSprite", value);
            return entity;
        }

        public static T SetShapeLineCenteredSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeLineCenteredSprite", value);
            return entity;
        }

        public static T SetShapePerceivingWithinDistanceSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapePerceivingWithinDistanceSprite", value);
            return entity;
        }

        public static T SetShapeSphereCenteredSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeSphereCenteredSprite", value);
            return entity;
        }

        public static T SetShapeSphereSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeSphereSprite", value);
            return entity;
        }

        public static T SetShapeWallLineSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeWallLineSprite", value);
            return entity;
        }

        public static T SetShapeWallRingSprite<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : TooltipFeatureAdvancedMagicParameters
        {
            entity.SetField("shapeWallRingSprite", value);
            return entity;
        }
    }
}