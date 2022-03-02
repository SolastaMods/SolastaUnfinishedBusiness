using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;
using static SolastaCommunityExpansion.Models.ModContext;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttack
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            HandleCharacterAttackHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackerAttackMode,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterAttackHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackerAttackMode,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackDamage
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            HandleCharacterAttackDamageHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackMode,
                ref rangedAttack,
                ref advantageType,
                ref actualEffectForms,
                ref rulesetEffect,
                ref criticalHit,
                ref firstTarget,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterAttackDamageHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackMode,
                ref rangedAttack,
                ref advantageType,
                ref actualEffectForms,
                ref rulesetEffect,
                ref criticalHit,
                ref firstTarget,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackFinished")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackFinished
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode)
        {
            HandleCharacterAttackFinishedHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackerAttackMode,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterAttackFinishedHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackerAttackMode,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackHit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackHit
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged)
        {
            HandleCharacterAttackHitHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackRoll,
                ref successDelta,
                ref ranged,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterAttackHitHandler?.Invoke(
                ref attacker,
                ref defender,
                ref attackModifier,
                ref attackRoll,
                ref successDelta,
                ref ranged,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMagicalAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterMagicalAttackDamage
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect activeEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget)
        {
            HandleCharacterMagicalAttackDamageHandler?.Invoke(
                ref attacker,
                ref defender,
                ref magicModifier,
                ref activeEffect,
                ref actualEffectForms,
                ref firstTarget,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterMagicalAttackDamageHandler?.Invoke(
                ref attacker,
                ref defender,
                ref magicModifier,
                ref activeEffect,
                ref actualEffectForms,
                ref firstTarget,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterMoveEnd
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter mover)
        {
            HandleCharacterMoveEndHandler?.Invoke(
                ref mover,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterMoveEndHandler?.Invoke(
                ref mover,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterMoveStart
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter mover,
            TA.int3 destination)
        {
            HandleCharacterMoveStartHandler?.Invoke(
                ref mover,
                ref destination,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterMoveStartHandler?.Invoke(
                ref mover,
                ref destination,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterSurprise")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterSurprise
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter character)
        {
            HandleCharacterSurpriseHandler?.Invoke(
                ref character,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleCharacterSurpriseHandler?.Invoke(
                ref character,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleFailedSavingThrowAgainstEffect")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleFailedSavingThrowAgainstEffect
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            CharacterActionMagicEffect action,
            GameLocationCharacter caster,
            GameLocationCharacter defender,
            RulesetEffect rulesetEffect,
            ActionModifier saveModifier,
            bool hasHitVisual)
        {
            HandleFailedSavingThrowAgainstEffectHandler?.Invoke(
                ref action,
                ref caster,
                ref defender,
                ref rulesetEffect,
                ref saveModifier,
                ref hasHitVisual,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleFailedSavingThrowAgainstEffectHandler?.Invoke(
                ref action,
                ref caster,
                ref defender,
                ref rulesetEffect,
                ref saveModifier,
                ref hasHitVisual,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleRangeAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleRangeAttack
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter launcher,
            GameLocationCharacter target,
            RulesetAttackMode attackMode,
            Vector3 sourcePoint,
            Vector3 impactPoint,
            float projectileFlightDuration)
        {
            HandleRangeAttackHandler?.Invoke(
                ref launcher,
                ref target,
                ref attackMode,
                ref sourcePoint,
                ref impactPoint,
                ref projectileFlightDuration,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleRangeAttackHandler?.Invoke(
                ref launcher,
                ref target,
                ref attackMode,
                ref sourcePoint,
                ref impactPoint,
                ref projectileFlightDuration,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleReactionToDamageShare")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleReactionToDamageShare
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter damagedCharacter,
            int damageAmount)
        {
            HandleReactionToDamageShareHandler?.Invoke(
                ref damagedCharacter,
                ref damageAmount,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleReactionToDamageShareHandler?.Invoke(
                ref damagedCharacter,
                ref damageAmount,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleReactionToRageStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleReactionToRageStart
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter rager)
        {
            HandleReactionToRageStartHandler?.Invoke(
                ref rager,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleReactionToRageStartHandler?.Invoke(
                ref rager,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleSpellCast")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleSpellCast
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter caster,
            CharacterActionCastSpell castAction,
            RulesetSpellRepertoire selectedRepertoire,
            SpellDefinition selectedSpellDefinition)
        {
            HandleSpellCastHandler?.Invoke(
                ref caster,
                ref castAction,
                ref selectedRepertoire,
                ref selectedSpellDefinition,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleSpellCastHandler?.Invoke(
                ref caster,
                ref castAction,
                ref selectedRepertoire,
                ref selectedSpellDefinition,
                isPrefix: false);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleSpellTargeted")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleSpellTargeted
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationCharacter caster,
            RulesetEffectSpell hostileSpell,
            GameLocationCharacter defender,
            ActionModifier attackModifier)
        {
            HandleSpellTargetedHandler?.Invoke(
                ref caster,
                ref hostileSpell,
                ref defender,
                ref attackModifier,
                isPrefix: true);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            HandleSpellTargetedHandler?.Invoke(
                ref caster,
                ref hostileSpell,
                ref defender,
                ref attackModifier,
                isPrefix: false);
        }
    }
}
