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
    [TargetType(typeof(AnimationEventParameters))]
    public static partial class AnimationEventParametersExtensions
    {
        public static T SetAudioEvent<T>(this T entity, AK.Wwise.Event value)
            where T : AnimationEventParameters
        {
            entity.SetField("audioEvent", value);
            return entity;
        }

        public static T SetCharacterEvent<T>(this T entity, GameLocationCharacterEventSystem.Event value)
            where T : AnimationEventParameters
        {
            entity.SetField("characterEvent", value);
            return entity;
        }

        public static T SetDirtFootstepParticlePrefab<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : AnimationEventParameters
        {
            entity.SetField("dirtFootstepParticlePrefab", value);
            return entity;
        }

        public static T SetIgnoreBlendRestriction<T>(this T entity, System.Boolean value)
            where T : AnimationEventParameters
        {
            entity.SetField("ignoreBlendRestriction", value);
            return entity;
        }

        public static T SetMudFootstepParticlePrefab<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : AnimationEventParameters
        {
            entity.SetField("mudFootstepParticlePrefab", value);
            return entity;
        }

        public static T SetParticlePrefab<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : AnimationEventParameters
        {
            entity.SetField("particlePrefab", value);
            return entity;
        }

        public static T SetPreferredCharacterAudioNode<T>(this T entity, AudioCharacterNode.Type value)
            where T : AnimationEventParameters
        {
            entity.SetField("preferredCharacterAudioNode", value);
            return entity;
        }

        public static T SetPreferredGadgetAudioNode<T>(this T entity, AudioGadgetNode.Type value)
            where T : AnimationEventParameters
        {
            entity.SetField("preferredGadgetAudioNode", value);
            return entity;
        }

        public static T SetWaterThighFootstepParticlePrefab<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : AnimationEventParameters
        {
            entity.SetField("waterThighFootstepParticlePrefab", value);
            return entity;
        }
    }
}