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
    [TargetType(typeof(UserEffectForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class UserEffectFormExtensions
    {
        public static UserEffectForm Copy(this UserEffectForm entity)
        {
            return new UserEffectForm(entity);
        }

        public static T SetBonusDamage<T>(this T entity, System.Int32 value)
            where T : UserEffectForm
        {
            entity.BonusDamage = value;
            return entity;
        }

        public static T SetCanSaveToCancel<T>(this T entity, System.Boolean value)
            where T : UserEffectForm
        {
            entity.CanSaveToCancel = value;
            return entity;
        }

        public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
            where T : UserEffectForm
        {
            entity.SetField("conditionDefinition", value);
            return entity;
        }

        public static T SetConditionDefinitionName<T>(this T entity, System.String value)
            where T : UserEffectForm
        {
            entity.ConditionDefinitionName = value;
            return entity;
        }

        public static T SetDamageType<T>(this T entity, System.String value)
            where T : UserEffectForm
        {
            entity.DamageType = value;
            return entity;
        }

        public static T SetDiceNumber<T>(this T entity, System.Int32 value)
            where T : UserEffectForm
        {
            entity.DiceNumber = value;
            return entity;
        }

        public static T SetDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : UserEffectForm
        {
            entity.DieType = value;
            return entity;
        }

        public static T SetFormType<T>(this T entity, UserContentDefinitions.EffectFormType value)
            where T : UserEffectForm
        {
            entity.FormType = value;
            return entity;
        }

        public static T SetSaveAffinity<T>(this T entity, RuleDefinitions.EffectSavingThrowType value)
            where T : UserEffectForm
        {
            entity.SaveAffinity = value;
            return entity;
        }
    }
}