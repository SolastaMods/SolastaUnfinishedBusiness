using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using TA;

namespace SolastaCommunityExpansion.CustomUI;

public static class ReachMeleeTargeting
{
    // Replaces call to `FindBestActionDestination` with custom method that respects attack mode's reach
    // Needed for reach melee
    public static void ApplyCursorLocationIsValidAttackTranspile(List<CodeInstruction> instructions)
    {
        var insertionIndex = instructions.FindIndex(x =>
            x.opcode == OpCodes.Call && x.operand.ToString().Contains("FindBestActionDestination"));

        if (insertionIndex > 0)
        {
            var method = typeof(ReachMeleeTargeting)
                .GetMethod("FindBestActionDestination", BindingFlags.Static | BindingFlags.NonPublic);

            instructions[insertionIndex] = new CodeInstruction(OpCodes.Call, method);
            instructions.InsertRange(insertionIndex,
                new[] {new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldloc_1)});
        }
    }

    // Used in `ApplyCursorLocationIsValidAttackTranspile`
    private static bool FindBestActionDestination(
        GameLocationCharacter actor,
        GameLocationCharacter target,
        ref int3 actorPosition,
        CursorLocationBattleFriendlyTurn cursor,
        RulesetAttackMode attackMode)
    {
        var reachRange = attackMode.ReachRange;
        var validDestinations =
            cursor.GetField<List<GameLocationCharacterDefinitions.PathStep>>("validDestinations");

        if (cursor.BattleService.IsWithinXCells(actor, target, reachRange) || validDestinations.Empty())
        {
            return true;
        }

        var battleBoundingBox = target.LocationBattleBoundingBox;
        battleBoundingBox.Inflate(reachRange);
        var foundDestination = false;

        var current = new GameLocationCharacterDefinitions.PathStep {moveCost = 99999};
        var distance = (current.position - actorPosition).magnitudeSqr;

        foreach (var destination in validDestinations)
        {
            if (battleBoundingBox.Contains(destination.position)
                && (!foundDestination // haven't found yet
                    || destination.moveCost < current.moveCost // or path is shorter
                    || (destination.moveCost == current.moveCost // or same length
                        && (destination.position - actorPosition).magnitudeSqr < distance))) // but closer to start
            {
                current = destination;
                distance = (current.position - actorPosition).magnitudeSqr;
                foundDestination = true;
            }
        }

        if (foundDestination)
        {
            actorPosition = current.position;
        }

        return foundDestination;
    }
}
