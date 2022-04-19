using System.Collections;
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
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackHit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackHit
    {
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            int attackRoll,
            int successDelta,
            bool ranged)
        {
            Main.Logger.Log("HandleCharacterAttackHit");

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
                {
                    feature.BeforeOnAttackHit(attacker, defender, attackModifier, attackRoll, successDelta, ranged);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
                {
                    feature.AfterOnAttackHit(attacker, defender, attackModifier, attackRoll, successDelta, ranged);
                }
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
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
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
            Main.Logger.Log("HandleCharacterAttackDamage");

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
                {
                    feature.BeforeOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter != null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackDamageEffect>())
                {
                    feature.AfterOnAttackDamage(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
                }
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
        internal static IEnumerator Postfix(
            IEnumerator values,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            Main.Logger.Log("HandleCharacterMagicalAttackDamage");

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnMagicalAttackDamageEffect>())
                {
                    feature.BeforeOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
                }
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            if (rulesetCharacter == null)
            {
                foreach (var feature in rulesetCharacter.EnumerateFeaturesToBrowse<IOnMagicalAttackDamageEffect>())
                {
                    feature.AfterOnMagicalAttackDamage(attacker, defender, magicModifier, rulesetEffect, actualEffectForms, firstTarget, criticalHit);
                }
            }
        }
    }
}
