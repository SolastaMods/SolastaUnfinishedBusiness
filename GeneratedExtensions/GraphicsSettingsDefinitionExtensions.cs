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
    [TargetType(typeof(GraphicsSettingsDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class GraphicsSettingsDefinitionExtensions
    {
        public static GraphicsSettingsDefinition SetAmbientOcclusion(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.AmbientOcclusion = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetAntialiasingMode(this GraphicsSettingsDefinition entity, UnityEngine.Rendering.PostProcessing.PostProcessLayer.Antialiasing value)
        {
            entity.AntialiasingMode = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetBillboardsCastShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("billboardsCastShadows", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetCampaignAuraQualitySettings(this GraphicsSettingsDefinition entity, Aura2API.AuraQualitySettings[] value)
        {
            entity.SetField("campaignAuraQualitySettings", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetCanCustomizeSettings(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("canCustomizeSettings", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetClothesSimulationQuality(this GraphicsSettingsDefinition entity, GraphicsDefinitions.ClothesSimulationQuality value)
        {
            entity.ClothesSimulationQuality = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetDepthOfField(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.DepthOfField = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetEnablePCSSForDirectional(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("enablePCSSForDirectional", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetExplorationNgssPointSamplingFilter(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("explorationNgssPointSamplingFilter", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetExplorationNgssPointSamplingTest(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("explorationNgssPointSamplingTest", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetExplorationShadowDistances(this GraphicsSettingsDefinition entity, System.Single[] value)
        {
            entity.SetField("explorationShadowDistances", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetFadingFogOfWar(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.FadingFogOfWar = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetGrassCastShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("grassCastShadows", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetInitialShadowDistance(this GraphicsSettingsDefinition entity, System.Single value)
        {
            entity.SetField("initialShadowDistance", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetLightingCutsceneShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("lightingCutsceneShadows", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetLocationAuraQualitySettings(this GraphicsSettingsDefinition entity, Aura2API.AuraQualitySettings[] value)
        {
            entity.SetField("locationAuraQualitySettings", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetLodBias(this GraphicsSettingsDefinition entity, System.Single value)
        {
            entity.SetField("lodBias", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNarrationAuraQualitySettings(this GraphicsSettingsDefinition entity, Aura2API.AuraQualitySettings[] value)
        {
            entity.SetField("narrationAuraQualitySettings", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNarrationShadowDistances(this GraphicsSettingsDefinition entity, System.Single[] value)
        {
            entity.SetField("narrationShadowDistances", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNgssDirectionalSamplingFilter(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("ngssDirectionalSamplingFilter", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNgssDirectionalSamplingTest(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("ngssDirectionalSamplingTest", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNgssPointSamplingFilter(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("ngssPointSamplingFilter", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetNgssPointSamplingTest(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("ngssPointSamplingTest", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetObjectsCastShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("objectsCastShadows", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetParticleRaycastBudget(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("particleRaycastBudget", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetPlantsCastShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("plantsCastShadows", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetRecommendedGraphicsMemorySize(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("recommendedGraphicsMemorySize", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetShadowCastingLightsInPartyLimit(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("shadowCastingLightsInPartyLimit", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetShadowCastingLightsPerCharacterLimit(this GraphicsSettingsDefinition entity, System.Int32 value)
        {
            entity.SetField("shadowCastingLightsPerCharacterLimit", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetShadowDistance(this GraphicsSettingsDefinition entity, GraphicsDefinitions.ShadowDistance value)
        {
            entity.ShadowDistance = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetShadowResolution(this GraphicsSettingsDefinition entity, UnityEngine.ShadowResolution value)
        {
            entity.ShadowResolution = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetShadows(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.Shadows = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetTextureQuality(this GraphicsSettingsDefinition entity, GraphicsDefinitions.TextureQuality value)
        {
            entity.TextureQuality = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetUseDirectionalLightInCutscene(this GraphicsSettingsDefinition entity, System.Boolean value)
        {
            entity.SetField("useDirectionalLightInCutscene", value);
            return entity;
        }

        public static GraphicsSettingsDefinition SetVolumetricLightingQuality(this GraphicsSettingsDefinition entity, GraphicsDefinitions.VolumetricLightingQuality value)
        {
            entity.VolumetricLightingQuality = value;
            return entity;
        }

        public static GraphicsSettingsDefinition SetVsyncCount(this GraphicsSettingsDefinition entity, GraphicsDefinitions.VsyncCount value)
        {
            entity.VsyncCount = value;
            return entity;
        }
    }
}