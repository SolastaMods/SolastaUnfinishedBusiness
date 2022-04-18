using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.OnCharacterAttackEffect
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttack
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (var feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackEffect>())
            {
                feature.OnAttack(attacker, defender, attackModifier, attackerAttackMode);
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackHit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackHit
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (var feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
            {
                feature.OnAttackHit(attacker, defender, attackModifier, attackRoll, successDelta, ranged);
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackDamage
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            GameLocationCharacter defender, 
            ActionModifier attackModifier, 
            RulesetAttackMode attackMode,
            bool rangedAttack, RuleDefinitions.AdvantageType advantageType, 
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect, 
            bool criticalHit, 
            bool firstTarget)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (var feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
            {
                feature.OnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMagicalAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterMagicalAttackDamage
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (var feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnMagicalAttackDamageEffect>())
            {
                feature.OnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
            }
        }
    }

    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackFinished")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackFinished
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (var feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackFinishedEffect>())
            {
                feature.OnAttackFinished(attacker, defender, attackerAttackMode);
            }
        }
    }
}
