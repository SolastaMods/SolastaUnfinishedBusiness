using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class ActionEnumeratorsContext
    {
        //
        // These are enumerators called from ActionAttack and ActionMagicEffect (CastSpell or UsePower)
        // "Invoke" methods are called at the very end of these enumerators contructors
        // Both the object being constructed and the type are passed to the "Invoke" methods
        // You can at this point change the object fields simulating a real PREFIX on IEnumerator Method() calls
        // ... or simply get a context at a specific point during an Action
        //

        //
        // On current implementation these "Invoke" methods try to call the public delegate handlers declared on this class
        //

        private static readonly Dictionary<string, System.Action<Object, System.Type>> EnumeratorMethodsScope = new()
        {
            //{ "CharacterActionMagicEffect.MagicEffectExecuteOnZone", InvokeMagicEffectExecuteOnZone },
            //{ "CharacterActionMagicEffect.MagicEffectExecuteOnTargets", InvokeMagicEffectExecuteOnTargets },
            //{ "CharacterActionMagicEffect.MagicEffectExecuteOnPositions", InvokeMagicEffectExecuteOnPositions },
            { "CharacterActionMagicEffect.ExecuteMagicAttack", InvokeExecuteMagicAttack },

            { "GameLocationBattleManager.HandleCharacterAttack", InvokeHandleCharacterAttack },
            { "GameLocationBattleManager.HandleRangeAttack", InvokeHandleRangeAttack },
            { "GameLocationBattleManager.HandleCharacterAttackHit", InvokeHandleCharacterAttackHit },
            { "GameLocationBattleManager.HandleCharacterAttackDamage", InvokeHandleCharacterAttackDamage },
            { "GameLocationBattleManager.HandleCharacterMagicalAttackDamage", InvokeHandleCharacterMagicalAttackDamage },
            { "GameLocationBattleManager.HandleReactionToDamageShare", InvokeHandleReactionToDamageShare },
            { "GameLocationBattleManager.HandleCharacterAttackFinished", InvokeHandleCharacterAttackFinished },
            { "GameLocationBattleManager.HandleSpellCast", InvokeHandleSpellCast },
            { "GameLocationBattleManager.HandleSpellTargeted", InvokeHandleSpellTargeted },
            { "GameLocationBattleManager.HandleFailedSavingThrowAgainstEffect", InvokeHandleFailedSavingThrowAgainstEffect },
        };

        //
        // Current delegates implementation. These can be used to change the object as in a PREFIX or simply get a context
        //

        // CharacterActionMagicEffect delegates
        //public static MagicEffectExecuteOnZone MagicEffectExecuteOnZoneHandler { get; set; }
        //public static MagicEffectExecuteOnTargets MagicEffectExecuteOnTargetsHandler { get; set; }
        //public static MagicEffectExecuteOnPositions MagicEffectExecuteOnPositionsHandler { get; set; }
        public static ExecuteMagicAttack ExecuteMagicAttackHandler { get; set; }

        //public delegate void MagicEffectExecuteOnZone(
        //    ref List<GameLocationCharacter> targets,
        //    ref List<ActionModifier> actionModifiers,
        //    ref Vector3 castingPoint,
        //    ref Vector3 impactPoint,
        //    ref Vector3 impactPlanePoint);

        //public delegate void MagicEffectExecuteOnTargets(
        //    ref List<GameLocationCharacter> targets,
        //    ref List<ActionModifier> actionModifiers,
        //    ref Vector3 castingPoint,
        //    ref Vector3 impactPoint,
        //    ref Vector3 impactPlanePoint);

        //public delegate void MagicEffectExecuteOnPositions(
        //    ref List<int3> positions,
        //    ref Vector3 castingPoint,
        //    ref Vector3 impactPoint,
        //    ref Vector3 impactPlanePoint);

        public delegate void ExecuteMagicAttack(
            RulesetEffect activeEffect,
            GameLocationCharacter target,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage);

        // GameLocationBattleManager delegates
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

        //
        // Current "Invoke" implementations. They get all the relevant object parameters and pass them as ref to the delegates
        // This allows 2 scenarios: change these objects before they are yielded back (executed) or get these objects context
        //

        //public static void InvokeMagicEffectExecuteOnZone(Object obj, System.Type type)
        //{
        //    if (MagicEffectExecuteOnZoneHandler == null)
        //    {
        //        return;
        //    }

        //    var targets = (List<GameLocationCharacter>)AccessTools.Field(type, "targets").GetValue(obj);
        //    var actionModifiers = (List<ActionModifier>)AccessTools.Field(type, "actionModifiers").GetValue(obj);
        //    var castingPoint = (Vector3)AccessTools.Field(type, "castingPoint").GetValue(obj);
        //    var impactPoint = (Vector3)AccessTools.Field(type, "impactPoint").GetValue(obj);
        //    var impactPlanePoint = (Vector3)AccessTools.Field(type, "impactPlanePoint").GetValue(obj);

        //    MagicEffectExecuteOnZoneHandler
        //        .Invoke(ref targets, ref actionModifiers, castingPoint, impactPoint, impactPlanePoint);

        //    AccessTools.Field(type, "targets").SetValue(obj, targets);
        //    AccessTools.Field(type, "actionModifiers").SetValue(obj, actionModifiers);
        //    AccessTools.Field(type, "castingPoint").SetValue(obj, castingPoint);
        //    AccessTools.Field(type, "impactPoint").SetValue(obj, impactPoint);
        //    AccessTools.Field(type, "impactPlanePoint").SetValue(obj, impactPlanePoint);
        //}

        //public static void InvokeMagicEffectExecuteOnTargets(Object obj, System.Type type)
        //{
        //    if (MagicEffectExecuteOnTargetsHandler == null)
        //    {
        //        return;
        //    }

        //    var targets = (List<GameLocationCharacter>)AccessTools.Field(type, "targets").GetValue(obj);
        //    var actionModifiers = (List<ActionModifier>)AccessTools.Field(type, "actionModifiers").GetValue(obj);
        //    var castingPoint = (Vector3)AccessTools.Field(type, "castingPoint").GetValue(obj);
        //    var impactPoint = (Vector3)AccessTools.Field(type, "impactPoint").GetValue(obj);
        //    var impactPlanePoint = (Vector3)AccessTools.Field(type, "impactPlanePoint").GetValue(obj);

        //    MagicEffectExecuteOnTargetsHandler
        //        .Invoke(targets, actionModifiers, castingPoint, impactPoint, impactPlanePoint);

        //    AccessTools.Field(type, "targets").SetValue(obj, targets);
        //    AccessTools.Field(type, "actionModifiers").SetValue(obj, actionModifiers);
        //    AccessTools.Field(type, "castingPoint").SetValue(obj, castingPoint);
        //    AccessTools.Field(type, "impactPoint").SetValue(obj, impactPoint);
        //    AccessTools.Field(type, "impactPlanePoint").SetValue(obj, impactPlanePoint);
        //}

        //public static void InvokeMagicEffectExecuteOnPositions(Object obj, System.Type type)
        //{
        //    if (MagicEffectExecuteOnPositionsHandler == null)
        //    {
        //        return;
        //    }

        //    var positions = (List<int3>)AccessTools.Field(type, "positions").GetValue(obj);
        //    var castingPoint = (Vector3)AccessTools.Field(type, "castingPoint").GetValue(obj);
        //    var impactPoint = (Vector3)AccessTools.Field(type, "impactPoint").GetValue(obj);
        //    var impactPlanePoint = (Vector3)AccessTools.Field(type, "impactPlanePoint").GetValue(obj);

        //    MagicEffectExecuteOnPositionsHandler
        //        .Invoke(positions, castingPoint, impactPoint, impactPlanePoint);

        //    AccessTools.Field(type, "positions").SetValue(obj, positions);
        //    AccessTools.Field(type, "castingPoint").SetValue(obj, castingPoint);
        //    AccessTools.Field(type, "impactPoint").SetValue(obj, impactPoint);
        //    AccessTools.Field(type, "impactPlanePoint").SetValue(obj, impactPlanePoint);
        //}

        public static void InvokeExecuteMagicAttack(Object obj, System.Type type)
        {
            if (ExecuteMagicAttackHandler == null)
            {
                return;
            }

            var activeEffect = (RulesetEffect)AccessTools.Field(type, "activeEffect").GetValue(obj);
            var target = (GameLocationCharacter)AccessTools.Field(type, "target").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(type, "attackModifier").GetValue(obj);
            var actualEffectForms = (List<EffectForm>)AccessTools.Field(type, "actualEffectForms").GetValue(obj);
            var firstTarget = (bool)AccessTools.Field(type, "firstTarget").GetValue(obj);
            var checkMagicalAttackDamage = (bool)AccessTools.Field(type, "checkMagicalAttackDamage").GetValue(obj);

            ExecuteMagicAttackHandler
                .Invoke(activeEffect, target, attackModifier, actualEffectForms, firstTarget, checkMagicalAttackDamage);

            AccessTools.Field(type, "activeEffect").SetValue(obj, activeEffect);
            AccessTools.Field(type, "target").SetValue(obj, target);
            AccessTools.Field(type, "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(type, "actualEffectForms").SetValue(obj, actualEffectForms);
            AccessTools.Field(type, "firstTarget").SetValue(obj, firstTarget);
            AccessTools.Field(type, "checkMagicalAttackDamage").SetValue(obj, checkMagicalAttackDamage);
        }

        private static void InvokeHandleCharacterAttack(Object obj, System.Type type)
        {
            if (HandleCharacterAttackHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(type, "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(type, "attackModifier").GetValue(obj);
            var attackerAttackMode = (RulesetAttackMode)AccessTools.Field(type, "attackerAttackMode").GetValue(obj);

            HandleCharacterAttackHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackerAttackMode);

            AccessTools.Field(type, "attacker").SetValue(obj, attacker);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(type, "attackerAttackMode").SetValue(obj, attackerAttackMode);
        }

        private static void InvokeHandleRangeAttack(Object obj, System.Type type)
        {
            if (HandleRangeAttackHandler == null)
            {
                return;
            }

            var launcher = (GameLocationCharacter)AccessTools.Field(type, "launcher").GetValue(obj);
            var target = (GameLocationCharacter)AccessTools.Field(type, "target").GetValue(obj);
            var attackMode = (RulesetAttackMode)AccessTools.Field(type, "attackMode").GetValue(obj);
            var sourcePoint = (Vector3)AccessTools.Field(type, "sourcePoint").GetValue(obj);
            var impactPoint = (Vector3)AccessTools.Field(type, "impactPoint").GetValue(obj);
            var projectileFlightDuration = (float)AccessTools.Field(type, "projectileFlightDuration").GetValue(obj);

            HandleRangeAttackHandler
                .Invoke(ref launcher, ref target, ref attackMode, ref sourcePoint, ref impactPoint, ref projectileFlightDuration);

            AccessTools.Field(type, "launcher").SetValue(obj, launcher);
            AccessTools.Field(type, "target").SetValue(obj, target);
            AccessTools.Field(type, "attackMode").SetValue(obj, attackMode);
            AccessTools.Field(type, "sourcePoint").SetValue(obj, sourcePoint);
            AccessTools.Field(type, "impactPoint").SetValue(obj, impactPoint);
            AccessTools.Field(type, "projectileFlightDuration").SetValue(obj, projectileFlightDuration);
        }

        private static void InvokeHandleCharacterAttackHit(Object obj, System.Type type)
        {
            if (HandleCharacterAttackHitHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(type, "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(type, "attackModifier").GetValue(obj);
            var attackRoll = (int)AccessTools.Field(type, "attackRoll").GetValue(obj);
            var successDelta = (int)AccessTools.Field(type, "successDelta").GetValue(obj);
            var ranged = (bool)AccessTools.Field(type, "ranged").GetValue(obj);

            HandleCharacterAttackHitHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackRoll, ref successDelta, ref ranged);

            AccessTools.Field(type, "attacker").SetValue(obj, attacker);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(type, "attackRoll").SetValue(obj, attackRoll);
            AccessTools.Field(type, "successDelta").SetValue(obj, successDelta);
            AccessTools.Field(type, "ranged").SetValue(obj, ranged);
        }

        private static void InvokeHandleCharacterAttackDamage(Object obj, System.Type type)
        {
            if (HandleCharacterAttackDamageHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(type, "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(type, "attackModifier").GetValue(obj);
            var attackMode = (RulesetAttackMode)AccessTools.Field(type, "attackMode").GetValue(obj);
            var rangedAttack = (bool)AccessTools.Field(type, "rangedAttack").GetValue(obj);
            var advantageType = (RuleDefinitions.AdvantageType)AccessTools.Field(type, "advantageType").GetValue(obj);
            var actualEffectForms = (List<EffectForm>)AccessTools.Field(type, "actualEffectForms").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(type, "rulesetEffect").GetValue(obj);
            var criticalHit = (bool)AccessTools.Field(type, "criticalHit").GetValue(obj);
            var firstTarget = (bool)AccessTools.Field(type, "firstTarget").GetValue(obj);

            HandleCharacterAttackDamageHandler
                .Invoke(ref attacker, ref defender, ref attackModifier, ref attackMode, ref rangedAttack, ref advantageType, ref actualEffectForms, ref rulesetEffect, ref criticalHit, ref firstTarget);

            AccessTools.Field(type, "attacker").SetValue(obj, attacker);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "attackModifier").SetValue(obj, attackModifier);
            AccessTools.Field(type, "attackMode").SetValue(obj, attackMode);
            AccessTools.Field(type, "rangedAttack").SetValue(obj, rangedAttack);
            AccessTools.Field(type, "advantageType").SetValue(obj, advantageType);
            AccessTools.Field(type, "actualEffectForms").SetValue(obj, actualEffectForms);
            AccessTools.Field(type, "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(type, "criticalHit").SetValue(obj, criticalHit);
            AccessTools.Field(type, "firstTarget").SetValue(obj, firstTarget);
        }

        private static void InvokeHandleCharacterMagicalAttackDamage(Object obj, System.Type type)
        {
            if (HandleCharacterMagicalAttackDamageHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(type, "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var magicModifier = (ActionModifier)AccessTools.Field(type, "magicModifier").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(type, "rulesetEffect").GetValue(obj);
            var actualEffectForms = (List<EffectForm>)AccessTools.Field(type, "actualEffectForms").GetValue(obj);

            HandleCharacterMagicalAttackDamageHandler
                .Invoke(ref attacker, ref defender, ref magicModifier, ref rulesetEffect, ref actualEffectForms);

            AccessTools.Field(type, "attacker").SetValue(obj, attacker);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "magicModifier").SetValue(obj, magicModifier);
            AccessTools.Field(type, "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(type, "actualEffectForms").SetValue(obj, actualEffectForms);
        }

        private static void InvokeHandleReactionToDamageShare(Object obj, System.Type type)
        {
            if (HandleReactionToDamageShareHandler == null)
            {
                return;
            }

            var damagedCharacter = (GameLocationCharacter)AccessTools.Field(type, "damagedCharacter").GetValue(obj);
            var damageAmount = (int)AccessTools.Field(type, "damageAmount").GetValue(obj);

            HandleReactionToDamageShareHandler
                .Invoke(ref damagedCharacter, ref damageAmount);

            AccessTools.Field(type, "damagedCharacter").SetValue(obj, damagedCharacter);
            AccessTools.Field(type, "damageAmount").SetValue(obj, damageAmount);
        }

        private static void InvokeHandleCharacterAttackFinished(Object obj, System.Type type)
        {
            if (HandleCharacterAttackFinishedHandler == null)
            {
                return;
            }

            var attacker = (GameLocationCharacter)AccessTools.Field(type, "attacker").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var attackerAttackMode = (RulesetAttackMode)AccessTools.Field(type, "attackerAttackMode").GetValue(obj);

            HandleCharacterAttackFinishedHandler
                .Invoke(ref attacker, ref defender, ref attackerAttackMode);

            AccessTools.Field(type, "attacker").SetValue(obj, attacker);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "attackerAttackMode").SetValue(obj, attackerAttackMode);
        }

        private static void InvokeHandleSpellCast(Object obj, System.Type type)
        {
            if (HandleSpellCastHandler == null)
            {
                return;
            }

            var caster = (GameLocationCharacter)AccessTools.Field(type, "caster").GetValue(obj);
            var castAction = (CharacterActionCastSpell)AccessTools.Field(type, "castAction").GetValue(obj);
            var selectedRepertoire = (RulesetSpellRepertoire)AccessTools.Field(type, "selectedRepertoire").GetValue(obj);
            var selectedSpellDefinition = (SpellDefinition)AccessTools.Field(type, "selectedSpellDefinition").GetValue(obj);

            HandleSpellCastHandler
                .Invoke(ref caster, ref castAction, ref selectedRepertoire, ref selectedSpellDefinition);

            AccessTools.Field(type, "caster").SetValue(obj, caster);
            AccessTools.Field(type, "castAction").SetValue(obj, castAction);
            AccessTools.Field(type, "selectedRepertoire").SetValue(obj, selectedRepertoire);
            AccessTools.Field(type, "selectedSpellDefinition").SetValue(obj, selectedSpellDefinition);
        }

        private static void InvokeHandleSpellTargeted(Object obj, System.Type type)
        {
            if (HandleSpellTargetedHandler == null)
            {
                return;
            }

            var caster = (GameLocationCharacter)AccessTools.Field(type, "caster").GetValue(obj);
            var hostileSpell = (RulesetEffectSpell)AccessTools.Field(type, "hostileSpell").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var attackModifier = (ActionModifier)AccessTools.Field(type, "attackModifier").GetValue(obj);

            HandleSpellTargetedHandler
                .Invoke(ref caster, ref hostileSpell, ref defender, ref attackModifier);

            AccessTools.Field(type, "caster").SetValue(obj, caster);
            AccessTools.Field(type, "hostileSpell").SetValue(obj, hostileSpell);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "attackModifier").SetValue(obj, attackModifier);
        }

        private static void InvokeHandleFailedSavingThrowAgainstEffect(Object obj, System.Type type)
        {
            if (HandleFailedSavingThrowAgainstEffectHandler == null)
            {
                return;
            }

            var action = (CharacterActionMagicEffect)AccessTools.Field(type, "action").GetValue(obj);
            var caster = (GameLocationCharacter)AccessTools.Field(type, "caster").GetValue(obj);
            var defender = (GameLocationCharacter)AccessTools.Field(type, "defender").GetValue(obj);
            var rulesetEffect = (RulesetEffect)AccessTools.Field(type, "rulesetEffect").GetValue(obj);
            var saveModifier = (ActionModifier)AccessTools.Field(type, "saveModifier").GetValue(obj);
            var hasHitVisual = (bool)AccessTools.Field(type, "hasHitVisual").GetValue(obj);

            HandleFailedSavingThrowAgainstEffectHandler
                .Invoke(ref action, ref caster, ref defender, ref rulesetEffect, ref saveModifier, ref hasHitVisual);

            AccessTools.Field(type, "action").SetValue(obj, action);
            AccessTools.Field(type, "caster").SetValue(obj, caster);
            AccessTools.Field(type, "defender").SetValue(obj, defender);
            AccessTools.Field(type, "rulesetEffect").SetValue(obj, rulesetEffect);
            AccessTools.Field(type, "saveModifier").SetValue(obj, saveModifier);
            AccessTools.Field(type, "hasHitVisual").SetValue(obj, hasHitVisual);
        }

        //
        // Generic dispatcher patcher logic
        //

        // helper structure to dispatch the object being constructed to the correct "Invoke"
        private static readonly Dictionary<System.Type, System.Action<Object, System.Type>> EnumeratorMethodsDispatcher = new();

        public static void Dispatcher(Object obj)
        {
            var type = obj.GetType();

            if (EnumeratorMethodsDispatcher.TryGetValue(type, out var dispatcher))
            {
                dispatcher.Invoke(obj, type);
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var dispatcherMethod = typeof(ActionEnumeratorsContext).GetMethod("Dispatcher");

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
            var gameAssembly = Assembly.GetAssembly(typeof(IService));
            var harmony = new Harmony("SolastaCommunityExpansion");
            var transpiler = typeof(ActionEnumeratorsContext).GetMethod("Transpiler");

            foreach (var kvp in EnumeratorMethodsScope)
            {
                var arr = kvp.Key.Split('.');
                var type = gameAssembly.GetType(arr[0]);
                var internalTypes = type.GetNestedTypes(BindingFlags.NonPublic);
                var internalType = internalTypes.FirstOrDefault(x => x.Name.Contains(arr[1]));

                if (internalType != null)
                {
                    var handleMethod = type.GetMethod(arr[1]);

                    harmony.Patch(handleMethod, transpiler: new HarmonyMethod(transpiler));
                    EnumeratorMethodsDispatcher.Add(internalType, kvp.Value);
                }
            }
        }

        //
        // Sample code using delegates to log events
        //

        private static void EnableLogs()
        {
            if (!Main.Settings.EnableBattleServiceEventLogs)
            {
                return;
            }

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
                ref List<EffectForm> actualEffectForms
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
            if (!Main.Settings.EnableActionsEnumeratorContext)
            {
                return;
            }

            PatchAll();
            EnableLogs();
        }
    }
}
