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
    [TargetType(typeof(RecordTableDefinition))]
    public static partial class RecordTableDefinitionExtensions
    {
        public static T SetAreaWidth<T>(this T entity, System.Single value)
            where T : RecordTableDefinition
        {
            entity.SetField("areaWidth", value);
            return entity;
        }

        public static T SetLayout<T>(this T entity, RecordTableDefinition.LayoutType value)
            where T : RecordTableDefinition
        {
            entity.SetField("layout", value);
            return entity;
        }

        public static T SetMaxEntries<T>(this T entity, System.Int32 value)
            where T : RecordTableDefinition
        {
            entity.SetField("maxEntries", value);
            return entity;
        }

        public static T SetMaxSerializedEntries<T>(this T entity, System.Int32 value)
            where T : RecordTableDefinition
        {
            entity.SetField("maxSerializedEntries", value);
            return entity;
        }

        public static T SetOffsetX<T>(this T entity, System.Single value)
            where T : RecordTableDefinition
        {
            entity.SetField("offsetX", value);
            return entity;
        }

        public static T SetOffsetY<T>(this T entity, System.Single value)
            where T : RecordTableDefinition
        {
            entity.SetField("offsetY", value);
            return entity;
        }

        public static T SetSpacing<T>(this T entity, System.Single value)
            where T : RecordTableDefinition
        {
            entity.SetField("spacing", value);
            return entity;
        }
    }
}