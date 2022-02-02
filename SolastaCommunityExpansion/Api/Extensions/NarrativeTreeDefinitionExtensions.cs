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
    [TargetType(typeof(NarrativeTreeDefinition))]
    public static partial class NarrativeTreeDefinitionExtensions
    {
        public static T SetAvailableCameraShotNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.AvailableCameraShotNames = value;
            return entity;
        }

        public static T SetAvailableCameraTargetNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.AvailableCameraTargetNames = value;
            return entity;
        }

        public static T SetGeneratedCameraShotNames<T>(this T entity, System.String[] value)
            where T : NarrativeTreeDefinition
        {
            entity.GeneratedCameraShotNames = value;
            return entity;
        }

        public static T SetHasSpecialCutsceneLightingForCharacters<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.HasSpecialCutsceneLightingForCharacters = value;
            return entity;
        }

        public static T SetIsUserDialog<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.SetField("<IsUserDialog>k__BackingField", value);
            return entity;
        }

        public static T SetNarrativeCameraSetupGUID<T>(this T entity, System.String value)
            where T : NarrativeTreeDefinition
        {
            entity.NarrativeCameraSetupGUID = value;
            return entity;
        }

        public static T SetSerializeVersion<T>(this T entity, System.Int32 value)
            where T : NarrativeTreeDefinition
        {
            entity.SerializeVersion = value;
            return entity;
        }

        public static T SetSkippable<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.Skippable = value;
            return entity;
        }

        public static T SetUnequipWieldedItems<T>(this T entity, System.Boolean value)
            where T : NarrativeTreeDefinition
        {
            entity.UnequipWieldedItems = value;
            return entity;
        }
    }
}