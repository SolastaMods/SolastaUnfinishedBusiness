using System;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class ArrayExtensions
{
    public static void ForEach([NotNull] this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0)
        {
            return;
        }

        var walker = new ArrayTraverse(array);
        do
        {
            action(array, walker.Position);
        } while (walker.Step());
    }
}

internal sealed class ArrayTraverse
{
    private readonly int[] maxLengths;

    public ArrayTraverse([NotNull] Array array)
    {
        maxLengths = new int[array.Rank];
        for (var i = 0; i < array.Rank; ++i)
        {
            maxLengths[i] = array.GetLength(i) - 1;
        }

        Position = new int[array.Rank];
    }

    public int[] Position { get; }

    public bool Step()
    {
        for (var i = 0; i < Position.Length; ++i)
        {
            if (Position[i] < maxLengths[i])
            {
                Position[i]++;
                for (var j = 0; j < i; j++)
                {
                    Position[j] = 0;
                }

                return true;
            }
        }

        return false;
    }
}
