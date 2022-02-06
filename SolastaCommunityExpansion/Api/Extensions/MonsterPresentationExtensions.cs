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
    [TargetType(typeof(MonsterPresentation)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MonsterPresentationExtensions
    {
        public static T SetAttachedParticlesReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentation
        {
            entity.SetField("attachedParticlesReference", value);
            return entity;
        }

        public static T SetBestiaryAttachedParticlesReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentation
        {
            entity.SetField("bestiaryAttachedParticlesReference", value);
            return entity;
        }

        public static T SetCanGeneratePortrait<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("canGeneratePortrait", value);
            return entity;
        }

        public static T SetCustomMaterials<T>(this T entity, UnityEngine.AddressableAssets.AssetReference[] value)
            where T : MonsterPresentation
        {
            entity.SetField("customMaterials", value);
            return entity;
        }

        public static T SetCustomShaderReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentation
        {
            entity.SetField("customShaderReference", value);
            return entity;
        }

        public static T SetFemaleModelScale<T>(this T entity, System.Single value)
            where T : MonsterPresentation
        {
            entity.SetField("femaleModelScale", value);
            return entity;
        }

        public static T SetFemalePrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentation
        {
            entity.SetField("femalePrefabReference", value);
            return entity;
        }

        public static T SetFirstCharacterShaderColor<T>(this T entity, UnityEngine.Color value)
            where T : MonsterPresentation
        {
            entity.SetField("firstCharacterShaderColor", value);
            return entity;
        }

        public static T SetHasLightingCutscene<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hasLightingCutscene", value);
            return entity;
        }

        public static T SetHasMonsterPortraitBackground<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hasMonsterPortraitBackground", value);
            return entity;
        }

        public static T SetHasPhantomDistortion<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hasPhantomDistortion", value);
            return entity;
        }

        public static T SetHasPrefabVariants<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hasPrefabVariants", value);
            return entity;
        }

        public static T SetHasUniqueName<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hasUniqueName", value);
            return entity;
        }

        public static T SetHideDuringCutscene<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("hideDuringCutscene", value);
            return entity;
        }

        public static T SetHumanoidMonsterPresentationDefinitions<T>(this T entity, HumanoidMonsterPresentationDefinition[] value)
            where T : MonsterPresentation
        {
            entity.SetField("humanoidMonsterPresentationDefinitions", value);
            return entity;
        }

        public static T SetMaleModelScale<T>(this T entity, System.Single value)
            where T : MonsterPresentation
        {
            entity.SetField("maleModelScale", value);
            return entity;
        }

        public static T SetMalePrefabReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterPresentation
        {
            entity.SetField("malePrefabReference", value);
            return entity;
        }

        public static T SetMonsterPresentationDefinitions<T>(this T entity, MonsterPresentationDefinition[] value)
            where T : MonsterPresentation
        {
            entity.SetField("monsterPresentationDefinitions", value);
            return entity;
        }

        public static T SetMutantFleshDirection<T>(this T entity, UnityEngine.Vector3 value)
            where T : MonsterPresentation
        {
            entity.SetField("mutantFleshDirection", value);
            return entity;
        }

        public static T SetNeedMerchantPortrait<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("needMerchantPortrait", value);
            return entity;
        }

        public static T SetOverrideCharacterShaderColors<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("overrideCharacterShaderColors", value);
            return entity;
        }

        public static T SetPortraitCameraFollowOffset<T>(this T entity, UnityEngine.Vector3 value)
            where T : MonsterPresentation
        {
            entity.SetField("portraitCameraFollowOffset", value);
            return entity;
        }

        public static T SetPortraitCameraFOV<T>(this T entity, System.Single value)
            where T : MonsterPresentation
        {
            entity.SetField("portraitCameraFOV", value);
            return entity;
        }

        public static T SetPortraitCameraLightingOffset<T>(this T entity, UnityEngine.Vector3 value)
            where T : MonsterPresentation
        {
            entity.SetField("portraitCameraLightingOffset", value);
            return entity;
        }

        public static T SetPortraitCameraLookAtScreenOffset<T>(this T entity, UnityEngine.Vector2 value)
            where T : MonsterPresentation
        {
            entity.SetField("portraitCameraLookAtScreenOffset", value);
            return entity;
        }

        public static T SetSecondCharacterShaderColor<T>(this T entity, UnityEngine.Color value)
            where T : MonsterPresentation
        {
            entity.SetField("secondCharacterShaderColor", value);
            return entity;
        }

        public static T SetUniqueNameTitle<T>(this T entity, System.String value)
            where T : MonsterPresentation
        {
            entity.SetField("uniqueNameTitle", value);
            return entity;
        }

        public static T SetUseCustomMaterials<T>(this T entity, System.Boolean value)
            where T : MonsterPresentation
        {
            entity.SetField("useCustomMaterials", value);
            return entity;
        }

        public static T SetWieldedItemsScale<T>(this T entity, System.Single value)
            where T : MonsterPresentation
        {
            entity.SetField("wieldedItemsScale", value);
            return entity;
        }
    }
}