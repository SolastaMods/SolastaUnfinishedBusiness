using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

public static class DictionaryExtensions
{
    public static void TryAddRange<TK, TV>(this Dictionary<TK, TV> dict, IEnumerable<TK> keys, TV value)
    {
        foreach (var key in keys)
        {
            dict.TryAdd(key, value);
        }
    }
}
