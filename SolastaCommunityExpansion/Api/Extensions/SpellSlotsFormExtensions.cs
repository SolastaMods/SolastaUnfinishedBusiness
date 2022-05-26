using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(SpellSlotsForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class SpellSlotsFormExtensions
    {
        public static SpellSlotsForm Copy(this SpellSlotsForm entity)
        {
            var copy = new SpellSlotsForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetMaxSlotLevel<T>(this T entity, System.Int32 value)
            where T : SpellSlotsForm
        {
            entity.SetField("maxSlotLevel", value);
            return entity;
        }

        public static T SetPowerDefinition<T>(this T entity, FeatureDefinitionPower value)
            where T : SpellSlotsForm
        {
            entity.SetField("powerDefinition", value);
            return entity;
        }

        public static T SetSorceryPointsGain<T>(this T entity, System.Int32 value)
            where T : SpellSlotsForm
        {
            entity.SetField("sorceryPointsGain", value);
            return entity;
        }

        public static T SetType<T>(this T entity, SpellSlotsForm.EffectType value)
            where T : SpellSlotsForm
        {
            entity.SetField("type", value);
            return entity;
        }
    }
}
