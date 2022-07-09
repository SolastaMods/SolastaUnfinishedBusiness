using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Classes.Magus;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "AttackOn")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackOn
    {
        internal static void Prefix(GameLocationCharacter __instance,
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
        internal static void Prefix(GameLocationCharacter __instance,
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
        internal static void Postfix(GameLocationCharacter __instance, CharacterActionParams actionParams, ActionDefinitions.ActionScope scope)
        {
            if (scope != ActionDefinitions.ActionScope.Battle)
            {
                return;
            }

            if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
            {
                return;
            }

            var rulesetCharacter = actionParams.actingCharacter.RulesetCharacter;
            if (!rulesetCharacter.HasAnyFeature(Magus.SpellStrike))
            {
                return;
            }

            if (!Global.IsSpellStrike)
            {
                return;
            }

            var actionType = ActionDefinitions.ActionType.Main;
            var t1 = __instance.actionPerformancesByType[actionType];
            var t2 = __instance.currentActionRankByType[actionType];
            var maxAttacksNumber = -1;
            if (t1.Count > t2)
            {
                maxAttacksNumber = t1[t2].maxAttacksNumber;
            }
            int num = 0;
            if (actionParams.AttackMode != null)
            {
                num = actionParams.AttackMode.AttacksNumber;
            }
            else
            {
                foreach (RulesetAttackMode attackMode in actionParams.ActingCharacter.RulesetCharacter.AttackModes)
                {
                    if (attackMode.ActionType == actionType)
                    {
                        num = Mathf.Max(num, attackMode.AttacksNumber);
                    }
                }
            }
            __instance.UsedMainAttacks++;
            if (rulesetCharacter != null)
            {
                rulesetCharacter.ExecutedAttacks++;
                rulesetCharacter.RefreshAttackModes();
            }
            int num2 = ((maxAttacksNumber >= 0 && maxAttacksNumber < num) ? maxAttacksNumber : num);
            if (__instance.UsedMainAttacks >= num2)
            {
                __instance.currentActionRankByType[actionType]++;
                __instance.UsedMainAttacks = 0;
            }

            __instance.currentActionRankByType[actionType]--;
            __instance.RefreshActionPerformances();
        }
    }
}
