using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(FeatureDefinitionCastSpell))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class FeatureDefinitionCastSpellExtensions
    {
        public static T AddKnownCantrips<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddKnownCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownCantrips<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownCantrips.AddRange(value);
            return entity;
        }

        public static T AddKnownSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddKnownSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddKnownSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownSpells.AddRange(value);
            return entity;
        }

        public static T AddReplacedSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddReplacedSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddReplacedSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.ReplacedSpells.AddRange(value);
            return entity;
        }

        public static T AddRestrictedSchools<T>(this T entity, params String[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddRestrictedSchools(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRestrictedSchools<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.RestrictedSchools.AddRange(value);
            return entity;
        }

        public static T AddScribedSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddScribedSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddScribedSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.ScribedSpells.AddRange(value);
            return entity;
        }

        public static T AddSlotsPerLevels<T>(this T entity,
            params FeatureDefinitionCastSpell.SlotsByLevelDuplet[] value)
            where T : FeatureDefinitionCastSpell
        {
            AddSlotsPerLevels(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSlotsPerLevels<T>(this T entity,
            IEnumerable<FeatureDefinitionCastSpell.SlotsByLevelDuplet> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SlotsPerLevels.AddRange(value);
            return entity;
        }

        public static T ClearKnownCantrips<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownCantrips.Clear();
            return entity;
        }

        public static T ClearKnownSpells<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownSpells.Clear();
            return entity;
        }

        public static T ClearReplacedSpells<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.ReplacedSpells.Clear();
            return entity;
        }

        public static T ClearRestrictedSchools<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.RestrictedSchools.Clear();
            return entity;
        }

        public static T ClearScribedSpells<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.ScribedSpells.Clear();
            return entity;
        }

        public static T ClearSlotsPerLevels<T>(this T entity)
            where T : FeatureDefinitionCastSpell
        {
            entity.SlotsPerLevels.Clear();
            return entity;
        }

        public static T SetFocusType<T>(this T entity, EquipmentDefinitions.FocusType value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("focusType", value);
            return entity;
        }

        public static T SetKnownCantrips<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetKnownCantrips(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownCantrips<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownCantrips.SetRange(value);
            return entity;
        }

        public static T SetKnownSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetKnownSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetKnownSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.KnownSpells.SetRange(value);
            return entity;
        }

        public static T SetReplacedSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetReplacedSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetReplacedSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.ReplacedSpells.SetRange(value);
            return entity;
        }

        public static T SetRestrictedSchools<T>(this T entity, params String[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetRestrictedSchools(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRestrictedSchools<T>(this T entity, IEnumerable<String> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.RestrictedSchools.SetRange(value);
            return entity;
        }

        public static T SetScribedSpells<T>(this T entity, params Int32[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetScribedSpells(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetScribedSpells<T>(this T entity, IEnumerable<Int32> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.ScribedSpells.SetRange(value);
            return entity;
        }

        public static T SetSlotsPerLevels<T>(this T entity,
            params FeatureDefinitionCastSpell.SlotsByLevelDuplet[] value)
            where T : FeatureDefinitionCastSpell
        {
            SetSlotsPerLevels(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSlotsPerLevels<T>(this T entity,
            IEnumerable<FeatureDefinitionCastSpell.SlotsByLevelDuplet> value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SlotsPerLevels.SetRange(value);
            return entity;
        }

        public static T SetSlotsRecharge<T>(this T entity, RechargeRate value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("slotsRecharge", value);
            return entity;
        }

        public static T SetSpellcastingAbility<T>(this T entity, String value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellcastingAbility", value);
            return entity;
        }

        public static T SetSpellCastingLevel<T>(this T entity, Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellCastingLevel", value);
            return entity;
        }

        public static T SetSpellCastingOrigin<T>(this T entity, FeatureDefinitionCastSpell.CastingOrigin value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellCastingOrigin", value);
            return entity;
        }

        public static T SetSpellcastingParametersComputation<T>(this T entity, SpellcastingParametersComputation value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellcastingParametersComputation", value);
            return entity;
        }

        public static T SetSpellKnowledge<T>(this T entity, SpellKnowledge value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellKnowledge", value);
            return entity;
        }

        public static T SetSpellListDefinition<T>(this T entity, SpellListDefinition value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellListDefinition", value);
            return entity;
        }

        public static T SetSpellPreparationCount<T>(this T entity, SpellPreparationCount value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellPreparationCount", value);
            return entity;
        }

        public static T SetSpellReadyness<T>(this T entity, SpellReadyness value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("spellReadyness", value);
            return entity;
        }

        public static T SetStaticDCValue<T>(this T entity, Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("staticDCValue", value);
            return entity;
        }

        public static T SetStaticToHitValue<T>(this T entity, Int32 value)
            where T : FeatureDefinitionCastSpell
        {
            entity.SetField("staticToHitValue", value);
            return entity;
        }
    }
}
