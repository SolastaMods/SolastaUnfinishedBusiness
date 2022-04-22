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
    [TargetType(typeof(BanterEventDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class BanterEventDefinitionExtensions
    {
        public static T AddEventVariants<T>(this T entity,  params  BanterDefinitions . EventVariant [ ]  value)
            where T : BanterEventDefinition
        {
            AddEventVariants(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddEventVariants<T>(this T entity, IEnumerable<BanterDefinitions.EventVariant> value)
            where T : BanterEventDefinition
        {
            entity.EventVariants.AddRange(value);
            return entity;
        }

        public static T ClearEventVariants<T>(this T entity)
            where T : BanterEventDefinition
        {
            entity.EventVariants.Clear();
            return entity;
        }

        public static T SetCanUseWhileCautious<T>(this T entity, System.Boolean value)
            where T : BanterEventDefinition
        {
            entity.SetField("canUseWhileCautious", value);
            return entity;
        }

        public static T SetEventProbability<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("eventProbability", value);
            return entity;
        }

        public static T SetEventTrigger<T>(this T entity, System.String value)
            where T : BanterEventDefinition
        {
            entity.SetField("eventTrigger", value);
            return entity;
        }

        public static T SetEventVariants<T>(this T entity,  params  BanterDefinitions . EventVariant [ ]  value)
            where T : BanterEventDefinition
        {
            SetEventVariants(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetEventVariants<T>(this T entity, IEnumerable<BanterDefinitions.EventVariant> value)
            where T : BanterEventDefinition
        {
            entity.EventVariants.SetRange(value);
            return entity;
        }

        public static T SetPlaybackDelay<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("playbackDelay", value);
            return entity;
        }

        public static T SetSearchKey<T>(this T entity, System.String value)
            where T : BanterEventDefinition
        {
            entity.SetField("searchKey", value);
            return entity;
        }

        public static T SetSelfProbability<T>(this T entity, System.Single value)
            where T : BanterEventDefinition
        {
            entity.SetField("selfProbability", value);
            return entity;
        }
    }
}