using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions
{
    internal static class GameGadgetExtensions
    {
        public const string Enabled = "Enabled";
        public const string Triggered = "Triggered";
        public const string RemoteEnabled = "RemoteEnabled";
        public const string ParamEnabled = "Param_Enabled";
        public const string Invisible = "Invisible";

        /// <summary>
        /// Returns state of Invisible parameter, or false if not present
        /// </summary>
        public static bool IsInvisible(this GameGadget gadget)
        {
            return gadget.CheckConditionName(Invisible, true, false);
        }

        public static bool IsEnabled(this GameGadget gadget, bool valueIfParamsNotPresent = false)
        {
            // We need to know if both Enabled and ParamEnabled are missing
            var names = gadget.GetField<GameGadget, List<string>>("conditionNames");

            if (!names.Any(n => n == Enabled || n == ParamEnabled))
            {
                // if not present return supplied default value
                return valueIfParamsNotPresent;
            }

            // if at least one is present then return if either is true
            var enabled = gadget.CheckConditionName(Enabled, true, false);
            var paramEnabled = gadget.CheckConditionName(ParamEnabled, true, false);

            SolastaCommunityExpansion.Main.Log($"{gadget.UniqueNameId}, Enabled={enabled}, ParamEnabled={paramEnabled}");

            return enabled || paramEnabled;
        }

        public static bool CheckConditionName(this GameGadget gadget, string name, bool value, bool valueIfMissing)
        {
            return (bool)CheckConditionNameMethod.Invoke(gadget, new object[] { name, value, valueIfMissing });
        }

        private static readonly MethodInfo CheckConditionNameMethod
            = typeof(GameGadget).GetMethod("CheckConditionName", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public enum ExtraEffectFormType
    {
        Damage = EffectForm.EffectFormType.Damage,
        Healing = EffectForm.EffectFormType.Healing,
        Condition = EffectForm.EffectFormType.Condition,
        LightSource = EffectForm.EffectFormType.LightSource,
        Summon = EffectForm.EffectFormType.Summon,
        Counter = EffectForm.EffectFormType.Counter,
        TemporaryHitPoints = EffectForm.EffectFormType.TemporaryHitPoints,
        Motion = EffectForm.EffectFormType.Motion,
        SpellSlots = EffectForm.EffectFormType.SpellSlots,
        Divination = EffectForm.EffectFormType.Divination,
        ItemProperty = EffectForm.EffectFormType.ItemProperty,
        Alteration = EffectForm.EffectFormType.Alteration,
        Topology = EffectForm.EffectFormType.Topology,
        Revive = EffectForm.EffectFormType.Revive,
        Kill = EffectForm.EffectFormType.Kill,
        ShapeChange = EffectForm.EffectFormType.ShapeChange,
        Custom = 9000,
    }

    public enum ExtraRitualCasting
    {
        None = RuleDefinitions.RitualCasting.None,
        Prepared = RuleDefinitions.RitualCasting.Prepared,
        Spellbook = RuleDefinitions.RitualCasting.Spellbook,
        Known = 9000
    }

    public enum ExtraOriginOfAmount
    {
        None = 0,
        SourceDamage = 1,
        SourceGain = 2,
        AddDice = 3,
        Fixed = 4,
        SourceHalfHitPoints = 5,
        SourceSpellCastingAbility = 6,
        SourceSpellAttack = 7,
        SourceProficiencyBonus = 9000,
        SourceCharacterLevel = 9001,
        SourceClassLevel = 9002
    }

    public enum ExtraAttributeModifierOperation
    {
        Set = 0,
        Additive= 1,
        Multiplicative= 2,
        MultiplyByClassLevel = 3,
        MultiplyByCharacterLevel= 4,
        Force = 5,
        AddAbilityScoreBonus = 6,
        ConditionAmount = 7,
        SurroundingEnemies = 8,
        AdditiveAtEnd = 9000
    }
}
