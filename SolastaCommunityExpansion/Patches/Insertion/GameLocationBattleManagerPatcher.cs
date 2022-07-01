using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using TA;

namespace SolastaCommunityExpansion.Patches.Insertion;

internal static class GameLocationBattleManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationBattleManager), "CanAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_CanAttack
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            RangedAttackInMeleeDisadvantageRemover.ApplyTranspile(code);

            return code;
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


    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveStart
    {
        internal static void Prefix(GameLocationBattleManager __instance,
            GameLocationCharacter mover,
            int3 destination
        )
        {
            AttacksOfOpportunity.ProcessOnCharacterMoveStart(mover, destination);
        }
    }

    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterMoveEnd")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleCharacterMoveEnd
    {
        internal static void Prefix(GameLocationCharacter mover)
        {
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

            foreach (var condition in matchingOccurenceConditions)
            {
                mover.RulesetActor.ExecuteRecurrentForms(condition);
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
    internal static class PrepareBattleEnd
    {
        internal static void Prefix(GameLocationBattleManager __instance)
        {
            AttacksOfOpportunity.CleanMovingCache();
        }
    }
}
