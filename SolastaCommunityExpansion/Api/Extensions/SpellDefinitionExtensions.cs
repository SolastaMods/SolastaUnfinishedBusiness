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
    [TargetType(typeof(SpellDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class SpellDefinitionExtensions
    {
        public static T AddSubspellsList<T>(this T entity, params SpellDefinition[] value)
            where T : SpellDefinition
        {
            AddSubspellsList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSubspellsList<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : SpellDefinition
        {
            entity.SubspellsList.AddRange(value);
            return entity;
        }

        public static T ClearSubspellsList<T>(this T entity)
            where T : SpellDefinition
        {
            entity.SubspellsList.Clear();
            return entity;
        }

        public static T SetAiParameters<T>(this T entity, SpellAIParameters value)
            where T : SpellDefinition
        {
            entity.SetField("aiParameters", value);
            return entity;
        }

        public static T SetCastingTime<T>(this T entity, ActivationTime value)
            where T : SpellDefinition
        {
            entity.SetField("castingTime", value);
            return entity;
        }

        public static T SetConcentrationAction<T>(this T entity, ActionDefinitions.ActionParameter value)
            where T : SpellDefinition
        {
            entity.SetField("concentrationAction", value);
            return entity;
        }

        public static T SetDisplayConditionDuration<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("displayConditionDuration", value);
            return entity;
        }

        public static T SetEffectDescription<T>(this T entity, EffectDescription value)
            where T : SpellDefinition
        {
            entity.SetField("effectDescription", value);
            return entity;
        }

        public static T SetImplemented<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("implemented", value);
            return entity;
        }

        public static T SetMaterialComponentType<T>(this T entity, MaterialComponentType value)
            where T : SpellDefinition
        {
            entity.SetField("materialComponentType", value);
            return entity;
        }

        public static T SetRequiresConcentration<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("requiresConcentration", value);
            return entity;
        }

        public static T SetRitual<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("ritual", value);
            return entity;
        }

        public static T SetRitualCastingTime<T>(this T entity, ActivationTime value)
            where T : SpellDefinition
        {
            entity.SetField("ritualCastingTime", value);
            return entity;
        }

        public static T SetSchoolOfMagic<T>(this T entity, String value)
            where T : SpellDefinition
        {
            entity.SetField("schoolOfMagic", value);
            return entity;
        }

        public static T SetSomaticComponent<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("somaticComponent", value);
            return entity;
        }

        public static T SetSpecificMaterialComponentConsumed<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("specificMaterialComponentConsumed", value);
            return entity;
        }

        public static T SetSpecificMaterialComponentCostGp<T>(this T entity, Int32 value)
            where T : SpellDefinition
        {
            entity.SetField("specificMaterialComponentCostGp", value);
            return entity;
        }

        public static T SetSpecificMaterialComponentTag<T>(this T entity, String value)
            where T : SpellDefinition
        {
            entity.SetField("specificMaterialComponentTag", value);
            return entity;
        }

        public static T SetSpellLevel<T>(this T entity, Int32 value)
            where T : SpellDefinition
        {
            entity.SetField("spellLevel", value);
            return entity;
        }

        public static T SetSpellsBundle<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("spellsBundle", value);
            return entity;
        }

        public static T SetSubspellsList<T>(this T entity, params SpellDefinition[] value)
            where T : SpellDefinition
        {
            SetSubspellsList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSubspellsList<T>(this T entity, IEnumerable<SpellDefinition> value)
            where T : SpellDefinition
        {
            entity.SubspellsList.SetRange(value);
            return entity;
        }

        public static T SetTerminateOnItemUnequip<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("terminateOnItemUnequip", value);
            return entity;
        }

        public static T SetUniqueInstance<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("uniqueInstance", value);
            return entity;
        }

        public static T SetVerboseComponent<T>(this T entity, Boolean value)
            where T : SpellDefinition
        {
            entity.SetField("verboseComponent", value);
            return entity;
        }

        public static T SetVocalSpellSemeType<T>(this T entity, VocalSpellSemeType value)
            where T : SpellDefinition
        {
            entity.SetField("vocalSpellSemeType", value);
            return entity;
        }
    }
}
