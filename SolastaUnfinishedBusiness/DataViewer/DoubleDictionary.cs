using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.DataViewer;

internal class DoubleDictionary<TKey1, TKey2, TValue>
{
    private readonly Dictionary<TKey1, Dictionary<TKey2, TValue>> _dictionary
        = new();

    internal TValue this[TKey1 key1, TKey2 key2]
    {
        set => _dictionary[key1][key2] = value;
    }

    internal bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
    {
        if (!_dictionary.TryGetValue(key1, out var innerDictionary))
        {
            _dictionary.Add(key1, innerDictionary = new Dictionary<TKey2, TValue>());
        }

        return innerDictionary.TryGetValue(key2, out value);
    }
}
