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
    [TargetType(typeof(MoviePlaybackDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class MoviePlaybackDefinitionExtensions
    {
        public static T AddFallbackImages<T>(this T entity,  params  FallbackImageDescription [ ]  value)
            where T : MoviePlaybackDefinition
        {
            AddFallbackImages(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFallbackImages<T>(this T entity, IEnumerable<FallbackImageDescription> value)
            where T : MoviePlaybackDefinition
        {
            entity.FallbackImages.AddRange(value);
            return entity;
        }

        public static T AddSubtitleOccurences<T>(this T entity,  params  SubtitleOccurenceDescription [ ]  value)
            where T : MoviePlaybackDefinition
        {
            AddSubtitleOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSubtitleOccurences<T>(this T entity, IEnumerable<SubtitleOccurenceDescription> value)
            where T : MoviePlaybackDefinition
        {
            entity.SubtitleOccurences.AddRange(value);
            return entity;
        }

        public static T ClearFallbackImages<T>(this T entity)
            where T : MoviePlaybackDefinition
        {
            entity.FallbackImages.Clear();
            return entity;
        }

        public static T ClearSubtitleOccurences<T>(this T entity)
            where T : MoviePlaybackDefinition
        {
            entity.SubtitleOccurences.Clear();
            return entity;
        }

        public static T SetAudioEvent<T>(this T entity, AK.Wwise.Event value)
            where T : MoviePlaybackDefinition
        {
            entity.SetField("audioEvent", value);
            return entity;
        }

        public static T SetFallbackImages<T>(this T entity,  params  FallbackImageDescription [ ]  value)
            where T : MoviePlaybackDefinition
        {
            SetFallbackImages(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFallbackImages<T>(this T entity, IEnumerable<FallbackImageDescription> value)
            where T : MoviePlaybackDefinition
        {
            entity.FallbackImages.SetRange(value);
            return entity;
        }

        public static T SetMovieFilename<T>(this T entity, System.String value)
            where T : MoviePlaybackDefinition
        {
            entity.SetField("movieFilename", value);
            return entity;
        }

        public static T SetSubtitleOccurences<T>(this T entity,  params  SubtitleOccurenceDescription [ ]  value)
            where T : MoviePlaybackDefinition
        {
            SetSubtitleOccurences(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSubtitleOccurences<T>(this T entity, IEnumerable<SubtitleOccurenceDescription> value)
            where T : MoviePlaybackDefinition
        {
            entity.SubtitleOccurences.SetRange(value);
            return entity;
        }

        public static T SetTotalFallbackDuration<T>(this T entity, System.Single value)
            where T : MoviePlaybackDefinition
        {
            entity.SetField("totalFallbackDuration", value);
            return entity;
        }
    }
}