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
    [TargetType(typeof(DamageForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class DamageFormExtensions
    {
        public static T AddDamageBonusTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : DamageForm
        {
            AddDamageBonusTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDamageBonusTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : DamageForm
        {
            entity.DamageBonusTrends.AddRange(value);
            return entity;
        }

        public static T ClearDamageBonusTrends<T>(this T entity)
            where T : DamageForm
        {
            entity.DamageBonusTrends.Clear();
            return entity;
        }

        public static DamageForm Copy(this DamageForm entity)
        {
            var copy = new DamageForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetBonusDamage<T>(this T entity, System.Int32 value)
            where T : DamageForm
        {
            entity.BonusDamage = value;
            return entity;
        }

        public static T SetDamageBonusTrends<T>(this T entity,  params  RuleDefinitions . TrendInfo [ ]  value)
            where T : DamageForm
        {
            SetDamageBonusTrends(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDamageBonusTrends<T>(this T entity, IEnumerable<RuleDefinitions.TrendInfo> value)
            where T : DamageForm
        {
            entity.DamageBonusTrends.SetRange(value);
            return entity;
        }

        public static T SetDamageType<T>(this T entity, System.String value)
            where T : DamageForm
        {
            entity.DamageType = value;
            return entity;
        }

        public static T SetDiceNumber<T>(this T entity, System.Int32 value)
            where T : DamageForm
        {
            entity.DiceNumber = value;
            return entity;
        }

        public static T SetDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : DamageForm
        {
            entity.DieType = value;
            return entity;
        }

        public static T SetForceKillOnZeroHp<T>(this T entity, System.Boolean value)
            where T : DamageForm
        {
            entity.SetField("forceKillOnZeroHp", value);
            return entity;
        }

        public static T SetHealFromInflictedDamage<T>(this T entity, RuleDefinitions.HealFromInflictedDamage value)
            where T : DamageForm
        {
            entity.SetField("healFromInflictedDamage", value);
            return entity;
        }

        public static T SetHitPointsFloor<T>(this T entity, System.Int32 value)
            where T : DamageForm
        {
            entity.SetField("hitPointsFloor", value);
            return entity;
        }

        public static T SetIgnoreCriticalDoubleDice<T>(this T entity, System.Boolean value)
            where T : DamageForm
        {
            entity.IgnoreCriticalDoubleDice = value;
            return entity;
        }

        public static T SetIgnoreFlyingCharacters<T>(this T entity, System.Boolean value)
            where T : DamageForm
        {
            entity.IgnoreFlyingCharacters = value;
            return entity;
        }

        public static T SetIgnoreSpellAdvancementDamageDice<T>(this T entity, System.Boolean value)
            where T : DamageForm
        {
            entity.IgnoreSpellAdvancementDamageDice = value;
            return entity;
        }

        public static T SetSpecialDeathCondition<T>(this T entity, ConditionDefinition value)
            where T : DamageForm
        {
            entity.SetField("specialDeathCondition", value);
            return entity;
        }

        public static T SetVersatile<T>(this T entity, System.Boolean value)
            where T : DamageForm
        {
            entity.SetField("versatile", value);
            return entity;
        }

        public static T SetVersatileDieType<T>(this T entity, RuleDefinitions.DieType value)
            where T : DamageForm
        {
            entity.VersatileDieType = value;
            return entity;
        }
    }
}