using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionMagicAffinity))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionMagicAffinityExtensions
    {
        public static T AddAdditionalSlots<T>(this T entity, params AdditionalSlotsDuplet[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            AddAdditionalSlots(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAdditionalSlots<T>(this T entity, IEnumerable<AdditionalSlotsDuplet> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.AdditionalSlots.AddRange(value);
            return entity;
        }

        public static T AddDeviceTagsAutoIdentifying<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            AddDeviceTagsAutoIdentifying(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDeviceTagsAutoIdentifying<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.DeviceTagsAutoIdentifying.AddRange(value);
            return entity;
        }

        public static T AddMetamagicOptions<T>(this T entity, params MetamagicOptionDefinition[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            AddMetamagicOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMetamagicOptions<T>(this T entity, IEnumerable<MetamagicOptionDefinition> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.MetamagicOptions.AddRange(value);
            return entity;
        }

        public static T AddSpellImmunities<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            AddSpellImmunities(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSpellImmunities<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SpellImmunities.AddRange(value);
            return entity;
        }

        public static T AddWarListSpells<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            AddWarListSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWarListSpells<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.WarListSpells.AddRange(value);
            return entity;
        }

        public static T ClearAdditionalSlots<T>(this T entity)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.AdditionalSlots.Clear();
            return entity;
        }

        public static T ClearDeviceTagsAutoIdentifying<T>(this T entity)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.DeviceTagsAutoIdentifying.Clear();
            return entity;
        }

        public static T ClearMetamagicOptions<T>(this T entity)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.MetamagicOptions.Clear();
            return entity;
        }

        public static T ClearSpellImmunities<T>(this T entity)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SpellImmunities.Clear();
            return entity;
        }

        public static T ClearWarListSpells<T>(this T entity)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.WarListSpells.Clear();
            return entity;
        }

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

        public static T SetAdditionalSlots<T>(this T entity, params AdditionalSlotsDuplet[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            SetAdditionalSlots(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAdditionalSlots<T>(this T entity, IEnumerable<AdditionalSlotsDuplet> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.AdditionalSlots.SetRange(value);
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

        public static T SetCastingAffinity<T>(this T entity, CastingAffinity value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("castingAffinity", value);
            return entity;
        }

        public static T SetConcentrationAffinity<T>(this T entity, ConcentrationAffinity value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("concentrationAffinity", value);
            return entity;
        }

        public static T SetCounterspellAffinity<T>(this T entity, AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("counterspellAffinity", value);
            return entity;
        }

        public static T SetDeviceTagsAutoIdentifying<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            SetDeviceTagsAutoIdentifying(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDeviceTagsAutoIdentifying<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.DeviceTagsAutoIdentifying.SetRange(value);
            return entity;
        }

        public static T SetExtendedSpellList<T>(this T entity, SpellListDefinition value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("extendedSpellList", value);
            return entity;
        }

        public static T SetForcedSavingThrowAffinity<T>(this T entity, AdvantageType value)
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

        public static T SetMetamagicOptions<T>(this T entity, params MetamagicOptionDefinition[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            SetMetamagicOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMetamagicOptions<T>(this T entity, IEnumerable<MetamagicOptionDefinition> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.MetamagicOptions.SetRange(value);
            return entity;
        }

        public static T SetOverConcentrationThreshold<T>(this T entity, System.Int32 value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("overConcentrationThreshold", value);
            return entity;
        }

        public static T SetPreparedSpellModifier<T>(this T entity, PreparedSpellsModifier value)
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

        public static T SetRitualCasting<T>(this T entity, RitualCasting value)
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

        public static T SetSaveDCModifierType<T>(this T entity, SpellParamsModifierType value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SetField("saveDCModifierType", value);
            return entity;
        }

        public static T SetScribeAdvantageType<T>(this T entity, AdvantageType value)
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

        public static T SetSpellAttackModifierType<T>(this T entity, SpellParamsModifierType value)
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

        public static T SetSpellImmunities<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            SetSpellImmunities(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSpellImmunities<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.SpellImmunities.SetRange(value);
            return entity;
        }

        public static T SetSpellsCounterAffinity<T>(this T entity, AdvantageType value)
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

        public static T SetWarListSpells<T>(this T entity, params System.String[] value)
            where T : FeatureDefinitionMagicAffinity
        {
            SetWarListSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWarListSpells<T>(this T entity, IEnumerable<System.String> value)
            where T : FeatureDefinitionMagicAffinity
        {
            entity.WarListSpells.SetRange(value);
            return entity;
        }
    }
}
