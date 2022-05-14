using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Utils
{
    public static class StringUtils
    {
        public static string ToString<T>(IEnumerable<T> list) where T : BaseDefinition
        {
            return $"[{string.Join(", ", list.Select(e => $"'{e.Name}'"))}]";
        }

        public static string ToStringAsDefinition<T>(IEnumerable<T> list)
        {
            return $"[{string.Join(", ", list.Select(e => $"'{(e as BaseDefinition)?.Name}'"))}]";
        }
    }
}
