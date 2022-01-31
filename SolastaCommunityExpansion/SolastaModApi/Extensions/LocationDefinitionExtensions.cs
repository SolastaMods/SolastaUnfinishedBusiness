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
    [TargetType(typeof(LocationDefinition))]
    public static partial class LocationDefinitionExtensions
    {
        public static T SetAlwaysHidden<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("alwaysHidden", value);
            return entity;
        }

        public static T SetAudioState<T>(this T entity, AK.Wwise.State value)
            where T : LocationDefinition
        {
            entity.SetField("audioState", value);
            return entity;
        }

        public static T SetCanAttackNonHostileCharacters<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("canAttackNonHostileCharacters", value);
            return entity;
        }

        public static T SetChallengeRating<T>(this T entity, System.Int32 value)
            where T : LocationDefinition
        {
            entity.SetField("challengeRating", value);
            return entity;
        }

        public static T SetFocusWhenRevealed<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("focusWhenRevealed", value);
            return entity;
        }

        public static T SetHasPriorityForTeleporter<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("hasPriorityForTeleporter", value);
            return entity;
        }

        public static T SetIgnoredByScavengers<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("ignoredByScavengers", value);
            return entity;
        }

        public static T SetIsUserLocation<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("isUserLocation", value);
            return entity;
        }

        public static T SetKeepSectorCullingResultsWhenInactive<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("keepSectorCullingResultsWhenInactive", value);
            return entity;
        }

        public static T SetLocationPresentation<T>(this T entity, LocationPresentation value)
            where T : LocationDefinition
        {
            entity.SetField("locationPresentation", value);
            return entity;
        }

        public static T SetRenderSettingsSceneProfile<T>(this T entity, RenderSettingsSceneProfile value)
            where T : LocationDefinition
        {
            entity.RenderSettingsSceneProfile = value;
            return entity;
        }

        public static T SetSceneFilePath<T>(this T entity, System.String value)
            where T : LocationDefinition
        {
            entity.SetField("sceneFilePath", value);
            return entity;
        }

        public static T SetSceneReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : LocationDefinition
        {
            entity.SetField("sceneReference", value);
            return entity;
        }

        public static T SetSetCurrentCampaignNodeWhenLeaving<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("setCurrentCampaignNodeWhenLeaving", value);
            return entity;
        }

        public static T SetStartsWithScavengerCamp<T>(this T entity, System.Boolean value)
            where T : LocationDefinition
        {
            entity.SetField("startsWithScavengerCamp", value);
            return entity;
        }
    }
}