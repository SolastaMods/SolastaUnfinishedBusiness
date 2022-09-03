using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches;

internal static class GameLocationBattleManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationBattleManager), "CanPerformReadiedActionOnCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanPerformReadiedActionOnCharacter_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            //PATCH: Makes only preferred cantrip valid if it is selected and forced
            CustomReactionsContext.ForcePreferredCantripUsage(codes);

            return codes.AsEnumerable();
        }
    }
    
    [HarmonyPatch(typeof(GameLocationBattleManager), "IsValidAttackForReadiedAction")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsValidAttackForReadiedAction_Patch
    {
        internal static void Postfix(
            GameLocationBattleManager __instance,
            ref bool __result,
            BattleDefinitions.AttackEvaluationParams attackParams,
            bool forbidDisadvantage)
        {
            //PATCH: Checks if attack cantrip is valid to be cast as readied action on a target
            // Used to properly check if melee cantrip can hit target when used for readied action
            
            if (!DatabaseHelper.TryGetDefinition<SpellDefinition>(attackParams.effectName, null, out var cantrip))
            {
                return;
            }

            var canAttack = cantrip.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>()?.CanAttack;

            if (canAttack != null)
            {
                __result = canAttack(attackParams.attacker, attackParams.defender);
            }
        }
    }
    
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveStart_Patch
    {
        internal static void Prefix(GameLocationBattleManager __instance,
            GameLocationCharacter mover,
            int3 destination
        )
        {
            //PATCH: support for Polearm Expert AoO
            //Stores character movements to be processed later
            AttacksOfOpportunity.ProcessOnCharacterMoveStart(mover, destination);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveEnd_Patch
    {
        internal static void Prefix(GameLocationCharacter mover)
        {
            //PATCH: support for conditions that trigger on movement end
            //Mostly for Magus's `Rupture Strike`
            //TODO: move this code to separate file
            
            if (mover.RulesetCharacter.isDeadOrDyingOrUnconscious)
            {
                return;
            }

            var matchingOccurenceConditions = new List<RulesetCondition>();
            foreach (var item2 in mover.RulesetCharacter.ConditionsByCategory
                         .SelectMany(item => item.Value))
            {
                switch (item2.endOccurence)
                {
                    case (RuleDefinitions.TurnOccurenceType)ExtraTurnOccurenceType.OnMoveEnd:
                        matchingOccurenceConditions.Add(item2);
                        break;
                }
            }

            var effectManager =
                ServiceRepository.GetService<IWorldLocationSpecialEffectsService>() as
                    WorldLocationSpecialEffectsManager;

            foreach (var condition in matchingOccurenceConditions)
            {
                Main.Log($"source character GUID {condition.sourceGuid}");

                if (effectManager != null)
                {
                    effectManager.ConditionAdded(mover.RulesetCharacter, condition, true);
                    mover.RulesetActor.ExecuteRecurrentForms(condition);
                    effectManager.ConditionRemoved(mover.RulesetCharacter, condition);
                }

                if (condition.HasFinished && !condition.IsDurationDefinedByEffect())
                {
                    mover.RulesetActor.RemoveCondition(condition);
                    mover.RulesetActor.ProcessConditionDurationEnded(condition);
                }
                else if (condition.CanSaveToCancel && condition.HasSaveOverride)
                {
                    mover.RulesetActor.SaveToCancelCondition(condition);
                }
                else
                {
                    mover.RulesetActor.ConditionOccurenceReached?.Invoke(mover.RulesetActor, condition);
                }
            }
        }

        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter mover
        )
        {
            //PATCH: support for Polearm Expert AoO
            //processes saved movenent to trigger AoO when appropriate
            
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            var extraEvents = AttacksOfOpportunity.ProcessOnCharacterMoveEnd(__instance, mover);

            while (extraEvents.MoveNext())
            {
                yield return extraEvents.Current;
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "PrepareBattleEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PrepareBattleEnd_Patch
    {
        internal static void Prefix(GameLocationBattleManager __instance)
        {
            //PATCH: support for Polearm Expert AoO
            //clears movement cache on battle end
            
            AttacksOfOpportunity.CleanMovingCache();
        }
    }
    
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackFinished")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterAttackFinished
    {
        internal static IEnumerator Postfix(
            IEnumerator __result,
            GameLocationBattleManager __instance,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode
        )
        {
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            var extraEvents =
                AttacksOfOpportunity.ProcessOnCharacterAttackFinished(__instance, attacker, defender,
                    attackerAttackMode);

            while (extraEvents.MoveNext())
            {
                yield return extraEvents.Current;
            }
        }
    }
}