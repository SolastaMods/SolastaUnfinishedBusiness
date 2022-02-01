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
    [TargetType(typeof(FeatureDefinitionDieRollModifier))]
    public static partial class FeatureDefinitionDieRollModifierExtensions
    {
        public static T SetForcedMinimalHalfDamageOnDice<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("forcedMinimalHalfDamageOnDice", value);
            return entity;
        }

        public static T SetMaxRollValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("maxRollValue", value);
            return entity;
        }

        public static T SetMinRerollValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("minRerollValue", value);
            return entity;
        }

        public static T SetMinRollValue<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("minRollValue", value);
            return entity;
        }

        public static T SetRequireProficiency<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("requireProficiency", value);
            return entity;
        }

        public static T SetRerollCount<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("rerollCount", value);
            return entity;
        }

        public static T SetRerollLocalizationKey<T>(this T entity, System.String value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("rerollLocalizationKey", value);
            return entity;
        }

        public static T SetValidityContext<T>(this T entity, RuleDefinitions.RollContext value)
            where T : FeatureDefinitionDieRollModifier
        {
            entity.SetField("validityContext", value);
            return entity;
        }
    }
}