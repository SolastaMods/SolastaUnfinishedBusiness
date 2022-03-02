using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    //
    // this patch shouldn't be protected
    //
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
            BattleContext.SetHandleCharacterAttackDamageContext(
                isPrefix: true,
                attacker,
                defender,
                attackModifier,
                attackMode,
                rangedAttack,
                advantageType,
                actualEffectForms,
                rulesetEffect,
                criticalHit,
                firstTarget);

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            BattleContext.SetHandleCharacterAttackDamageContext(
                isPrefix: false,
                attacker,
                defender,
                attackModifier,
                attackMode,
                rangedAttack,
                advantageType,
                actualEffectForms,
                rulesetEffect,
                criticalHit,
                firstTarget);
        }

        internal static void Postfix(GameLocationCharacter attacker,
            GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
            bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (IOnAttackHitEffect feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
            {
                feature.OnAttackHit(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }
    }


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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }

    //[HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
    //[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    //internal static class GameLocationBattleManager_HandleCharacterAttackDamage
    //{
    //    internal static IEnumerator Postfix(
    //        IEnumerator values,
    //        GameLocationCharacter attacker,
    //        GameLocationCharacter defender,
    //        ActionModifier attackModifier,
    //        RulesetAttackMode attackMode,
    //        bool rangedAttack,
    //        RuleDefinitions.AdvantageType advantageType,
    //        List<EffectForm> actualEffectForms,
    //        RulesetEffect rulesetEffect,
    //        bool criticalHit,
    //        bool firstTarget)
    //    {
    //        while (values.MoveNext())
    //        {
    //            yield return values.Current;
    //        }
    //    }
    //}

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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
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
            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }
}
