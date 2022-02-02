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
    [TargetType(typeof(CampaignDefinition))]
    public static partial class CampaignDefinitionExtensions
    {
        public static T SetCalendar<T>(this T entity, CalendarDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("calendar", value);
            return entity;
        }

        public static T SetCanSkipIntro<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("canSkipIntro", value);
            return entity;
        }

        public static T SetConclusionMovieDefinition<T>(this T entity, MoviePlaybackDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("conclusionMovieDefinition", value);
            return entity;
        }

        public static T SetEditorOnly<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("editorOnly", value);
            return entity;
        }

        public static T SetFastTimeScaleDuringTravel<T>(this T entity, System.Single value)
            where T : CampaignDefinition
        {
            entity.SetField("fastTimeScaleDuringTravel", value);
            return entity;
        }

        public static T SetGraphicsCampaignMapReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : CampaignDefinition
        {
            entity.SetField("graphicsCampaignMapReference", value);
            return entity;
        }

        public static T SetInitialPartyPosition<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("initialPartyPosition", value);
            return entity;
        }

        public static T SetInsideLocation<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("insideLocation", value);
            return entity;
        }

        public static T SetIntroMovieDefinition<T>(this T entity, MoviePlaybackDefinition value)
            where T : CampaignDefinition
        {
            entity.SetField("introMovieDefinition", value);
            return entity;
        }

        public static T SetIsUserCampaign<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("isUserCampaign", value);
            return entity;
        }

        public static T SetJournalStart<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("journalStart", value);
            return entity;
        }

        public static T SetLevelCap<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("levelCap", value);
            return entity;
        }

        public static T SetLocationProfile<T>(this T entity, UnityEngine.Rendering.PostProcessing.PostProcessProfile value)
            where T : CampaignDefinition
        {
            entity.SetField("locationProfile", value);
            return entity;
        }

        public static T SetMaxStartLevel<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("maxStartLevel", value);
            return entity;
        }

        public static T SetMinStartLevel<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("minStartLevel", value);
            return entity;
        }

        public static T SetMultiplayer<T>(this T entity, System.Boolean value)
            where T : CampaignDefinition
        {
            entity.SetField("multiplayer", value);
            return entity;
        }

        public static T SetNormalTimeScaleDuringTravel<T>(this T entity, System.Single value)
            where T : CampaignDefinition
        {
            entity.SetField("normalTimeScaleDuringTravel", value);
            return entity;
        }

        public static T SetPartySize<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("partySize", value);
            return entity;
        }

        public static T SetRenderSettingsSceneProfile<T>(this T entity, RenderSettingsSceneProfile value)
            where T : CampaignDefinition
        {
            entity.SetField("renderSettingsSceneProfile", value);
            return entity;
        }

        public static T SetSceneFilePath<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("sceneFilePath", value);
            return entity;
        }

        public static T SetSceneReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : CampaignDefinition
        {
            entity.SetField("sceneReference", value);
            return entity;
        }

        public static T SetSkipIntroEntrance<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("skipIntroEntrance", value);
            return entity;
        }

        public static T SetSkipIntroLocation<T>(this T entity, System.String value)
            where T : CampaignDefinition
        {
            entity.SetField("skipIntroLocation", value);
            return entity;
        }

        public static T SetStartDay<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startDay", value);
            return entity;
        }

        public static T SetStartHour<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startHour", value);
            return entity;
        }

        public static T SetStartMonth<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startMonth", value);
            return entity;
        }

        public static T SetStartYear<T>(this T entity, System.Int32 value)
            where T : CampaignDefinition
        {
            entity.SetField("startYear", value);
            return entity;
        }
    }
}