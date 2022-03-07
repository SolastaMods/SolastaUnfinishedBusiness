using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class BattleHandleEventContext
    {
        public delegate void HandleCharacterAttack(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackerAttackMode);

        public delegate void HandleRangeAttack(
            ref GameLocationCharacter launcher,
            ref GameLocationCharacter target,
            ref RulesetAttackMode attackMode,
            ref Vector3 sourcePoint,
            ref Vector3 impactPoint,
            ref float projectileFlightDuration);

        public delegate void HandleCharacterAttackHit(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref int attackRoll,
            ref int successDelta,
            ref bool ranged);

        public delegate void HandleCharacterAttackDamage(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackMode,
            ref bool rangedAttack,
            ref RuleDefinitions.AdvantageType advantageType,
            ref List<EffectForm> actualEffectForms,
            ref RulesetEffect rulesetEffect,
            ref bool criticalHit,
            ref bool firstTarget);

        public delegate void HandleReactionToDamageShare(
            ref GameLocationCharacter damagedCharacter,
            ref int damageAmount);

        public delegate void HandleCharacterAttackFinished(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref RulesetAttackMode attackerAttackMode);

        public delegate void HandleSpellCast(
            ref GameLocationCharacter caster,
            ref CharacterActionCastSpell castAction,
            ref RulesetSpellRepertoire selectedRepertoire,
            ref SpellDefinition selectedSpellDefinition);

        public delegate void HandleSpellTargeted(
            ref GameLocationCharacter caster,
            ref RulesetEffectSpell hostileSpell,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier);

        public delegate void HandleFailedSavingThrowAgainstEffect(
            ref CharacterActionMagicEffect action,
            ref GameLocationCharacter caster,
            ref GameLocationCharacter defender,
            ref RulesetEffect rulesetEffect,
            ref ActionModifier saveModifier,
            ref bool hasHitVisual);

        public delegate void HandleCharacterMagicalAttackDamage(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier magicModifier,
            ref RulesetEffect rulesetEffect,
            ref List<EffectForm> actualEffectForms);

        public static HandleCharacterAttack HandleCharacterAttackHandler { get; set; }
        public static HandleRangeAttack HandleRangeAttackHandler { get; set; }
        public static HandleCharacterAttackHit HandleCharacterAttackHitHandler { get; set; }
        public static HandleCharacterAttackDamage HandleCharacterAttackDamageHandler { get; set; }
        public static HandleCharacterMagicalAttackDamage HandleCharacterMagicalAttackDamageHandler { get; set; }
        public static HandleReactionToDamageShare HandleReactionToDamageShareHandler { get; set; }
        public static HandleCharacterAttackFinished HandleCharacterAttackFinishedHandler { get; set; }
        public static HandleSpellCast HandleSpellCastHandler { get; set; }
        public static HandleSpellTargeted HandleSpellTargetedHandler { get; set; }
        public static HandleFailedSavingThrowAgainstEffect HandleFailedSavingThrowAgainstEffectHandler { get; set; }

        private static readonly Dictionary<string, System.Action<Object>> EnumeratorMethodsScope = new()
        {
            { "HandleCharacterAttack", InvokeHandleCharacterAttack },
            { "HandleRangeAttack", InvokeHandleRangeAttack },
            { "HandleCharacterAttackHit", InvokeHandleCharacterAttackHit },
            { "HandleCharacterAttackDamage", InvokeHandleCharacterAttackDamage },
            { "HandleCharacterMagicalAttackDamage", InvokeHandleCharacterMagicalAttackDamage },
            { "HandleReactionToDamageShare", InvokeHandleReactionToDamageShare },
            { "HandleCharacterAttackFinished", InvokeHandleCharacterAttackFinished },
            { "HandleSpellCast", InvokeHandleSpellCast },
            { "HandleSpellTargeted", InvokeHandleSpellTargeted },
            { "HandleFailedSavingThrowAgainstEffect", InvokeHandleFailedSavingThrowAgainstEffect },
        };

        private static readonly Dictionary<System.Type, System.Action<Object>> EnumeratorMethodsDispatcher = new();

        private static void InvokeHandleCharacterAttack(Object obj)
        {
            if (HandleCharacterAttackHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "attackModifier").GetValue(obj);
            var attackerAttackMode = (RulesetAttackMode)AccessTools.Field(obj.GetType(), "attackerAttackMode").GetValue(obj);

            HandleCharacterAttackHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackerAttackMode);

            AccessTools.Field(obj.GetType(), "attacker").SetValue(obj, attacker);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(obj.GetType(), "attackerAttackMode").SetValue(obj, attackerAttackMode);
        }

        private static void InvokeHandleRangeAttack(Object obj)
        {
            if (HandleRangeAttackHandler == null)
            {
                return;
            }

            var launcher = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "launcher").GetValue(obj);
            var target = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "target").GetValue(obj);
            var attackMode = (RulesetAttackMode)AccessTools.Field(obj.GetType(), "attackMode").GetValue(obj);
            var sourcePoint = (Vector3)AccessTools.Field(obj.GetType(), "sourcePoint").GetValue(obj);
            var impactPoint = (Vector3)AccessTools.Field(obj.GetType(), "impactPoint").GetValue(obj);
            var projectileFlightDuration = (float)AccessTools.Field(obj.GetType(), "projectileFlightDuration").GetValue(obj);

            HandleRangeAttackHandler
                .Invoke(ref launcher, ref target, ref attackMode, ref sourcePoint, ref impactPoint, ref projectileFlightDuration);

            AccessTools.Field(obj.GetType(), "launcher").SetValue(obj, launcher);
            AccessTools.Field(obj.GetType(), "target").SetValue(obj, target);
            AccessTools.Field(obj.GetType(), "attackMode").SetValue(obj, attackMode);
            AccessTools.Field(obj.GetType(), "sourcePoint").SetValue(obj, sourcePoint);
            AccessTools.Field(obj.GetType(), "impactPoint").SetValue(obj, impactPoint);
            AccessTools.Field(obj.GetType(), "projectileFlightDuration").SetValue(obj, projectileFlightDuration);
        }

        private static void InvokeHandleCharacterAttackHit(Object obj)
        {
            if (HandleCharacterAttackHitHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "attackModifier").GetValue(obj);
            var attackRoll = (int)AccessTools.Field(obj.GetType(), "attackRoll").GetValue(obj);
            var successDelta = (int)AccessTools.Field(obj.GetType(), "successDelta").GetValue(obj);
            var ranged = (bool)AccessTools.Field(obj.GetType(), "ranged").GetValue(obj);

            HandleCharacterAttackHitHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackRoll, ref successDelta, ref ranged);

            AccessTools.Field(obj.GetType(), "attacker").SetValue(obj, attacker);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(obj.GetType(), "attackRoll").SetValue(obj, attackRoll);
            AccessTools.Field(obj.GetType(), "successDelta").SetValue(obj, successDelta);
            AccessTools.Field(obj.GetType(), "ranged").SetValue(obj, ranged);
        }

        private static void InvokeHandleCharacterAttackDamage(Object obj)
        {
            if (HandleCharacterAttackDamageHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "attackModifier").GetValue(obj);
            var attackMode = (RulesetAttackMode)AccessTools.Field(obj.GetType(), "attackMode").GetValue(obj);
            var rangedAttack = (bool)AccessTools.Field(obj.GetType(), "rangedAttack").GetValue(obj);
            var advantageType = (RuleDefinitions.AdvantageType)AccessTools.Field(obj.GetType(), "advantageType").GetValue(obj);
            var actualEffectForms = (List<EffectForm>)AccessTools.Field(obj.GetType(), "actualEffectForms").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(obj.GetType(), "rulesetEffect").GetValue(obj);
            var criticalHit = (bool)AccessTools.Field(obj.GetType(), "criticalHit").GetValue(obj);
            var firstTarget = (bool)AccessTools.Field(obj.GetType(), "firstTarget").GetValue(obj);

            HandleCharacterAttackDamageHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackMode, ref rangedAttack, ref advantageType, ref actualEffectForms, ref rulesetEffect, ref criticalHit, ref firstTarget);

            AccessTools.Field(obj.GetType(), "attacker").SetValue(obj, attacker);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(obj.GetType(), "attackMode").SetValue(obj, attackMode);
            AccessTools.Field(obj.GetType(), "rangedAttack").SetValue(obj, rangedAttack);
            AccessTools.Field(obj.GetType(), "advantageType").SetValue(obj, advantageType);
            AccessTools.Field(obj.GetType(), "actualEffectForms").SetValue(obj, actualEffectForms);
            AccessTools.Field(obj.GetType(), "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(obj.GetType(), "criticalHit").SetValue(obj, criticalHit);
            AccessTools.Field(obj.GetType(), "firstTarget").SetValue(obj, firstTarget);
        }

        private static void InvokeHandleCharacterMagicalAttackDamage(Object obj)
        {
            if (HandleCharacterMagicalAttackDamageHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var magicModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "magicModifier").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(obj.GetType(), "rulesetEffect").GetValue(obj);
            var actualEffectForms = (List<EffectForm>)AccessTools.Field(obj.GetType(), "actualEffectForms").GetValue(obj);

            HandleCharacterMagicalAttackDamageHandler
                .Invoke(ref attacker, ref defender, ref magicModifier, ref rulesetEffect, ref actualEffectForms);

            AccessTools.Field(obj.GetType(), "attacker").SetValue(obj, attacker);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "magicModifier").SetValue(obj, magicModifier);
            AccessTools.Field(obj.GetType(), "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(obj.GetType(), "actualEffectForms").SetValue(obj, actualEffectForms);
        }

        private static void InvokeHandleReactionToDamageShare(Object obj)
        {
            if (HandleReactionToDamageShareHandler == null)
            {
                return;
            }

            var damagedCharacter = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "damagedCharacter").GetValue(obj);
            var damageAmount = (int)AccessTools.Field(obj.GetType(), "damageAmount").GetValue(obj);

            HandleReactionToDamageShareHandler
                .Invoke(ref damagedCharacter, ref damageAmount);

            AccessTools.Field(obj.GetType(), "damagedCharacter").SetValue(obj, damagedCharacter);
            AccessTools.Field(obj.GetType(), "damageAmount").SetValue(obj, damageAmount);
        }

        private static void InvokeHandleCharacterAttackFinished(Object obj)
        {
            if (HandleCharacterAttackFinishedHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var attackerAttackMode = (RulesetAttackMode)AccessTools.Field(obj.GetType(), "attackerAttackMode").GetValue(obj);

            HandleCharacterAttackFinishedHandler
                .Invoke(ref attacker, ref defender, ref attackerAttackMode);

            AccessTools.Field(obj.GetType(), "attacker").SetValue(obj, attacker);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "attackerAttackMode").SetValue(obj, attackerAttackMode);
        }

        private static void InvokeHandleSpellCast(Object obj)
        {
            if (HandleSpellCastHandler == null)
            {
                return;
            }

            var caster = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "caster").GetValue(obj);
            var castAction = (CharacterActionCastSpell)AccessTools.Field(obj.GetType(), "castAction").GetValue(obj);
            var selectedRepertoire = (RulesetSpellRepertoire)AccessTools.Field(obj.GetType(), "selectedRepertoire").GetValue(obj);
            var selectedSpellDefinition = (SpellDefinition)AccessTools.Field(obj.GetType(), "selectedSpellDefinition").GetValue(obj);

            HandleSpellCastHandler
                .Invoke(ref caster, ref castAction, ref selectedRepertoire, ref selectedSpellDefinition);

            AccessTools.Field(obj.GetType(), "caster").SetValue(obj, caster);
            AccessTools.Field(obj.GetType(), "castAction").SetValue(obj, castAction);
            AccessTools.Field(obj.GetType(), "selectedRepertoire").SetValue(obj, selectedRepertoire);
            AccessTools.Field(obj.GetType(), "selectedSpellDefinition").SetValue(obj, selectedSpellDefinition);
        }

        private static void InvokeHandleSpellTargeted(Object obj)
        {
            if (HandleSpellTargetedHandler == null)
            {
                return;
            }

            var caster = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "caster").GetValue(obj);
            var hostileSpell = (RulesetEffectSpell)AccessTools.Field(obj.GetType(), "hostileSpell").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "attackModifier").GetValue(obj);


            HandleSpellTargetedHandler
                .Invoke(ref caster, ref hostileSpell, ref defender, ref attackModifier);

            AccessTools.Field(obj.GetType(), "caster").SetValue(obj, caster);
            AccessTools.Field(obj.GetType(), "hostileSpell").SetValue(obj, hostileSpell);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "attackModifier").SetValue(obj, attackModifier);
        }

        private static void InvokeHandleFailedSavingThrowAgainstEffect(Object obj)
        {
            if (HandleFailedSavingThrowAgainstEffectHandler == null)
            {
                return;
            }

            var action = (CharacterActionMagicEffect)AccessTools.Field(obj.GetType(), "action").GetValue(obj);
            var caster = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "caster").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(obj.GetType(), "defender").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(obj.GetType(), "rulesetEffect").GetValue(obj);
            var saveModifier = (ActionModifier)AccessTools.Field(obj.GetType(), "saveModifier").GetValue(obj);
            var hasHitVisual = (bool)AccessTools.Field(obj.GetType(), "hasHitVisual").GetValue(obj);


            HandleFailedSavingThrowAgainstEffectHandler
                .Invoke(ref action, ref caster, ref defender, ref rulesetEffect, ref saveModifier, ref hasHitVisual);

            AccessTools.Field(obj.GetType(), "action").SetValue(obj, action);
            AccessTools.Field(obj.GetType(), "caster").SetValue(obj, caster);
            AccessTools.Field(obj.GetType(), "defender").SetValue(obj, defender);
            AccessTools.Field(obj.GetType(), "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(obj.GetType(), "saveModifier").SetValue(obj, saveModifier);
            AccessTools.Field(obj.GetType(), "hasHitVisual").SetValue(obj, hasHitVisual);
        }

        public static void Dispatcher(Object obj)
        {
            if (EnumeratorMethodsDispatcher.TryGetValue(obj.GetType(), out var dispatcher))
            {
                dispatcher.Invoke(obj);
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var dispatcherMethod = typeof(BattleHandleEventContext).GetMethod("Dispatcher");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Dup); // duplicates the object on stack
                    yield return new CodeInstruction(OpCodes.Call, dispatcherMethod);
                }

                yield return instruction;
            }
        }

        private static void PatchAll()
        {
            var harmony = new Harmony("SolastaCommunityExpansion");
            var gameLocationBattleManagerType = typeof(GameLocationBattleManager);
            var internalEnumeratorClasses = gameLocationBattleManagerType.GetNestedTypes(System.Reflection.BindingFlags.NonPublic);
            var transpiler = typeof(BattleHandleEventContext).GetMethod("Transpiler");

            foreach (var internalEnumeratorClass in internalEnumeratorClasses)
            {
                var className = internalEnumeratorClass.Name;
                var startIndex = className.IndexOf('<') + 1;
                var length = className.IndexOf('>') - startIndex;
                var methodName = className.Substring(startIndex, length);

                if (EnumeratorMethodsScope.TryGetValue(methodName, out var dispatcher))
                {
                    var handleMethod = typeof(GameLocationBattleManager).GetMethod(methodName);

                    harmony.Patch(handleMethod, transpiler: new HarmonyMethod(transpiler));
                    EnumeratorMethodsDispatcher.Add(internalEnumeratorClass, dispatcher);
                }
            }
        }

        private static void EnableLogs()
        {
            static string ActorName(GameLocationCharacter gameLocationCharacter) =>
                gameLocationCharacter != null && gameLocationCharacter.RulesetActor != null ? gameLocationCharacter.RulesetActor.Name : "NONE";

            HandleCharacterAttackHandler += new HandleCharacterAttack(
            (
                ref GameLocationCharacter attacker,
                ref GameLocationCharacter defender,
                ref ActionModifier attackModifier,
                ref RulesetAttackMode attackerAttackMode
            ) => Main.Warning($"HandleCharacterAttack attacker: {ActorName(attacker)} defender: {ActorName(defender)}"));

            HandleRangeAttackHandler += new HandleRangeAttack(
            (
                ref GameLocationCharacter launcher,
                ref GameLocationCharacter target,
                ref RulesetAttackMode attackMode,
                ref Vector3 sourcePoint,
                ref Vector3 impactPoint,
                ref float projectileFlightDuration
            ) => Main.Warning($"HandleRangeAttack launcher: {ActorName(launcher)} target: {ActorName(target)}"));

            HandleCharacterAttackHitHandler += new HandleCharacterAttackHit(
            (
                ref GameLocationCharacter attacker,
                ref GameLocationCharacter defender,
                ref ActionModifier attackModifier,
                ref int attackRoll,
                ref int successDelta,
                ref bool ranged
            ) => Main.Warning($"HandleCharacterAttackHit attacker: {ActorName(attacker)} defender: {ActorName(defender)}"));

            HandleCharacterAttackDamageHandler += new HandleCharacterAttackDamage(
            (
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackMode,
            ref bool rangedAttack,
            ref RuleDefinitions.AdvantageType advantageType,
            ref List<EffectForm> actualEffectForms,
            ref RulesetEffect rulesetEffect,
            ref bool criticalHit,
            ref bool firstTarget
            ) => Main.Warning($"HandleCharacterAttackDamage attacker: {ActorName(attacker)} defender: {ActorName(defender)}"));

            HandleCharacterMagicalAttackDamageHandler += new HandleCharacterMagicalAttackDamage(
            (
                ref GameLocationCharacter attacker,
                ref GameLocationCharacter defender,
                ref ActionModifier magicModifier,
                ref RulesetEffect rulesetEffect,
                ref List <EffectForm> actualEffectForms
            ) => Main.Warning($"HandleCharacterMagicalAttackDamage attacker: {ActorName(attacker)} defender: {ActorName(defender)}"));

            HandleReactionToDamageShareHandler += new HandleReactionToDamageShare(
            (
                ref GameLocationCharacter damagedCharacter,
                ref int damageAmount
            ) => Main.Warning($"HandleReactionToDamageShare damagedCharacter: {ActorName(damagedCharacter)} damageAmount: {damageAmount}"));

            HandleCharacterAttackFinishedHandler += new HandleCharacterAttackFinished(
            (
                ref GameLocationCharacter attacker,
                ref GameLocationCharacter defender,
                ref RulesetAttackMode attackerAttackMode
            ) => Main.Warning($"HandleCharacterAttackFinished attacker: {ActorName(attacker)} defender: {ActorName(defender)}"));

            HandleSpellCastHandler += new HandleSpellCast(
            (
                ref GameLocationCharacter caster,
                ref CharacterActionCastSpell castAction,
                ref RulesetSpellRepertoire selectedRepertoire,
                ref SpellDefinition selectedSpellDefinition
            ) => Main.Warning($"HandleSpellCast caster: {ActorName(caster)} spell: {selectedSpellDefinition.Name}"));

            HandleSpellTargetedHandler += new HandleSpellTargeted(
            (
                ref GameLocationCharacter caster,
                ref RulesetEffectSpell hostileSpell,
                ref GameLocationCharacter defender,
                ref ActionModifier attackModifier
            ) => Main.Warning($"HandleSpellTargeted caster: {ActorName(caster)} defender: {ActorName(defender)}"));

            HandleFailedSavingThrowAgainstEffectHandler += new HandleFailedSavingThrowAgainstEffect(
            (
                ref CharacterActionMagicEffect action,
                ref GameLocationCharacter caster,
                ref GameLocationCharacter defender,
                ref RulesetEffect rulesetEffect,
                ref ActionModifier saveModifier,
                ref bool hasHitVisual
            ) => Main.Warning($"HandleFailedSavingThrowAgainstEffect caster: {ActorName(caster)} defender: {ActorName(defender)}"));
        }

        internal static void Load()
        {
            if (Main.Settings.EnableHandleEventsLog)
            {
                PatchAll();
                EnableLogs();
            }
        }
    }
}
