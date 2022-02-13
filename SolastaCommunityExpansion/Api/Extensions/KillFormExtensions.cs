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
    [TargetType(typeof(KillForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class KillFormExtensions
    {
        public static KillForm Copy(this KillForm entity)
        {
            var copy = new KillForm();
            copy.Copy(entity);
            return entity;
        }

        public static T SetChallengeRating<T>(this T entity, System.Single value)
            where T : KillForm
        {
            entity.SetField("challengeRating", value);
            return entity;
        }

        public static T SetHitPoints<T>(this T entity, System.Int32 value)
            where T : KillForm
        {
            entity.SetField("hitPoints", value);
            return entity;
        }

        public static T SetKillCondition<T>(this T entity, RuleDefinitions.KillCondition value)
            where T : KillForm
        {
            entity.SetField("killCondition", value);
            return entity;
        }
    }
}