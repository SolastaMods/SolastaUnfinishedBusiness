using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

internal static class ArrayExtensions
{
    internal static void ForEach([NotNull] this Array array, Action<Array, int[]> action)
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
    private readonly int[] _maxLengths;

    internal ArrayTraverse([NotNull] Array array)
    {
        _maxLengths = new int[array.Rank];

        for (var i = 0; i < array.Rank; ++i)
        {
            _maxLengths[i] = array.GetLength(i) - 1;
        }

        Position = new int[array.Rank];
    }

    internal int[] Position { get; }

    internal bool Step()
    {
        for (var i = 0; i < Position.Length; ++i)
        {
            if (Position[i] >= _maxLengths[i])
            {
                continue;
            }

            Position[i]++;

            for (var j = 0; j < i; j++)
            {
                Position[j] = 0;
            }

            return true;
        }

        return false;
    }
}
