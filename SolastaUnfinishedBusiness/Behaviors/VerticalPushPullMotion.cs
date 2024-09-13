using System;
using TA;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Behaviors;

internal static class VerticalPushPullMotion
{
    private const double Epsilon = 0.015;
    private const double StepHigh = 0.4 - Epsilon;
    private const double StepLow = 0.1 - Epsilon;

    public static bool ComputePushDestination(
        Vector3 sourceCenter,
        GameLocationCharacter target,
        int distance,
        bool reverse,
        IGameLocationPositioningService positioningService,
        ref int3 destination,
        ref Vector3 step)
    {
        var targetCenter = new Vector3();
        positioningService.ComputeGravityCenterPosition(target, ref targetCenter);
        var direction = targetCenter - sourceCenter;
        if (reverse)
        {
            direction = -direction;
            var b = (int)Math.Max(Math.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y)), Mathf.Abs(direction.z));
            distance = distance <= 0 ? b : Mathf.Min(distance, b);
        }
        else
        {
            distance = Mathf.Max(1, distance);
        }

        step = direction.normalized;
        destination = target.LocationPosition;
        var position = target.LocationPosition;
        var delta = new Vector3();
        var result = false;

        for (var index = 0; index < distance; ++index)
        {
            delta += step;

            var sides = Step(delta, StepHigh);
            if (sides is { x: 0, y: 0, z: 0 })
            {
                sides = Step(delta, StepLow);
            }

            position += sides;
            delta.x = sides.x == 0 ? delta.x : 0;
            delta.y = sides.y == 0 ? delta.y : 0;
            delta.z = sides.z == 0 ? delta.z : 0;

            var allSurfaceSides = CellFlags.DirectionToAllSurfaceSides(sides);
            var flag = true;
            var canMoveThroughWalls = target.CanMoveInSituation(RulesetCharacter.MotionRange.ThroughWalls);
            foreach (var allSide in CellFlags.AllSides)
            {
                if (flag && (allSide & allSurfaceSides) != CellFlags.Side.None)
                {
                    flag &= positioningService.CanCharacterMoveThroughSide(target.SizeParameters, position, allSide,
                        canMoveThroughWalls);
                }
            }

            if (flag && positioningService.CanPlaceCharacter(target, position, CellHelpers.PlacementMode.Station))
            {
                destination = position;
                result = true;
            }
            else
            {
                break;
            }
        }

        //TODO: remove after testing
#if DEBUG
        var dir = reverse ? "Pull" : "Push";
        Main.Log(
            $"{dir} [{target.Name}] {distance}\u25ce applied: {result}, source: {sourceCenter}, target: {targetCenter}, destination: {destination}",
            true);
#endif

        return result;
    }

    private static int3 Step(Vector3 delta, double tolerance)
    {
        return new int3(
            (int)Mathf.Sign(delta.x) * (Mathf.Abs(delta.x) < tolerance ? 0 : 1),
            (int)Mathf.Sign(delta.y) * (Mathf.Abs(delta.y) < tolerance ? 0 : 1),
            (int)Mathf.Sign(delta.z) * (Mathf.Abs(delta.z) < tolerance ? 0 : 1)
        );
    }
}

internal class SlamDown
{
    private SlamDown()
    {
    }

    public static SlamDown Mark { get; } = new();
}
