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
    [TargetType(typeof(FeatDefinition))]
    public static partial class FeatDefinitionExtensions
    {
        public static T SetArmorProficiencyCategory<T>(this T entity, System.String value)
            where T : FeatDefinition
        {
            entity.SetField("armorProficiencyCategory", value);
            return entity;
        }

        public static T SetArmorProficiencyPrerequisite<T>(this T entity, System.Boolean value)
            where T : FeatDefinition
        {
            entity.SetField("armorProficiencyPrerequisite", value);
            return entity;
        }

        public static T SetMinimalAbilityScoreName<T>(this T entity, System.String value)
            where T : FeatDefinition
        {
            entity.SetField("minimalAbilityScoreName", value);
            return entity;
        }

        public static T SetMinimalAbilityScorePrerequisite<T>(this T entity, System.Boolean value)
            where T : FeatDefinition
        {
            entity.SetField("minimalAbilityScorePrerequisite", value);
            return entity;
        }

        public static T SetMinimalAbilityScoreValue<T>(this T entity, System.Int32 value)
            where T : FeatDefinition
        {
            entity.SetField("minimalAbilityScoreValue", value);
            return entity;
        }

        public static T SetMustCastSpellsPrerequisite<T>(this T entity, System.Boolean value)
            where T : FeatDefinition
        {
            entity.SetField("mustCastSpellsPrerequisite", value);
            return entity;
        }
    }
}