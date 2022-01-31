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
    [TargetType(typeof(FeatureDefinitionMagicAffinity))]
    public static partial class FeatureDefinitionMagicAffinityExtensions
    {
        public static T SetAdditionalKnownSpellsCount<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("additionalKnownSpellsCount", value);
            return entity;
        }

        public static T SetAdditionalScribedSpells<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("additionalScribedSpells", value);
            return entity;
        }

        public static T SetCantripRetribution<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("cantripRetribution", value);
            return entity;
        }

        public static T SetCanUseProficientWeaponAsFocus<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("canUseProficientWeaponAsFocus", value);
            return entity;
        }

        public static T SetCastingAffinity<T>(this T entity, RuleDefinitions.CastingAffinity value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("castingAffinity", value);
            return entity;
        }

        public static T SetConcentrationAffinity<T>(this T entity, RuleDefinitions.ConcentrationAffinity value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("concentrationAffinity", value);
            return entity;
        }

        public static T SetCounterspellAffinity<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("counterspellAffinity", value);
            return entity;
        }

        public static T SetExtendedSpellList<T>(this T entity, SpellListDefinition value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("extendedSpellList", value);
            return entity;
        }

        public static T SetForcedSavingThrowAffinity<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("forcedSavingThrowAffinity", value);
            return entity;
        }

        public static T SetForcedSpellDefinition<T>(this T entity, SpellDefinition value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("forcedSpellDefinition", value);
            return entity;
        }

        public static T SetForceHalfDamageOnCantrips<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("forceHalfDamageOnCantrips", value);
            return entity;
        }

        public static T SetImpairedSpeech<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("impairedSpeech", value);
            return entity;
        }

        public static T SetMaxSpellLevelImmunity<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("maxSpellLevelImmunity", value);
            return entity;
        }

        public static T SetOverConcentrationThreshold<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("overConcentrationThreshold", value);
            return entity;
        }

        public static T SetPreparedSpellModifier<T>(this T entity, RuleDefinitions.PreparedSpellsModifier value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("preparedSpellModifier", value);
            return entity;
        }

        public static T SetPreserveSlotLevelCap<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("preserveSlotLevelCap", value);
            return entity;
        }

        public static T SetPreserveSlotRoll<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("preserveSlotRoll", value);
            return entity;
        }

        public static T SetPreserveSlotThreshold<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("preserveSlotThreshold", value);
            return entity;
        }

        public static T SetRangeSpellNoProximityPenalty<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("rangeSpellNoProximityPenalty", value);
            return entity;
        }

        public static T SetRitualCasting<T>(this T entity, RuleDefinitions.RitualCasting value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("ritualCasting", value);
            return entity;
        }

        public static T SetSaveDCModifier<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("saveDCModifier", value);
            return entity;
        }

        public static T SetSaveDCModifierType<T>(this T entity, RuleDefinitions.SpellParamsModifierType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("saveDCModifierType", value);
            return entity;
        }

        public static T SetScribeAdvantageType<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("scribeAdvantageType", value);
            return entity;
        }

        public static T SetScribeCostMultiplier<T>(this T entity, System.Single value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("scribeCostMultiplier", value);
            return entity;
        }

        public static T SetScribeDurationMultiplier<T>(this T entity, System.Single value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("scribeDurationMultiplier", value);
            return entity;
        }

        public static T SetSomaticWithWeapon<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("somaticWithWeapon", value);
            return entity;
        }

        public static T SetSomaticWithWeaponOrShield<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("somaticWithWeaponOrShield", value);
            return entity;
        }

        public static T SetSpellAttackModifier<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("spellAttackModifier", value);
            return entity;
        }

        public static T SetSpellAttackModifierType<T>(this T entity, RuleDefinitions.SpellParamsModifierType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("spellAttackModifierType", value);
            return entity;
        }

        public static T SetSpellcastingSuccessDC<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("spellcastingSuccessDC", value);
            return entity;
        }

        public static T SetSpellsCounterAffinity<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("spellsCounterAffinity", value);
            return entity;
        }

        public static T SetUsesWarList<T>(this T entity, System.Boolean value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("usesWarList", value);
            return entity;
        }

        public static T SetWarListSlotBonus<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("warListSlotBonus", value);
            return entity;
        }
    }
}