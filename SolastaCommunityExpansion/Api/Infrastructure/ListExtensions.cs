using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Infrastructure;

public static class ListExtensions
{
    public static void AddRange<T>([NotNull] this List<T> list, params T[] range)
    {
        list.AddRange(range.AsEnumerable());
    }

    public static void SetRange<T>([NotNull] this List<T> list, [NotNull] params T[] range)
    {
        list.Clear();
        list.AddRange(range);
    }

    public static void SetRange<T>([NotNull] this List<T> list, [NotNull] IEnumerable<T> range)
    {
        list.Clear();
        list.AddRange(range);
    }
}
