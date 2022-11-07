using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

public static class DictionaryExtensions
{
    public static void TryAddRange<K, V>(this Dictionary<K, V> dict, IEnumerable<K> keys, V value)
    {
        foreach (var key in keys)
        {
            dict.TryAdd(key, value);
        }
    }
}
