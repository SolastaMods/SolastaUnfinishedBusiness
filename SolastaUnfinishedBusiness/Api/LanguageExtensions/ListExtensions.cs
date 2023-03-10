using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

internal static class ListExtensions
{
    internal static void AddRange<T>([NotNull] this List<T> list, params T[] range)
    {
        list.AddRange(range);
    }

    internal static void SetRange<T>([NotNull] this List<T> list, [NotNull] params T[] range)
    {
        list.Clear();
        list.AddRange(range);
    }

    internal static void SetRange<T>([NotNull] this List<T> list, [NotNull] IEnumerable<T> range)
    {
        list.Clear();
        list.AddRange(range);
    }
}
