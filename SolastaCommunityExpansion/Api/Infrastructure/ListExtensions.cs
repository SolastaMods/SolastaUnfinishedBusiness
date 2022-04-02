using System.Collections.Generic;
using System.Linq;

namespace SolastaModApi.Infrastructure
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this List<T> list, params T[] range)
        {
            list.AddRange(range.AsEnumerable());
        }

        public static void SetRange<T>(this List<T> list, params T[] range)
        {
            list.Clear();
            list.AddRange(range);
        }

        public static void SetRange<T>(this List<T> list, IEnumerable<T> range)
        {
            list.Clear();
            list.AddRange(range);
        }
    }
}
