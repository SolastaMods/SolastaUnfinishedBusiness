using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class DictionaryExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void TryAddRange<TK, TV>(this Dictionary<TK, TV> dict, IEnumerable<TK> keys, TV value)
    {
        foreach (var key in keys)
        {
            dict.TryAdd(key, value);
        }
    }
}
