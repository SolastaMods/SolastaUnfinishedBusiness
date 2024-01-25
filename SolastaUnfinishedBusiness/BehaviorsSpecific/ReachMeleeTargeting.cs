using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA;

namespace SolastaUnfinishedBusiness.BehaviorsSpecific;

internal static class ReachMeleeTargeting
{
    // Used in `CursorLocationBattleFriendlyTurnPatcher`
    // ReSharper disable once UnusedMember.Global
    internal static bool FindBestActionDestination(
        GameLocationCharacter actor,
        GameLocationCharacter target,
        ref int3 actorPosition,
        CursorLocationBattleFriendlyTurn cursor,
        RulesetAttackMode attackMode)
    {
        var reachRange = attackMode.ReachRange;
        var validDestinations = cursor.validDestinations;

        if (actor.IsWithinRange(target, reachRange) || validDestinations.Count == 0)
        {
            return true;
        }

        var battleBoundingBox = target.LocationBattleBoundingBox;

        battleBoundingBox.Inflate(reachRange);

        var foundDestination = false;

        var current = new GameLocationCharacterDefinitions.PathStep { moveCost = 99999 };
        var distance = (current.position - actorPosition).magnitudeSqr;

        foreach (var destination in validDestinations
                     .Where(destination => battleBoundingBox.Contains(destination.position)))
        {
            bool better;

            if (foundDestination && destination.moveCost >= current.moveCost)
            {
                if (destination.moveCost == current.moveCost)
                {
                    better = (destination.position - actorPosition).magnitudeSqr < distance;
                }
                else
                {
                    better = false;
                }
            }
            else
            {
                better = true;
            }

            if (!better)
            {
                continue;
            }

            current = destination;
            distance = (current.position - actorPosition).magnitudeSqr;
            foundDestination = true;
        }

        if (foundDestination)
        {
            actorPosition = current.position;
        }

        return foundDestination;
    }
}
