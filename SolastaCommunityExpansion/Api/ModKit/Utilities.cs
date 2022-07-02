// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using System;
using System.Collections.Generic;
using System.Linq;

namespace ModKit;

public static class Utilties
{
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
        TValue defaultValue = default)
    {
        if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); } // using C# 6

        if (key == null) { throw new ArgumentNullException(nameof(key)); } //  using C# 6

        return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public static Dictionary<string, TEnum> NameToValueDictionary<TEnum>(this TEnum enumValue) where TEnum : struct
    {
        var enumType = enumValue.GetType();
        return Enum.GetValues(enumType)
            .Cast<TEnum>()
            .ToDictionary(e => Enum.GetName(enumType, e), e => e);
    }
}
