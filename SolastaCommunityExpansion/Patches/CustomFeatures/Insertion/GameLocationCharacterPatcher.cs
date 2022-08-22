using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Insertion;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var character = __instance.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();

            foreach (var effect in features)
            {
                effect.BeforeOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "AttackImpactOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackImpactOn
    {
        internal static void Prefix(
            [NotNull] GameLocationCharacter __instance,
            GameLocationCharacter target,
            RuleDefinitions.RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var character = __instance.RulesetCharacter;
            if (character == null)
            {
                return;
            }

            var features = character.GetSubFeaturesByType<IOnAttackHitEffect>();
            foreach (var effect in features)
            {
                effect.AfterOnAttackHit(__instance, target, outcome, actionParams, attackMode, attackModifier);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationCharacter), "HandleActionExecution")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_HandleActionExecution
    {
        internal static void Postfix(
            GameLocationCharacter __instance,
            CharacterActionParams actionParams,
            ActionDefinitions.ActionScope scope)
        {
            if (scope != ActionDefinitions.ActionScope.Battle)
            {
                return;
            }

            var rulesetCharacter = actionParams.actingCharacter.RulesetCharacter;

            if (!rulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>())
            {
                return;
            }

            if (!Global.IsSpellStrike && !rulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>())
            {
                return;
            }

            if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain &&
                actionParams.actionDefinition.Id != ActionDefinitions.Id.AttackMain)
            {
                return;
            }

            if (actionParams.RulesetEffect is not RulesetEffectSpell spellEffect ||
                spellEffect.spellDefinition.spellLevel > 0)
            {
                return;
            }

            var num = 0;
            foreach (var attackMode in actionParams.ActingCharacter.RulesetCharacter.AttackModes)
            {
                if (attackMode.ActionType == ActionDefinitions.ActionType.Main)
                {
                    num = Mathf.Max(num, attackMode.AttacksNumber);
                }
            }

            if (actionParams.actionDefinition.Id == ActionDefinitions.Id.CastMain)
            {
                __instance.usedMainAttacks++;
                if (__instance.usedMainAttacks < num)
                {
                    __instance.currentActionRankByType[ActionDefinitions.ActionType.Main]--;
                }
                else
                {
                    __instance.usedMainAttacks = 0;
                }
            }
        }
    }
}

[HarmonyPatch(typeof(GameLocationCharacter), "GetActionStatus")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacter_GetActionStatus
{
    internal static void Postfix(ref GameLocationCharacter __instance, ActionDefinitions.Id actionId,
        ActionDefinitions.ActionScope scope, ref ActionDefinitions.ActionStatus __result)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        if (actionId != ActionDefinitions.Id.CastMain)
        {
            return;
        }

        if (!__instance.RulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>())
        {
            return;
        }

        if (__instance.usedMainAttacks == 0)
        {
            return;
        }

        if (__instance.usedMainCantrip)
        {
            return;
        }

        __result = ActionDefinitions.ActionStatus.Available;
    }
}
