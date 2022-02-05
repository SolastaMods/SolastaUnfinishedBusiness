using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
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
    [TargetType(typeof(CharacterClassDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterClassDefinitionExtensions
    {
        public static T SetClassAnimationId<T>(this T entity, AnimationDefinitions.ClassAnimationId value)
            where T : CharacterClassDefinition
        {
            entity.SetField("classAnimationId", value);
            return entity;
        }

        public static T SetClassPictogramReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReferenceSprite value)
            where T : CharacterClassDefinition
        {
            entity.SetField("classPictogramReference", value);
            return entity;
        }

        public static T SetDefaultBattleDecisions<T>(this T entity, TA.AI.DecisionPackageDefinition value)
            where T : CharacterClassDefinition
        {
            entity.SetField("defaultBattleDecisions", value);
            return entity;
        }

        public static T SetHitDice<T>(this T entity, RuleDefinitions.DieType value)
            where T : CharacterClassDefinition
        {
            entity.SetField("hitDice", value);
            return entity;
        }

        public static T SetIngredientGatheringOdds<T>(this T entity, System.Int32 value)
            where T : CharacterClassDefinition
        {
            entity.SetField("ingredientGatheringOdds", value);
            return entity;
        }

        public static T SetRequiresDeity<T>(this T entity, System.Boolean value)
            where T : CharacterClassDefinition
        {
            entity.SetField("requiresDeity", value);
            return entity;
        }

        public static T SetVocalSpellSemeClass<T>(this T entity, RuleDefinitions.VocalSpellSemeClass value)
            where T : CharacterClassDefinition
        {
            entity.SetField("vocalSpellSemeClass", value);
            return entity;
        }
    }
}