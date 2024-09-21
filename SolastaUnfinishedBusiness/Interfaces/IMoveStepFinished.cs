using System.Collections.Generic;
using JetBrains.Annotations;
using TA;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMoveStepFinished
{
    public void MoveStepFinished(GameLocationCharacter mover, int3 previousPosition);
}

internal static class MoveStepFinished
{
    private static readonly Dictionary<ulong, (int3, int3)> MovementCache = [];

    internal static bool TryGetMovement(ulong guid, out (int3, int3) movement)
    {
        if (MovementCache.TryGetValue(guid, out movement))
        {
            return true;
        }

        movement = (int3.invalid, int3.invalid);

        return false;
    }

    internal static void RecordMovement([NotNull] GameLocationCharacter mover, int3 destination)
    {
        MovementCache.AddOrReplace(mover.Guid, (mover.locationPosition, destination));
    }

    internal static void CleanMovementCache()
    {
        MovementCache.Clear();
    }
}
