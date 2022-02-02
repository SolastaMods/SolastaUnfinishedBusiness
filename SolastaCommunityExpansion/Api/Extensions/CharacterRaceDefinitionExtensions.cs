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
    [TargetType(typeof(CharacterRaceDefinition))]
    public static partial class CharacterRaceDefinitionExtensions
    {
        public static T SetAudioRaceRTPCValue<T>(this T entity, System.Single value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("audioRaceRTPCValue", value);
            return entity;
        }

        public static T SetBaseHeight<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("baseHeight", value);
            return entity;
        }

        public static T SetBaseWeight<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("baseWeight", value);
            return entity;
        }

        public static T SetDefaultAlignement<T>(this T entity, System.String value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("defaultAlignement", value);
            return entity;
        }

        public static T SetDualSex<T>(this T entity, System.Boolean value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("dualSex", value);
            return entity;
        }

        public static T SetInventoryDefinition<T>(this T entity, InventoryDefinition value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("inventoryDefinition", value);
            return entity;
        }

        public static T SetMaximalAge<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("maximalAge", value);
            return entity;
        }

        public static T SetMinimalAge<T>(this T entity, System.Int32 value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("minimalAge", value);
            return entity;
        }

        public static T SetRacePresentation<T>(this T entity, RacePresentation value)
            where T : CharacterRaceDefinition
        {
            entity.SetField("racePresentation", value);
            return entity;
        }

        public static T SetSizeDefinition<T>(this T entity, CharacterSizeDefinition value)
            where T : CharacterRaceDefinition
        {
            entity.SizeDefinition = value;
            return entity;
        }
    }
}